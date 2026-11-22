using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Modulos.Compra;

using MySql.Data.MySqlClient;

using System.Data;
using System.Globalization;
using System.Text;

namespace aDVanceERP.Core.Repositorios.Modulos.Compra {
    /// <summary>
    /// Recepción de mercancía de una compra. TODO ocurre en una sola transacción InnoDB:
    /// recepción + detalle, movimiento de entrada, inventario (cantidad, costo promedio, valor), historial de costo,
    /// último costo del producto, acumulados de la compra, estado de recepción y bitácora.
    /// Si algo falla, no queda nada a medias.
    /// </summary>
    public class RepoRecepcionCompra {
        public static RepoRecepcionCompra Instancia { get; } = new();

        private const long IdTipoMovimientoEntradaCompra = 1;

        /// <summary>true si la compra ya tiene alguna recepción registrada (movió inventario y costo).</summary>
        public bool TieneRecepciones(long idCompra) =>
            ContextoBaseDatos.EjecutarConsultaEscalar<long>(
                "SELECT COUNT(*) FROM adv__recepcion_compra WHERE id_compra = @id;",
                new Dictionary<string, object> { { "@id", idCompra } }) > 0;

        /// <summary>Líneas de la compra con lo recibido, lo pendiente y la existencia actual en el almacén destino.</summary>
        public List<LineaPendienteRecepcion> ObtenerLineas(long idCompra) {
            const string consulta = """
                SELECT d.id_detalle_compra_producto, d.id_producto, IFNULL(d.id_presentacion, 0) AS id_presentacion,
                       p.nombre AS nombre_producto,
                       IFNULL(um.abreviatura, '-') AS unidad,
                       d.factor_presentacion, d.cantidad, d.cantidad_recibida, d.cantidad_devuelta,
                       d.costo_unitario_base, d.descuento_item, d.impuesto_adicional_item,
                       IFNULL(i.cantidad, 0) AS existencia, IFNULL(i.costo_promedio, 0) AS costo_promedio
                FROM adv__detalle_compra_producto d
                INNER JOIN adv__compra c ON c.id_compra = d.id_compra
                INNER JOIN adv__producto p ON p.id_producto = d.id_producto
                LEFT JOIN adv__precio_presentacion pp ON pp.id_presentacion = d.id_presentacion
                LEFT JOIN adv__unidad_medida um ON um.id_unidad_medida = IF(d.id_presentacion IS NULL OR d.id_presentacion = 0, p.id_unidad_medida, pp.id_unidad_medida)
                LEFT JOIN adv__inventario i ON i.id_producto = d.id_producto AND i.id_almacen = c.id_almacen_destino
                WHERE d.id_compra = @id_compra
                ORDER BY d.id_detalle_compra_producto;
                """;

            var lista = new List<LineaPendienteRecepcion>();

            using var conexion = ContextoBaseDatos.ObtenerConexion();
            using var comando = new MySqlCommand(consulta, conexion);
            comando.Parameters.AddWithValue("@id_compra", idCompra);
            using var lector = comando.ExecuteReader();

            while (lector.Read()) {
                var factor = ConvertirDecimal(lector["factor_presentacion"], 1m);
                if (factor <= 0m) factor = 1m;

                lista.Add(new LineaPendienteRecepcion {
                    IdDetalleCompra = Convert.ToInt64(lector["id_detalle_compra_producto"], CultureInfo.InvariantCulture),
                    IdProducto = Convert.ToInt64(lector["id_producto"], CultureInfo.InvariantCulture),
                    IdPresentacion = Convert.ToInt64(lector["id_presentacion"], CultureInfo.InvariantCulture),
                    NombreProducto = lector["nombre_producto"].ToString() ?? string.Empty,
                    UnidadMedida = lector["unidad"].ToString() ?? "-",
                    Factor = factor,
                    Pedido = ConvertirDecimal(lector["cantidad"]),
                    Recibido = ConvertirDecimal(lector["cantidad_recibida"]),
                    Devuelto = ConvertirDecimal(lector["cantidad_devuelta"]),
                    CostoBasePorUnidadBase = CostoEfectivoPorUnidadBase(
                        ConvertirDecimal(lector["costo_unitario_base"]), factor,
                        ConvertirDecimal(lector["descuento_item"]), ConvertirDecimal(lector["impuesto_adicional_item"])),
                    ExistenciaActual = ConvertirDecimal(lector["existencia"]),
                    CostoPromedioActual = ConvertirDecimal(lector["costo_promedio"])
                });
            }

            return lista;
        }

        /// <summary>
        /// Registra una recepción (total o parcial). Lanza InvalidOperationException con un mensaje legible si no se puede
        /// (compra anulada, ya recibida, cantidad mayor a la pendiente, cantidad base con más de 2 decimales...).
        /// </summary>
        public ResultadoRecepcion RegistrarRecepcion(long idCompra, IReadOnlyList<LineaRecepcion> lineas, long idUsuario, string? observaciones) {
            var aRecibir = lineas.Where(l => l.Cantidad > 0m).ToList();

            if (aRecibir.Count == 0)
                throw new InvalidOperationException("Indique la cantidad recibida de al menos un producto.");

            using var conexion = ContextoBaseDatos.ObtenerConexion();
            using var tx = conexion.BeginTransaction(IsolationLevel.ReadCommitted);

            try {
                // 1) Bloquear la compra: dos PCs no pueden recibir lo mismo a la vez.
                long idAlmacen, idProveedor;
                string codigo, estadoCompra, estadoRecepcion;

                using (var cmd = Cmd(conexion, tx, """
                    SELECT codigo, id_almacen_destino, id_proveedor, estado_compra, estado_recepcion
                    FROM adv__compra WHERE id_compra = @id FOR UPDATE;
                    """, ("@id", idCompra))) {
                    using var r = cmd.ExecuteReader();
                    if (!r.Read())
                        throw new InvalidOperationException("La compra no existe.");

                    codigo = r["codigo"].ToString() ?? string.Empty;
                    idAlmacen = Convert.ToInt64(r["id_almacen_destino"], CultureInfo.InvariantCulture);
                    idProveedor = Convert.ToInt64(r["id_proveedor"], CultureInfo.InvariantCulture);
                    estadoCompra = r["estado_compra"].ToString() ?? string.Empty;
                    estadoRecepcion = r["estado_recepcion"].ToString() ?? string.Empty;
                }

                if (estadoCompra == "Anulada")
                    throw new InvalidOperationException("La compra está anulada: no se puede recibir.");
                if (estadoRecepcion == "Completa")
                    throw new InvalidOperationException("La compra ya fue recibida por completo.");
                if (idAlmacen <= 0)
                    throw new InvalidOperationException("La compra no tiene almacén destino.");

                // 2) Detalles bloqueados.
                var detalles = new Dictionary<long, DetalleBloqueado>();
                using (var cmd = Cmd(conexion, tx, """
                    SELECT id_detalle_compra_producto, id_producto, cantidad, cantidad_recibida,
                           id_moneda, tasa_cambio_aplicada, costo_unitario, costo_unitario_base,
                           factor_presentacion, descuento_item, impuesto_adicional_item
                    FROM adv__detalle_compra_producto WHERE id_compra = @id FOR UPDATE;
                    """, ("@id", idCompra))) {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) {
                        var factor = ConvertirDecimal(r["factor_presentacion"], 1m);
                        if (factor <= 0m) factor = 1m;

                        var d = new DetalleBloqueado {
                            Id = Convert.ToInt64(r["id_detalle_compra_producto"], CultureInfo.InvariantCulture),
                            IdProducto = Convert.ToInt64(r["id_producto"], CultureInfo.InvariantCulture),
                            Cantidad = ConvertirDecimal(r["cantidad"]),
                            Recibida = ConvertirDecimal(r["cantidad_recibida"]),
                            IdMoneda = r["id_moneda"] == DBNull.Value ? null : Convert.ToInt64(r["id_moneda"], CultureInfo.InvariantCulture),
                            Tasa = ConvertirDecimal(r["tasa_cambio_aplicada"], 1m),
                            CostoOriginal = ConvertirDecimal(r["costo_unitario"]),
                            Factor = factor
                        };
                        d.CostoBasePorUnidadBase = CostoEfectivoPorUnidadBase(
                            ConvertirDecimal(r["costo_unitario_base"]), factor,
                            ConvertirDecimal(r["descuento_item"]), ConvertirDecimal(r["impuesto_adicional_item"]));
                        detalles[d.Id] = d;
                    }
                }

                // 3) Validar todo ANTES de tocar nada.
                foreach (var l in aRecibir) {
                    if (!detalles.TryGetValue(l.IdDetalleCompra, out var d))
                        throw new InvalidOperationException("Una de las líneas no pertenece a esta compra.");

                    var pendiente = d.Cantidad - d.Recibida;
                    if (l.Cantidad > pendiente + 0.0001m)
                        throw new InvalidOperationException($"No se puede recibir {l.Cantidad:0.##}: solo quedan {pendiente:0.##} pendientes en una de las líneas.");

                    if (d.CostoBasePorUnidadBase <= 0m)
                        throw new InvalidOperationException("Una línea no tiene costo en moneda base. Revise la compra antes de recibir.");

                    var cantidadBase = l.Cantidad * d.Factor;
                    if (cantidadBase != Math.Round(cantidadBase, 2))
                        throw new InvalidOperationException(
                            $"La cantidad recibida equivale a {cantidadBase:0.####} unidades base; el inventario admite hasta 2 decimales. Ajuste la cantidad.");
                }

                // 4) Cabecera de la recepción.
                var idRecepcion = Insertar(conexion, tx, """
                    INSERT INTO adv__recepcion_compra (id_compra, fecha_recepcion, recibido_por, observaciones)
                    VALUES (@id_compra, NOW(), @usuario, @obs);
                    """, ("@id_compra", idCompra), ("@usuario", idUsuario), ("@obs", (object?) observaciones ?? DBNull.Value));

                var resultados = new List<ResultadoLineaRecepcion>();
                var resumen = new StringBuilder();

                foreach (var l in aRecibir) {
                    var d = detalles[l.IdDetalleCompra];
                    var cantidadBase = Math.Round(l.Cantidad * d.Factor, 2);
                    var costo = Math.Round(d.CostoBasePorUnidadBase, 4);

                    // 4a) Inventario actual (bloqueado).
                    long? idInventario = null;
                    decimal cantidadActual = 0m, promedioActual = 0m, valorActual = 0m;

                    using (var cmd = Cmd(conexion, tx, """
                        SELECT id_inventario, cantidad, costo_promedio, valor_total
                        FROM adv__inventario WHERE id_producto = @p AND id_almacen = @a FOR UPDATE;
                        """, ("@p", d.IdProducto), ("@a", idAlmacen))) {
                        using var r = cmd.ExecuteReader();
                        if (r.Read()) {
                            idInventario = Convert.ToInt64(r["id_inventario"], CultureInfo.InvariantCulture);
                            cantidadActual = ConvertirDecimal(r["cantidad"]);
                            promedioActual = ConvertirDecimal(r["costo_promedio"]);
                            valorActual = ConvertirDecimal(r["valor_total"]);
                        }
                    }

                    var cantidadNueva = cantidadActual + cantidadBase;

                    // Costo promedio ponderado. Con existencia <= 0 (ventas sin stock) se parte del costo de esta compra.
                    var promedioNuevo = cantidadActual <= 0m
                        ? costo
                        : Math.Round((valorActual + cantidadBase * costo) / cantidadNueva, 4);
                    var valorNuevo = cantidadActual <= 0m
                        ? Math.Round(cantidadNueva * costo, 4)
                        : valorActual + Math.Round(cantidadBase * costo, 4);

                    // 4b) Movimiento ANTES de tocar inventario: los triggers de auditoría lo correlacionan (ventana de 10 s).
                    Insertar(conexion, tx, """
                        INSERT INTO adv__movimiento (id_producto, costo_unitario, id_almacen_origen, id_almacen_destino,
                            fecha_creacion, estado, fecha_termino, saldo_inicial, cantidad_movida, saldo_final,
                            id_tipo_movimiento, id_cuenta_usuario, notas)
                        VALUES (@p, @costo, 0, @a, NOW(), 'Completado', NOW(), @ini, @cant, @fin, @tipo, @usuario, @notas);
                        """,
                        ("@p", d.IdProducto), ("@costo", Math.Round(costo, 2)), ("@a", idAlmacen),
                        ("@ini", cantidadActual), ("@cant", cantidadBase), ("@fin", cantidadNueva),
                        ("@tipo", IdTipoMovimientoEntradaCompra), ("@usuario", idUsuario),
                        ("@notas", $"Recepción #{idRecepcion} de la compra {codigo}."));

                    // 4c) Inventario.
                    if (idInventario == null) {
                        Ejecutar(conexion, tx, """
                            INSERT INTO adv__inventario (id_producto, id_almacen, cantidad, costo_promedio, valor_total)
                            VALUES (@p, @a, @cant, @prom, @valor);
                            """, ("@p", d.IdProducto), ("@a", idAlmacen), ("@cant", cantidadNueva), ("@prom", promedioNuevo), ("@valor", valorNuevo));
                    } else {
                        Ejecutar(conexion, tx, """
                            UPDATE adv__inventario SET cantidad = @cant, costo_promedio = @prom, valor_total = @valor
                            WHERE id_inventario = @id;
                            """, ("@cant", cantidadNueva), ("@prom", promedioNuevo), ("@valor", valorNuevo), ("@id", idInventario.Value));
                    }

                    // 4d) Detalle de recepción con el costo real aplicado (para devolver al mismo costo).
                    Ejecutar(conexion, tx, """
                        INSERT INTO adv__detalle_recepcion_compra (id_recepcion_compra, id_detalle_compra_producto, cantidad_recibida,
                            costo_unitario_base, tasa_cambio_aplicada)
                        VALUES (@r, @d, @cant, @costo, @tasa);
                        """, ("@r", idRecepcion), ("@d", d.Id), ("@cant", l.Cantidad), ("@costo", costo), ("@tasa", d.Tasa));

                    // 4e) Acumulado en la línea de compra.
                    Ejecutar(conexion, tx, """
                        UPDATE adv__detalle_compra_producto SET cantidad_recibida = cantidad_recibida + @cant
                        WHERE id_detalle_compra_producto = @d;
                        """, ("@cant", l.Cantidad), ("@d", d.Id));

                    // 4f) Historial de costo (solo inserciones).
                    Ejecutar(conexion, tx, """
                        INSERT INTO adv__historial_costo_producto (id_producto, id_almacen, origen, id_compra, id_recepcion_compra,
                            id_proveedor, cantidad, id_moneda, costo_unitario_original, tasa_cambio_aplicada, costo_unitario_base,
                            costo_promedio_resultante, existencia_resultante, id_cuenta_usuario)
                        VALUES (@p, @a, 'Recepcion', @compra, @recepcion, @prov, @cant, @moneda, @orig, @tasa, @costo,
                            @prom, @exist, @usuario);
                        """,
                        ("@p", d.IdProducto), ("@a", idAlmacen), ("@compra", idCompra), ("@recepcion", idRecepcion), ("@prov", idProveedor),
                        ("@cant", cantidadBase), ("@moneda", (object?) d.IdMoneda ?? DBNull.Value),
                        ("@orig", Math.Round(d.CostoOriginal / d.Factor, 4)), ("@tasa", d.Tasa), ("@costo", costo),
                        ("@prom", promedioNuevo), ("@exist", cantidadNueva), ("@usuario", idUsuario));

                    // 4g) Último costo del catálogo (por unidad base, moneda base; la columna admite 2 decimales).
                    Ejecutar(conexion, tx, """
                        UPDATE adv__producto SET costo_adquisicion_unitario = @costo WHERE id_producto = @p;
                        """, ("@costo", Math.Round(costo, 2)), ("@p", d.IdProducto));

                    resultados.Add(new ResultadoLineaRecepcion {
                        IdProducto = d.IdProducto,
                        CantidadBase = cantidadBase,
                        CostoBase = costo,
                        CostoPromedioAnterior = promedioActual,
                        CostoPromedioNuevo = promedioNuevo,
                        ExistenciaNueva = cantidadNueva
                    });

                    resumen.Append($"[det {d.Id}: +{l.Cantidad:0.##} pres. = {cantidadBase:0.##} u base @ {costo:0.0000}; prom {promedioActual:0.0000}→{promedioNuevo:0.0000}] ");
                }

                // 5) Estado de recepción de la compra (Parcial / Completa).
                var pendientes = Escalar<long>(conexion, tx, """
                    SELECT COUNT(*) FROM adv__detalle_compra_producto
                    WHERE id_compra = @id AND cantidad_recibida < cantidad - 0.0001;
                    """, ("@id", idCompra));

                var nuevoEstado = pendientes == 0 ? "Completa" : "Parcial";

                Ejecutar(conexion, tx, "UPDATE adv__compra SET estado_recepcion = @e WHERE id_compra = @id;", ("@e", nuevoEstado), ("@id", idCompra));

                // 6) Bitácora.
                Ejecutar(conexion, tx, """
                    INSERT INTO adv__compra_evento (id_compra, tipo, id_cuenta_usuario, motivo, detalle)
                    VALUES (@id, 'Recepcion', @usuario, @obs, @detalle);
                    """,
                    ("@id", idCompra), ("@usuario", idUsuario), ("@obs", (object?) observaciones ?? DBNull.Value),
                    ("@detalle", $"Recepción #{idRecepcion} ({nuevoEstado}). {resumen}".Trim()));

                tx.Commit();

                return new ResultadoRecepcion { IdRecepcion = idRecepcion, EstadoRecepcion = nuevoEstado, Lineas = resultados };
            } catch {
                try { tx.Rollback(); } catch { /* la conexión ya pudo haberse cerrado */ }
                throw;
            }
        }

        #region AUX

        private sealed class DetalleBloqueado {
            public long Id { get; init; }
            public long IdProducto { get; init; }
            public decimal Cantidad { get; init; }
            public decimal Recibida { get; init; }
            public long? IdMoneda { get; init; }
            public decimal Tasa { get; init; }
            public decimal CostoOriginal { get; init; }
            public decimal Factor { get; init; }
            public decimal CostoBasePorUnidadBase { get; set; }
        }

        /// <summary>Costo por unidad base: costo base por presentación / factor, con descuento e impuesto de la línea (hoy 0).</summary>
        private static decimal CostoEfectivoPorUnidadBase(decimal costoBasePorPresentacion, decimal factor, decimal descuentoPct, decimal impuestoPct) {
            if (factor <= 0m) factor = 1m;
            return costoBasePorPresentacion / factor * (1m - descuentoPct / 100m + impuestoPct / 100m);
        }

        private static decimal ConvertirDecimal(object? valor, decimal porDefecto = 0m) =>
            valor == null || valor == DBNull.Value ? porDefecto : Convert.ToDecimal(valor, CultureInfo.InvariantCulture);

        private static MySqlCommand Cmd(MySqlConnection c, MySqlTransaction tx, string sql, params (string Nombre, object? Valor)[] p) {
            var cmd = new MySqlCommand(sql, c, tx) { CommandTimeout = 120 };
            foreach (var (nombre, valor) in p)
                cmd.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
            return cmd;
        }

        private static void Ejecutar(MySqlConnection c, MySqlTransaction tx, string sql, params (string Nombre, object? Valor)[] p) {
            using var cmd = Cmd(c, tx, sql, p);
            cmd.ExecuteNonQuery();
        }

        private static long Insertar(MySqlConnection c, MySqlTransaction tx, string sql, params (string Nombre, object? Valor)[] p) {
            using var cmd = Cmd(c, tx, sql, p);
            cmd.ExecuteNonQuery();
            return cmd.LastInsertedId;
        }

        private static T Escalar<T>(MySqlConnection c, MySqlTransaction tx, string sql, params (string Nombre, object? Valor)[] p) {
            using var cmd = Cmd(c, tx, sql, p);
            return (T) Convert.ChangeType(cmd.ExecuteScalar()!, typeof(T), CultureInfo.InvariantCulture);
        }

        #endregion
    }
}

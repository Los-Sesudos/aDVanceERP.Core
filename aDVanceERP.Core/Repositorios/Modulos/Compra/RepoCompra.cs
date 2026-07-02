using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Compra;
using aDVanceERP.Core.Modelos.Modulos.Compra.Estadisticas;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Compra {
    public class RepoCompra : RepoEntidadBaseDatos<Modelos.Modulos.Compra.Compra, FiltroBusquedaCompra> {
        public RepoCompra() : base("adv__compra", "id_compra") {
        }

        protected override string GenerarComandoAdicionar(Modelos.Modulos.Compra.Compra entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = $"""
                INSERT INTO adv__compra (
                    codigo,
                    id_proveedor,
                    id_cuenta_usuario,
                    id_almacen_destino,
                    fecha,
                    subtotal,
                    descuento_total,
                    impuesto_total,
                    importe_total,
                    estado_compra,
                    observaciones,
                    activo,
                    id_moneda,
                    tasa_cambio_aplicada
                ) VALUES (
                    @codigo,
                    @id_proveedor,
                    @id_cuenta_usuario,
                    @id_almacen_destino,
                    @fecha,
                    @subtotal,
                    @descuento_total,
                    @impuesto_total,
                    @importe_total,
                    @estado_compra,
                    @observaciones,
                    @activo,
                    @id_moneda,
                    @tasa_cambio_aplicada
                )
                """;

            parametros = new Dictionary<string, object> {
                { "@codigo", entidad.Codigo },
                { "@id_proveedor", entidad.IdProveedor },
                { "@id_cuenta_usuario", entidad.IdCuentaUsuario.HasValue ? entidad.IdCuentaUsuario.Value : DBNull.Value },
                { "@id_almacen_destino", entidad.IdAlmacenDestino },
                { "@fecha", entidad.Fecha.ToString("yyyy-MM-dd HH:mm:ss") },
                { "@subtotal", entidad.Subtotal },
                { "@descuento_total", entidad.DescuentoTotal },
                { "@impuesto_total", entidad.ImpuestoTotal },
                { "@importe_total", entidad.ImporteTotal },
                { "@estado_compra", entidad.EstadoCompra.ToString() },
                { "@observaciones", entidad.Observaciones ?? (object)DBNull.Value },
                { "@activo", entidad.Activo },
                { "@id_moneda", entidad.IdMoneda },
                { "@tasa_cambio_aplicada", entidad.TasaCambioAplicada }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(Modelos.Modulos.Compra.Compra entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = $"""
                UPDATE adv__compra 
                SET 
                    codigo = @codigo,
                    id_proveedor = @id_proveedor,
                    id_cuenta_usuario = @id_cuenta_usuario,
                    id_almacen_destino = @id_almacen_destino,
                    fecha = @fecha,
                    subtotal = @subtotal,
                    descuento_total = @descuento_total,
                    impuesto_total = @impuesto_total,
                    importe_total = @importe_total,
                    estado_compra = @estado_compra,
                    observaciones = @observaciones,
                    activo = @activo,
                    id_moneda = @id_moneda,
                    tasa_cambio_aplicada = @tasa_cambio_aplicada
                WHERE id_compra = @id_compra
                """;

            parametros = new Dictionary<string, object> {
                { "@codigo", entidad.Codigo },
                { "@id_proveedor", entidad.IdProveedor },
                { "@id_cuenta_usuario", entidad.IdCuentaUsuario.HasValue ? entidad.IdCuentaUsuario.Value : DBNull.Value },
                { "@id_almacen_destino", entidad.IdAlmacenDestino },
                { "@fecha", entidad.Fecha.ToString("yyyy-MM-dd HH:mm:ss") },
                { "@subtotal", entidad.Subtotal },
                { "@descuento_total", entidad.DescuentoTotal },
                { "@impuesto_total", entidad.ImpuestoTotal },
                { "@importe_total", entidad.ImporteTotal },
                { "@estado_compra", entidad.EstadoCompra.ToString() },
                { "@observaciones", entidad.Observaciones ?? (object)DBNull.Value },
                { "@activo", entidad.Activo },
                { "@id_moneda", entidad.IdMoneda },
                { "@tasa_cambio_aplicada", entidad.TasaCambioAplicada },
                { "@id_compra", entidad.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            const string consulta = """
                -- Eliminar recepciones y sus detalles primero
                DELETE drc FROM adv__detalle_recepcion_compra drc
                INNER JOIN adv__recepcion_compra rc ON drc.id_recepcion_compra = rc.id_recepcion_compra
                WHERE rc.id_compra = @id_compra;

                DELETE FROM adv__recepcion_compra
                WHERE id_compra = @id_compra;

                -- Eliminar detalles de compra
                DELETE FROM adv__detalle_compra_producto
                WHERE id_compra = @id_compra;

                -- Finalmente eliminar la compra
                DELETE FROM adv__compra
                WHERE id_compra = @id_compra;
                """;

            parametros = new Dictionary<string, object> {
                { "@id_compra", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaCompra filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var fechaDesde = criteriosBusqueda.Length == 3 ? criteriosBusqueda[0] : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = criteriosBusqueda.Length == 3 ? criteriosBusqueda[1] : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var criterio = criteriosBusqueda.Length == 3 ? criteriosBusqueda[2] : criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;
            var consultaComun = $"""
                SELECT c.*, 
                       prov.razon_social as nombre_proveedor,
                       al.nombre as nombre_almacen
                FROM adv__compra c
                LEFT JOIN adv__proveedor prov ON c.id_proveedor = prov.id_proveedor
                LEFT JOIN adv__almacen al ON c.id_almacen_destino = al.id_almacen
                WHERE c.activo = @activo 
                {(criteriosBusqueda.Length == 3 ? "AND c.fecha >= @fecha_desde AND c.fecha <= @fecha_hasta" : string.Empty)} 
                """;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaCompra.Id => $"""
                    {consultaComun}
                    AND c.id_compra = @id_compra
                    """,
                FiltroBusquedaCompra.Codigo => $"""
                    {consultaComun}
                    AND c.codigo = @codigo
                    """,
                FiltroBusquedaCompra.IdProveedor => $"""
                    {consultaComun}
                    AND c.id_proveedor = @id_proveedor
                    """,
                FiltroBusquedaCompra.Estado => $"""
                    {consultaComun}
                    AND c.estado_compra = @estado
                    """,
                _ => consultaComun
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaCompra.Id => new Dictionary<string, object> {
                    { "@id_compra", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) },
                    { "@activo", true }
                },
                FiltroBusquedaCompra.Codigo => new Dictionary<string, object> {
                    { "@codigo", criterio },
                    { "@activo", true }
                },
                FiltroBusquedaCompra.IdProveedor => new Dictionary<string, object> {
                    { "@id_proveedor", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) },
                    { "@activo", true }
                },
                FiltroBusquedaCompra.Estado => new Dictionary<string, object> {
                    { "@estado", criterio },
                    { "@activo", true }
                },
                _ => new Dictionary<string, object>() {
                    { "@activo", true }
                }
            };

            if (criteriosBusqueda.Length == 3) {
                parametros.Add("@fecha_desde", DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00"));
                parametros.Add("@fecha_hasta", DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59"));
            }

            return consulta;
        }

        protected override (Modelos.Modulos.Compra.Compra, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            var compra = new Modelos.Modulos.Compra.Compra {
                Id = Convert.ToInt64(lector["id_compra"]),
                Codigo = Convert.ToString(lector["codigo"]) ?? "N/A",
                IdProveedor = Convert.ToInt64(lector["id_proveedor"]),
                IdCuentaUsuario = lector["id_cuenta_usuario"] != DBNull.Value ? Convert.ToInt64(lector["id_cuenta_usuario"]) : null,
                IdAlmacenDestino = Convert.ToInt64(lector["id_almacen_destino"]),
                Fecha = Convert.ToDateTime(lector["fecha"]),
                Subtotal = Convert.ToDecimal(lector["subtotal"], CultureInfo.InvariantCulture),
                DescuentoTotal = Convert.ToDecimal(lector["descuento_total"], CultureInfo.InvariantCulture),
                ImpuestoTotal = Convert.ToDecimal(lector["impuesto_total"], CultureInfo.InvariantCulture),
                ImporteTotal = Convert.ToDecimal(lector["importe_total"], CultureInfo.InvariantCulture),
                EstadoCompra = Enum.Parse<EstadoCompraEnum>(Convert.ToString(lector["estado_compra"]) ?? "Pendiente"),
                Observaciones = lector["observaciones"] != DBNull.Value ? Convert.ToString(lector["observaciones"]) : null,
                Activo = Convert.ToBoolean(lector["activo"]),
                IdMoneda = lector["id_moneda"] != DBNull.Value ? Convert.ToInt32(lector["id_moneda"]) : 1,
                TasaCambioAplicada = lector["tasa_cambio_aplicada"] != DBNull.Value ? Convert.ToDecimal(lector["tasa_cambio_aplicada"], CultureInfo.InvariantCulture) : 1.0m
            };

            var entidadesExtra = new List<IEntidadBaseDatos>();

            return (compra, entidadesExtra);
        }

        #region SINGLETON

        public static RepoCompra Instancia { get; } = new RepoCompra();

        #endregion

        #region UTILES

        public bool CambiarEstadoCompra(long idCompra, EstadoCompraEnum nuevoEstado) {
            var consulta = $"""
                UPDATE adv__compra
                SET estado_compra = @nuevo_estado
                WHERE id_compra = @id_compra
                """;

            return ContextoBaseDatos.EjecutarComandoNoQuery(consulta,
                new Dictionary<string, object> {
                    { "@id_compra", idCompra },
                    { "@nuevo_estado", nuevoEstado.ToString() }
                }) > 0;
        }

        public bool CancelarCompra(long idCompra, string motivo) {
            var consulta = """
                UPDATE adv__compra
                SET estado_compra = 'Anulada',
                    observaciones = CONCAT(COALESCE(observaciones, ''), ' | Cancelada: ', @motivo)
                WHERE id_compra = @id_compra 
                  AND estado_compra NOT IN ('Completada')
                """;
            var parametros = new Dictionary<string, object> {
                { "@id_compra", idCompra },
                { "@motivo", motivo }
            };

            return ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros) > 0;
        }

        public decimal ObtenerTotalComprasPorPeriodo(DateTime fechaInicio, DateTime fechaFin) {
            var consulta = $"""
                SELECT COALESCE(SUM(importe_total), 0) as total_compras
                FROM adv__compra
                WHERE fecha >= @fecha_inicio
                AND fecha <= @fecha_fin
                AND estado_compra IN ('Completada')
                AND activo = 1;
                """;

            var parametros = new Dictionary<string, object> {
                { "@fecha_inicio", fechaInicio.ToString("yyyy-MM-dd 00:00:00") },
                { "@fecha_fin", fechaFin.ToString("yyyy-MM-dd 23:59:59") }
            };

            return ContextoBaseDatos.EjecutarConsultaEscalar<decimal>(consulta, parametros);
        }

        /// <summary>
        /// Devuelve el monto total de pagos CONFIRMADOS de la compra.
        /// </summary>
        public decimal ObtenerTotalPagado(long idCompra) {
            const string consulta = """
                SELECT COALESCE(SUM(monto_pagado), 0)
                FROM adv__pago
                WHERE id_compra = @id_compra
                  AND estado_pago = 'Confirmado'
                """;

            return ContextoBaseDatos.EjecutarConsultaEscalar<decimal>(consulta,
                new Dictionary<string, object> { { "@id_compra", idCompra } });
        }

        /// <summary>Devuelve el saldo pendiente (importe_total − total pagado confirmado).</summary>
        public decimal ObtenerSaldoPendiente(long idCompra) {
            var compra = ObtenerPorId(idCompra);
            return compra == null ? 0 : compra.ImporteTotal - ObtenerTotalPagado(idCompra);
        }

        /// <summary>
        /// Retorna true cuando total_pagado >= importe_total Y no quedan pagos en estado Pendiente.
        /// </summary>
        public bool CompraEstaPagadaCompletamente(long idCompra) {
            const string consulta = """
                SELECT 
                    CASE 
                        WHEN c.importe_total <= COALESCE(SUM(p.monto_pagado), 0) 
                             AND COUNT(CASE WHEN p.estado_pago = 'Pendiente' THEN 1 END) = 0
                        THEN 1 ELSE 0 
                    END AS esta_pagada
                FROM adv__compra c
                LEFT JOIN adv__pago p ON c.id_compra = p.id_compra 
                    AND p.estado_pago IN ('Confirmado', 'Pendiente')
                WHERE c.id_compra = @id_compra
                GROUP BY c.id_compra, c.importe_total
                """;

            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(consulta,
                new Dictionary<string, object> { { "@id_compra", idCompra } }) == 1;
        }

        public List<CompraPendientePago> ObtenerComprasPendientesDePago() {
            var consulta = $"""
                SELECT 
                    c.id_compra,
                    c.codigo,
                    c.fecha,
                    c.importe_total,
                    COALESCE(SUM(CASE WHEN p.estado_pago = 'Confirmado' THEN p.monto_pagado ELSE 0 END), 0) as total_pagado,
                    c.importe_total - COALESCE(SUM(CASE WHEN p.estado_pago = 'Confirmado' THEN p.monto_pagado ELSE 0 END), 0) as saldo_pendiente,
                    COUNT(CASE WHEN p.estado_pago = 'Pendiente' THEN 1 END) as pagos_pendientes_confirmacion,
                    pr.codigo_proveedor,
                    per.nombre_completo as nombre_proveedor
                FROM adv__compra c
                LEFT JOIN adv__proveedor pr ON c.id_proveedor = pr.id_proveedor
                LEFT JOIN adv__persona per ON pr.id_persona = per.id_persona
                LEFT JOIN adv__pago p ON c.id_compra = p.id_compra 
                    AND p.estado_pago IN ('Confirmado', 'Pendiente')
                WHERE c.estado_compra NOT IN ('Anulada')
                    AND c.activo = 1
                GROUP BY c.id_compra, c.codigo, c.fecha, c.importe_total,
                         pr.codigo_proveedor, per.nombre_completo
            HAVING saldo_pendiente > 0
            ORDER BY c.fecha ASC;
            """;

            return ContextoBaseDatos.EjecutarConsulta(consulta, null, MapearEntidadCompraPendientePago)
                .Select(r => r.entidadBase)
                .ToList();
        }

        private (CompraPendientePago, List<IEntidadBaseDatos>) MapearEntidadCompraPendientePago(MySqlDataReader reader) {
            var compraPendiente = new CompraPendientePago {
                IdCompra = Convert.ToInt64(reader["id_compra"]),
                CodigoCompra = reader["codigo"]?.ToString() ?? string.Empty,
                Fecha = Convert.ToDateTime(reader["fecha"]),
                ImporteTotal = Convert.ToDecimal(reader["importe_total"], CultureInfo.InvariantCulture),
                TotalPagado = Convert.ToDecimal(reader["total_pagado"], CultureInfo.InvariantCulture),
                SaldoPendiente = Convert.ToDecimal(reader["saldo_pendiente"], CultureInfo.InvariantCulture),
                PagosPendientesConfirmacion = Convert.ToInt32(reader["pagos_pendientes_confirmacion"]),
                CodigoProveedor = reader["codigo_proveedor"]?.ToString() ?? string.Empty,
                NombreProveedor = reader["nombre_proveedor"]?.ToString() ?? string.Empty
            };

            return (compraPendiente, new List<IEntidadBaseDatos>());
        }

        #endregion
    }
}
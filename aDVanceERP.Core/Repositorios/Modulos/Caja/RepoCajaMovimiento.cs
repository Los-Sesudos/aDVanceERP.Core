using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Caja;
using aDVanceERP.Core.Modelos.Modulos.Comun;
using aDVanceERP.Core.Repositorios.BD;
using aDVanceERP.Core.Repositorios.Modulos.Monedas;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Caja {

    public class RepoCajaMovimiento : RepoEntidadBaseDatos<CajaMovimiento, FiltroBusquedaCajaMovimiento> {

        public RepoCajaMovimiento() : base("adv__caja_movimiento", "id_movimiento_caja") { }

        protected override string GenerarComandoAdicionar(
                CajaMovimiento entidad,
                out Dictionary<string, object> parametros,
                params IEntidadBaseDatos[] entidadesExtra) {

            var comando = """
                INSERT INTO adv__caja_movimiento (
                    id_turno,
                    tipo,
                    canal_pago,
                    id_moneda,
                    id_venta,
                    monto,
                    monto_moneda_base,
                    tasa_cambio_aplicada,
                    descripcion,
                    id_cuenta_usuario,
                    fecha_movimiento
                ) VALUES (
                    @id_turno,
                    @tipo,
                    @canal_pago,
                    @id_moneda,
                    @id_venta,
                    @monto,
                    @monto_moneda_base,
                    @tasa_cambio_aplicada,
                    @descripcion,
                    @id_cuenta_usuario,
                    @fecha_movimiento
                );
                """;

            // Se congela la tasa del día del movimiento — no la de hoy — para que un
            // reporte histórico no cambie de valor si la tasa se actualiza después.
            var idMonedaBase = RepoMoneda.Instancia.ObtenerMonedaBase().Id;
            decimal tasaAplicada = entidad.IdMoneda == idMonedaBase
                ? 1m
                : RepoTasaCambio.Instancia.ObtenerTasaVigenteEfectivo(entidad.IdMoneda, idMonedaBase, entidad.FechaMovimiento);
            decimal montoBase = entidad.Monto * tasaAplicada;

            parametros = new Dictionary<string, object> {
                { "@id_turno",             entidad.IdTurno },
                { "@tipo",                 entidad.Tipo.ToString() },
                { "@canal_pago",           entidad.CanalPago.ToString() },
                { "@id_moneda",            entidad.IdMoneda },
                { "@id_venta",             entidad.IdVenta.HasValue ? entidad.IdVenta.Value : (object)DBNull.Value },
                { "@monto",                entidad.Monto },
                { "@monto_moneda_base",    montoBase },
                { "@tasa_cambio_aplicada", tasaAplicada },
                { "@descripcion",          entidad.Descripcion ?? (object)DBNull.Value },
                { "@id_cuenta_usuario",    entidad.IdCuentaUsuario },
                { "@fecha_movimiento",     entidad.FechaMovimiento.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            return comando;
        }

        protected override string GenerarComandoEditar(CajaMovimiento entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var comando = """
                UPDATE adv__caja_movimiento
                SET 
                    tipo = @tipo,
                    canal_pago = @canal_pago,
                    monto = @monto,
                    id_moneda = @id_moneda,
                    descripcion = @descripcion
                WHERE id_movimiento_caja = @id_movimiento_caja;
                """;

            // Se congela la tasa del día del movimiento — no la de hoy — para que un
            // reporte histórico no cambie de valor si la tasa se actualiza después.
            var idMonedaBase = RepoMoneda.Instancia.ObtenerMonedaBase().Id;
            decimal tasaAplicada = entidad.IdMoneda == idMonedaBase
                ? 1m
                : RepoTasaCambio.Instancia
                    .ObtenerTasaVigenteEfectivo(
                        entidad.IdMoneda, 
                        idMonedaBase, 
                        entidad.FechaMovimiento
                    );
            decimal montoBase = entidad.Monto * tasaAplicada;

            parametros = new Dictionary<string, object> {
                { "@id_movimiento_caja",   entidad.Id },
                { "@tipo",                 entidad.Tipo.ToString() },
                { "@monto",                entidad.Monto },
                { "@canal_pago",           entidad.CanalPago.ToString() },
                { "@id_moneda",            entidad.IdMoneda },
                { "@monto_moneda_base",    montoBase },
                { "@tasa_cambio_aplicada", tasaAplicada },
                { "@descripcion",          entidad.Descripcion ?? (object)DBNull.Value }
            };

            return comando;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            throw new InvalidOperationException(
                "Los movimientos de caja son inmutables. " +
                "Para revertir un movimiento incorrecto, registre uno de tipo AjusteArqueo.");
        }

        protected override string GenerarComandoObtener(FiltroBusquedaCajaMovimiento filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var fechaDesde = criteriosBusqueda.Length == 3
                ? criteriosBusqueda[0]
                : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = criteriosBusqueda.Length == 3
                ? criteriosBusqueda[1]
                : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var criterio = criteriosBusqueda.Length >= 1 ? criteriosBusqueda[^1] : string.Empty;

            var consultaBase = $"""
                SELECT
                    m.*,
                    v.numero_factura_ticket AS numero_factura,
                    cu.nombre               AS nombre_usuario
                FROM adv__caja_movimiento m
                LEFT JOIN adv__venta         v  ON v.id_venta          = m.id_venta
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario = m.id_cuenta_usuario
                {(criteriosBusqueda.Length == 3
                    ? "WHERE m.fecha_movimiento >= @fecha_desde AND m.fecha_movimiento <= @fecha_hasta"
                    : "WHERE 1=1")}
                """;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaCajaMovimiento.IdTurno => $"""
                    {consultaBase}
                    AND m.id_turno = @id_turno
                    ORDER BY m.fecha_movimiento ASC
                    """,
                FiltroBusquedaCajaMovimiento.Tipo => $"""
                    {consultaBase}
                    AND m.tipo = @tipo
                    ORDER BY m.fecha_movimiento DESC
                    """,
                FiltroBusquedaCajaMovimiento.Canal => $"""
                    {consultaBase}
                    AND m.canal_pago = @canal_pago
                    ORDER BY m.fecha_movimiento DESC
                    """,
                _ => $"{consultaBase} ORDER BY m.fecha_movimiento DESC"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaCajaMovimiento.IdTurno => new Dictionary<string, object> {
                    { "@id_turno",    Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) },
                    { "@fecha_desde", DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta", DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                },
                FiltroBusquedaCajaMovimiento.Tipo => new Dictionary<string, object> {
                    { "@tipo",        criterio },
                    { "@fecha_desde", DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta", DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                },
                FiltroBusquedaCajaMovimiento.Canal => new Dictionary<string, object> {
                    { "@canal_pago",  criterio },
                    { "@fecha_desde", DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta", DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                },
                _ => new Dictionary<string, object> {
                    { "@fecha_desde", DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta", DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                }
            };

            return consulta;
        }

        protected override (CajaMovimiento, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            var movimiento = new CajaMovimiento {
                Id = Convert.ToInt64(lector["id_movimiento_caja"]),
                IdTurno = Convert.ToInt64(lector["id_turno"]),
                Tipo = Enum.Parse<TipoMovimientoCajaEnum>(Convert.ToString(lector["tipo"]) ?? "EntradaManual"),
                CanalPago = Enum.Parse<CanalPagoEnum>(Convert.ToString(lector["canal_pago"]) ?? "NA"),
                IdMoneda = Convert.ToInt64(lector["id_moneda"]),
                IdVenta = lector["id_venta"] != DBNull.Value ? Convert.ToInt64(lector["id_venta"]) : null,
                Monto = Convert.ToDecimal(lector["monto"], CultureInfo.InvariantCulture),
                MontoMonedaBase = Convert.ToDecimal(lector["monto_moneda_base"], CultureInfo.InvariantCulture),
                TasaCambioAplicada = Convert.ToDecimal(lector["tasa_cambio_aplicada"], CultureInfo.InvariantCulture),
                Descripcion = lector["descripcion"] != DBNull.Value ? Convert.ToString(lector["descripcion"]) : null,
                IdCuentaUsuario = Convert.ToInt64(lector["id_cuenta_usuario"]),
                FechaMovimiento = Convert.ToDateTime(lector["fecha_movimiento"])
            };

            if (lector.VisibleFieldCount > 8) {
                movimiento.NumeroFactura = lector["numero_factura"] != DBNull.Value ? Convert.ToString(lector["numero_factura"]) : null;
                movimiento.NombreUsuario = lector["nombre_usuario"] != DBNull.Value ? Convert.ToString(lector["nombre_usuario"]) : null;
            }

            return (movimiento, new List<IEntidadBaseDatos>());
        }

        public static RepoCajaMovimiento Instancia { get; } = new RepoCajaMovimiento();

        [Obsolete("Usar ObtenerTotalesPorCanalYMoneda para soporte multimoneda.")]
        public TotalesCierreCaja ObtenerTotalesPorCanal(long idTurno) =>
            ObtenerTotalesPorCanalYMoneda(idTurno).FirstOrDefault(t => t.IdMoneda == RepoMoneda.Instancia.ObtenerMonedaBase().Id)
            ?? new TotalesCierreCaja { IdTurno = idTurno };

        /// <summary>
        /// Totales por canal y moneda de un turno. TotalEfectivoBase/TotalTransferenciasBase
        /// ahora vienen de SUM(monto_moneda_base) — la tasa que quedó CONGELADA al momento
        /// de cada movimiento — en vez de recalcularse con la tasa de hoy. Así el cierre de
        /// un turno de hace tiempo no cambia si la tasa se actualizó después.
        /// </summary>
        public List<TotalesCierreCaja> ObtenerTotalesPorCanalYMoneda(long idTurno) {
            var consulta = """
                SELECT id_moneda, canal_pago, SUM(monto) AS total, SUM(monto_moneda_base) AS total_base
                FROM adv__caja_movimiento
                WHERE id_turno = @id_turno
                  AND tipo IN ('Venta','DevolucionVenta','EntradaManual','SalidaManual')
                GROUP BY id_moneda, canal_pago;
                """;

            var parametros = new Dictionary<string, object> { { "@id_turno", idTurno } };

            var resultados = ContextoBaseDatos.EjecutarConsulta(consulta, parametros, (lector) => {
                var idMoneda = Convert.ToInt64(lector["id_moneda"]);
                var canal = Enum.Parse<CanalPagoEnum>(Convert.ToString(lector["canal_pago"]) ?? "NA");
                var total = Convert.ToDecimal(lector["total"], CultureInfo.InvariantCulture);
                var totalBase = Convert.ToDecimal(lector["total_base"], CultureInfo.InvariantCulture);
                return (new { idMoneda, canal, total, totalBase }, new List<IEntidadBaseDatos>());
            });

            var totalesPorMoneda = new Dictionary<long, TotalesCierreCaja>();

            foreach (var (fila, _) in resultados) {
                if (!totalesPorMoneda.TryGetValue(fila.idMoneda, out var totales)) {
                    totales = new TotalesCierreCaja { IdTurno = idTurno, IdMoneda = fila.idMoneda };
                    totalesPorMoneda[fila.idMoneda] = totales;
                }

                switch (fila.canal) {
                    case CanalPagoEnum.Efectivo:
                        totales.TotalEfectivo += fila.total;
                        totales.TotalEfectivoBase += fila.totalBase;
                        break;
                    case CanalPagoEnum.TransferenciaBancaria:
                        totales.TotalTransferencias += fila.total;
                        totales.TotalTransferenciasBase += fila.totalBase;
                        break;
                    case CanalPagoEnum.Mixto:
                        totales.TotalEfectivo += fila.total;
                        totales.TotalEfectivoBase += fila.totalBase;
                        break;
                }
            }

            var idMonedaBase = RepoMoneda.Instancia.ObtenerMonedaBase().Id;
            if (!totalesPorMoneda.ContainsKey(idMonedaBase))
                totalesPorMoneda[idMonedaBase] = new TotalesCierreCaja { IdTurno = idTurno, IdMoneda = idMonedaBase };

            var montoApertura = ContextoBaseDatos.EjecutarConsultaEscalar<decimal>(
                "SELECT monto_apertura FROM adv__caja_turno WHERE id_turno = @id_turno LIMIT 1;", parametros);
            totalesPorMoneda[idMonedaBase].TotalEfectivo += montoApertura;
            totalesPorMoneda[idMonedaBase].TotalEfectivoBase += montoApertura; // apertura ya está en base

            return [.. totalesPorMoneda.Values];
        }

        /// <summary>
        /// Totales netos por tipo y moneda en un rango de fechas, incluyendo el equivalente
        /// ya congelado en moneda base (SUM(monto_moneda_base)) — no se recalcula con la
        /// tasa de hoy, así el reporte de un período pasado no cambia si la tasa se actualiza.
        /// </summary>
        public List<TotalMovimientoPorTipo> ObtenerTotalesPorTipoYMonedaEnRango(DateTime fechaDesde, DateTime fechaHasta) {
            var consulta = """
                SELECT id_moneda, tipo, SUM(monto) AS total, SUM(monto_moneda_base) AS total_base
                FROM adv__caja_movimiento
                WHERE fecha_movimiento >= @fecha_desde
                  AND fecha_movimiento <= @fecha_hasta
                GROUP BY id_moneda, tipo;
                """;

            var parametros = new Dictionary<string, object> {
                { "@fecha_desde", fechaDesde.ToString("yyyy-MM-dd 00:00:00") },
                { "@fecha_hasta", fechaHasta.ToString("yyyy-MM-dd 23:59:59") }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, (lector) => {
                var item = new TotalMovimientoPorTipo {
                    IdMoneda = Convert.ToInt64(lector["id_moneda"]),
                    Tipo = Enum.Parse<TipoMovimientoCajaEnum>(Convert.ToString(lector["tipo"]) ?? "EntradaManual"),
                    Total = Convert.ToDecimal(lector["total"], CultureInfo.InvariantCulture),
                    TotalBase = Convert.ToDecimal(lector["total_base"], CultureInfo.InvariantCulture)
                };
                return (item, new List<IEntidadBaseDatos>());
            }).Select(r => r.entidadBase).ToList();
        }

        public List<CajaMovimiento> ObtenerMovimientosTurno(long idTurno) {
            var consulta = """
                SELECT
                    m.*,
                    v.numero_factura_ticket AS numero_factura,
                    cu.nombre               AS nombre_usuario
                FROM adv__caja_movimiento m
                LEFT JOIN adv__venta          v  ON v.id_venta           = m.id_venta
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario  = m.id_cuenta_usuario
                WHERE m.id_turno = @id_turno
                ORDER BY m.fecha_movimiento ASC;
                """;

            var parametros = new Dictionary<string, object> { { "@id_turno", idTurno } };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public List<CajaMovimiento> ObtenerMovimientosEnRango(DateTime fechaDesde, DateTime fechaHasta) {
            var consulta = """
                SELECT
                    m.*,
                    v.numero_factura_ticket AS numero_factura,
                    cu.nombre               AS nombre_usuario
                FROM adv__caja_movimiento m
                LEFT JOIN adv__venta          v  ON v.id_venta           = m.id_venta
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario  = m.id_cuenta_usuario
                WHERE m.fecha_movimiento >= @fecha_desde
                  AND m.fecha_movimiento <= @fecha_hasta
                ORDER BY m.fecha_movimiento ASC;
                """;

            var parametros = new Dictionary<string, object> {
                { "@fecha_desde", fechaDesde.ToString("yyyy-MM-dd 00:00:00") },
                { "@fecha_hasta", fechaHasta.ToString("yyyy-MM-dd 23:59:59") }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public bool TurnoTieneMovimientos(long idTurno) {
            var consulta = """
                SELECT COUNT(*)
                FROM adv__caja_movimiento
                WHERE id_turno = @id_turno
                LIMIT 1;
                """;

            var parametros = new Dictionary<string, object> { { "@id_turno", idTurno } };

            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(consulta, parametros) > 0;
        }
    }
}
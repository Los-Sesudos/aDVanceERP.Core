using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Caja;
using aDVanceERP.Core.Repositorios.BD;
using aDVanceERP.Core.Repositorios.Modulos.Monedas;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Caja {

    public class RepoCajaTurno : RepoEntidadBaseDatos<CajaTurno, FiltroBusquedaCajaTurno> {

        public RepoCajaTurno() : base("adv__caja_turno", "id_turno") { }

        // ── CRUD base ─────────────────────────────────────────────

        protected override string GenerarComandoAdicionar(
                CajaTurno entidad,
                out Dictionary<string, object> parametros,
                params IEntidadBaseDatos[] entidadesExtra) {

            var comando = """
                INSERT INTO adv__caja_turno (
                    codigo,
                    id_almacen,
                    id_cuenta_apertura,
                    fecha_apertura,
                    monto_apertura,
                    estado,
                    observaciones_apertura
                ) VALUES (
                    @codigo,
                    @id_almacen,
                    @id_cuenta_apertura,
                    @fecha_apertura,
                    @monto_apertura,
                    @estado,
                    @observaciones_apertura
                );
                """;

            parametros = new Dictionary<string, object> {
                { "@codigo",                    entidad.Codigo },
                { "@id_almacen",                entidad.IdAlmacen },
                { "@id_cuenta_apertura",        entidad.IdCuentaApertura },
                { "@fecha_apertura",            entidad.FechaApertura.ToString("yyyy-MM-dd HH:mm:ss") },
                { "@monto_apertura",            entidad.MontoApertura },
                { "@estado",                    entidad.Estado.ToString() },
                { "@observaciones_apertura",    entidad.ObservacionesApertura ?? (object)DBNull.Value }
            };

            return comando;
        }

        protected override string GenerarComandoEditar(
                CajaTurno entidad,
                out Dictionary<string, object> parametros,
                params IEntidadBaseDatos[] entidadesExtra) {

            // Edición general (usada solo en casos excepcionales).
            // Para el cierre usa CerrarTurno(); para anulación usa AnularTurno().
            var comando = """
                UPDATE adv__caja_turno
                SET
                    estado                          = @estado,
                    id_cuenta_cierre                = @id_cuenta_cierre,
                    fecha_cierre                    = @fecha_cierre,
                    monto_efectivo_calculado        = @monto_efectivo_calculado,
                    monto_efectivo_declarado        = @monto_efectivo_declarado,
                    monto_transferencias_calculado  = @monto_transferencias_calculado,
                    monto_transferencias_declarado  = @monto_transferencias_declarado,
                    observaciones_cierre            = @observaciones_cierre
                WHERE id_turno = @id_turno
                """;

            parametros = new Dictionary<string, object> {
                { "@id_turno",                        entidad.Id },
                { "@estado",                          entidad.Estado.ToString() },
                { "@id_cuenta_cierre",                entidad.IdCuentaCierre.HasValue ? entidad.IdCuentaCierre.Value : (object)DBNull.Value },
                { "@fecha_cierre",                    entidad.FechaCierre.HasValue ? entidad.FechaCierre.Value.ToString("yyyy-MM-dd HH:mm:ss") : (object)DBNull.Value },
                { "@monto_efectivo_calculado",        entidad.MontoEfectivoCalculado       ?? (object)DBNull.Value },
                { "@monto_efectivo_declarado",        entidad.MontoEfectivoDeclarado       ?? (object)DBNull.Value },
                { "@monto_transferencias_calculado",  entidad.MontoTransferenciasCalculado ?? (object)DBNull.Value },
                { "@monto_transferencias_declarado",  entidad.MontoTransferenciasDeclarado ?? (object)DBNull.Value },
                { "@observaciones_cierre",            entidad.ObservacionesCierre          ?? (object)DBNull.Value }
            };

            return comando;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            // Los turnos no se eliminan físicamente — usar AnularTurno().
            // Este método existe por contrato de la clase base.
            var comando = """
                UPDATE adv__caja_turno
                SET estado = 'Anulado'
                WHERE id_turno = @id_turno
                """;

            parametros = new Dictionary<string, object> {
                { "@id_turno", id }
            };

            return comando;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaCajaTurno filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var idAlmacen = criteriosBusqueda.Length == 4
                ? long.Parse(criteriosBusqueda[0])
                : 0L;
            var fechaDesde = criteriosBusqueda.Length == 4
                ? criteriosBusqueda[1]
                : DateTime.Today.ToString("yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture);
            var fechaHasta = criteriosBusqueda.Length == 4
                ? criteriosBusqueda[2]
                : DateTime.Today.ToString("yyyy-MM-dd 23:59:59", CultureInfo.InvariantCulture);

            var criterio = criteriosBusqueda.Length >= 1 ? criteriosBusqueda[^1] : string.Empty;

            var consultaBase = $"""
                SELECT
                    t.*,
                    a.nombre        AS nombre_almacen,
                    cu.nombre       AS nombre_usuario_apertura
                FROM adv__caja_turno t
                LEFT JOIN adv__almacen       a  ON a.id_almacen           = t.id_almacen
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario  = t.id_cuenta_apertura
                {(criteriosBusqueda.Length == 4
                    ? "WHERE t.fecha_apertura >= @fecha_desde AND t.fecha_apertura <= @fecha_hasta AND t.id_almacen = @id_almacen"
                    : "WHERE 1=1")}
                """;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaCajaTurno.Id => $"""
                    {consultaBase}
                    AND t.id_turno = @id_turno
                    """,
                FiltroBusquedaCajaTurno.Codigo => $"""
                    {consultaBase}
                    AND t.codigo = @codigo
                    """,
                FiltroBusquedaCajaTurno.Estado => $"""
                    {consultaBase}
                    AND t.estado = @estado
                    """,
                _ => $"{consultaBase} ORDER BY t.fecha_apertura DESC"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaCajaTurno.Id => new Dictionary<string, object> {
                    { "@id_almacen",   idAlmacen },
                    { "@id_turno",     Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) },
                    { "@fecha_desde",  DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta",  DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                },
                FiltroBusquedaCajaTurno.Codigo => new Dictionary<string, object> {
                    { "@id_almacen",   idAlmacen },
                    { "@codigo",       criterio },
                    { "@fecha_desde",  DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta",  DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                },
                FiltroBusquedaCajaTurno.Estado => new Dictionary<string, object> {
                    { "@id_almacen",   idAlmacen },
                    { "@estado",       criterio },
                    { "@fecha_desde",  DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta",  DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                },
                _ => new Dictionary<string, object> {
                    { "@id_almacen",   idAlmacen },
                    { "@fecha_desde",  DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00") },
                    { "@fecha_hasta",  DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59") }
                }
            };

            return consulta;
        }

        protected override (CajaTurno, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            var turno = new CajaTurno {
                Id = Convert.ToInt64(lector["id_turno"]),
                Codigo = Convert.ToString(lector["codigo"]) ?? string.Empty,
                IdAlmacen = Convert.ToInt64(lector["id_almacen"]),
                IdCuentaApertura = Convert.ToInt64(lector["id_cuenta_apertura"]),
                IdCuentaCierre = lector["id_cuenta_cierre"] != DBNull.Value ? Convert.ToInt64(lector["id_cuenta_cierre"]) : null,
                FechaApertura = Convert.ToDateTime(lector["fecha_apertura"]),
                FechaCierre = lector["fecha_cierre"] != DBNull.Value ? Convert.ToDateTime(lector["fecha_cierre"]) : null,
                MontoApertura = Convert.ToDecimal(lector["monto_apertura"], CultureInfo.InvariantCulture),
                MontoEfectivoCalculado = lector["monto_efectivo_calculado"] != DBNull.Value ? Convert.ToDecimal(lector["monto_efectivo_calculado"], CultureInfo.InvariantCulture) : null,
                MontoEfectivoDeclarado = lector["monto_efectivo_declarado"] != DBNull.Value ? Convert.ToDecimal(lector["monto_efectivo_declarado"], CultureInfo.InvariantCulture) : null,
                DiferenciaEfectivo = lector["diferencia_efectivo"] != DBNull.Value ? Convert.ToDecimal(lector["diferencia_efectivo"], CultureInfo.InvariantCulture) : null,
                MontoTransferenciasCalculado = lector["monto_transferencias_calculado"] != DBNull.Value ? Convert.ToDecimal(lector["monto_transferencias_calculado"], CultureInfo.InvariantCulture) : null,
                MontoTransferenciasDeclarado = lector["monto_transferencias_declarado"] != DBNull.Value ? Convert.ToDecimal(lector["monto_transferencias_declarado"], CultureInfo.InvariantCulture) : null,
                DiferenciaTransferencias = lector["diferencia_transferencias"] != DBNull.Value ? Convert.ToDecimal(lector["diferencia_transferencias"], CultureInfo.InvariantCulture) : null,
                Estado = Enum.Parse<EstadoCajaTurnoEnum>(Convert.ToString(lector["estado"]) ?? "Abierto"),
                ObservacionesApertura = lector["observaciones_apertura"] != DBNull.Value ? Convert.ToString(lector["observaciones_apertura"]) : null,
                ObservacionesCierre = lector["observaciones_cierre"] != DBNull.Value ? Convert.ToString(lector["observaciones_cierre"]) : null,
            };

            // Datos auxiliares de JOIN (solo presentes en consultas de listado)
            if (lector.VisibleFieldCount > 18) {
                turno.NombreAlmacen = lector["nombre_almacen"] != DBNull.Value ? Convert.ToString(lector["nombre_almacen"]) : null;
                turno.NombreUsuarioApertura = lector["nombre_usuario_apertura"] != DBNull.Value ? Convert.ToString(lector["nombre_usuario_apertura"]) : null;
            }

            return (turno, new List<IEntidadBaseDatos>());
        }

        // ── Singleton ─────────────────────────────────────────────

        public static RepoCajaTurno Instancia { get; } = new RepoCajaTurno();

        // ── Operaciones específicas del dominio de caja ───────────

        /// <summary>
        /// Conteo monotónico para numerar turnos — incluye anulados para nunca 
        /// reutilizar un número ya emitido.
        /// </summary>
        public int ObtenerCorrelativoTurnoHoy()
            => ContextoBaseDatos.EjecutarConsultaEscalar<int>("""
                SELECT COUNT(*)
                FROM adv__caja_turno
                WHERE DATE(fecha_apertura) = CURDATE()
                  AND activo = 1;
                """);

        /// <summary>
        /// Verifica si existe un turno con estado Abierto para el almacén indicado.
        /// El presentador llama a esto ANTES de abrir un nuevo turno (regla: 1 turno activo por almacén).
        /// </summary>
        public bool ExisteTurnoAbierto(long idAlmacen) {
            var consulta = """
                SELECT COUNT(*)
                FROM adv__caja_turno
                WHERE id_almacen = @id_almacen
                  AND estado = 'Abierto'
                LIMIT 1;
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_almacen", idAlmacen }
            };

            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(consulta, parametros) > 0;
        }

        /// <summary>
        /// Devuelve el turno actualmente abierto para un almacén, o null si no hay ninguno.
        /// </summary>
        public CajaTurno? ObtenerTurnoAbierto(long idAlmacen) {
            var consulta = """
                SELECT
                    t.*,
                    a.nombre         AS nombre_almacen,
                    cu.nombre        AS nombre_usuario_apertura
                FROM adv__caja_turno t
                LEFT JOIN adv__almacen        a  ON a.id_almacen          = t.id_almacen
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario  = t.id_cuenta_apertura
                WHERE t.id_almacen = @id_almacen
                  AND t.estado = 'Abierto'
                LIMIT 1;
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_almacen", idAlmacen }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .FirstOrDefault()
                .entidadBase;
        }

        /// <summary>
        /// Genera el código único del turno con formato TRN-YYYYMMDD-XXXX.
        /// Obtiene el número correlativo desde la BD para evitar colisiones.
        /// </summary>
        public string GenerarCodigoTurno(long idAlmacen) {
            var fechaHoy = DateTime.Today.ToString("yyyyMMdd");

            var consulta = """
                SELECT COUNT(*) + 1
                FROM adv__caja_turno
                WHERE DATE(fecha_apertura) = CURDATE()
                  AND id_almacen = @id_almacen;
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_almacen", idAlmacen }
            };

            var correlativo = ContextoBaseDatos.EjecutarConsultaEscalar<int>(consulta, parametros);

            return $"TRN-{fechaHoy}-{idAlmacen:D2}{correlativo:D2}";
        }

        /// <summary>
        /// Escribe los montos calculados y declarados, la fecha de cierre y cambia el estado a Cerrado.
        /// El presentador debe haber calculado los totales desde RepoCajaMovimiento antes de llamar aquí.
        /// </summary>
        public bool CerrarTurno(
            long idTurno,
            long idCuentaCierre,
            decimal montoEfectivoCalculado,
            decimal montoEfectivoDeclarado,
            decimal montoTransferenciasCalculado,
            decimal montoTransferenciasDeclarado,
            string? observacionesCierre) {

            var consulta = """
                UPDATE adv__caja_turno
                SET
                    estado                          = 'Cerrado',
                    id_cuenta_cierre                = @id_cuenta_cierre,
                    fecha_cierre                    = @fecha_cierre,
                    monto_efectivo_calculado        = @monto_efectivo_calculado,
                    monto_efectivo_declarado        = @monto_efectivo_declarado,
                    monto_transferencias_calculado  = @monto_transferencias_calculado,
                    monto_transferencias_declarado  = @monto_transferencias_declarado,
                    observaciones_cierre            = @observaciones_cierre
                WHERE id_turno = @id_turno
                  AND estado   = 'Abierto';
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_turno",                        idTurno },
                { "@id_cuenta_cierre",                idCuentaCierre },
                { "@fecha_cierre",                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "@monto_efectivo_calculado",        montoEfectivoCalculado },
                { "@monto_efectivo_declarado",        montoEfectivoDeclarado },
                { "@monto_transferencias_calculado",  montoTransferenciasCalculado },
                { "@monto_transferencias_declarado",  montoTransferenciasDeclarado },
                { "@observaciones_cierre",            observacionesCierre ?? (object)DBNull.Value }
            };

            // Devuelve true solo si afectó exactamente 1 fila
            // (garantía de que el turno existía y estaba Abierto)
            return ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros) == 1;
        }

        /// <summary>
        /// Anula un turno abierto. Solo se permite si el presentador valida que no tiene movimientos.
        /// </summary>
        public bool AnularTurno(long idTurno, string? motivo) {
            var consulta = """
                UPDATE adv__caja_turno
                SET
                    estado              = 'Anulado',
                    observaciones_cierre = @motivo
                WHERE id_turno = @id_turno
                  AND estado   = 'Abierto';
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_turno", idTurno },
                { "@motivo",   motivo ?? (object)DBNull.Value }
            };

            return ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros) == 1;
        }

        /// <summary>
        /// Resumen del arqueo de efectivo de un turno, UNA SOLA moneda (compatibilidad).
        /// Para turnos multimoneda usar <see cref="ObtenerResumenArqueoPorMoneda"/>.
        /// </summary>
        [Obsolete("Usar ObtenerResumenArqueoPorMoneda para soporte multimoneda.")]
        public ResumenArqueo ObtenerResumenArqueo(long idTurno) =>
            ObtenerResumenArqueoPorMoneda(idTurno).FirstOrDefault()
                ?? new ResumenArqueo { IdTurno = idTurno };

        /// <summary>
        /// Resumen del arqueo de efectivo de un turno, desglosado por moneda.
        /// Cada ResumenArqueo trae su propio TotalContado (en su moneda) y, para
        /// las monedas que no son la base, la tasa de efectivo vigente aplicada
        /// (ver <see cref="RepoTasaCambio.ObtenerTasaVigenteEfectivo"/>) más
        /// TotalContadoBase ya convertido.
        /// </summary>
        public List<ResumenArqueo> ObtenerResumenArqueoPorMoneda(long idTurno) {
            var consulta = """
                SELECT
                    ca.id_arqueo,
                    ca.id_turno,
                    ca.id_moneda,
                    m.codigo AS codigo_moneda,
                    ca.denominacion,
                    ca.cantidad,
                    ca.subtotal
                FROM adv__caja_arqueo ca
                INNER JOIN adv__moneda m ON ca.id_moneda = m.id_moneda
                WHERE ca.id_turno = @id_turno
                ORDER BY ca.id_moneda, ca.denominacion DESC;
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_turno", idTurno }
            };

            var filas = ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearArqueoConMoneda)
                .Select(r => r.entidadBase)
                .ToList();

            var idMonedaBase = RepoMoneda.Instancia.ObtenerMonedaBase().Id;

            return filas
                .GroupBy(f => f.arqueo.IdMoneda)
                .Select(g => {
                    var idMoneda = g.Key;
                    var tasa = idMoneda == idMonedaBase
                        ? 1m
                        : RepoTasaCambio.Instancia.ObtenerTasaVigenteEfectivo(idMoneda, idMonedaBase);

                    return new ResumenArqueo {
                        IdTurno = idTurno,
                        IdMoneda = idMoneda,
                        CodigoMoneda = g.First().codigoMoneda,
                        Denominaciones = g.Select(x => x.arqueo).ToList(),
                        TotalContado = g.Sum(x => x.arqueo.Subtotal),
                        TasaCambioAplicada = tasa
                    };
                })
                .OrderByDescending(r => r.IdMoneda == idMonedaBase) // moneda base primero
                .ThenBy(r => r.CodigoMoneda)
                .ToList();
        }

        private ((CajaArqueo arqueo, string codigoMoneda) entidadBase, List<IEntidadBaseDatos> entidadesExtra) MapearArqueoConMoneda(MySqlDataReader lector) {
            var arqueo = new CajaArqueo {
                Id = Convert.ToInt64(lector["id_arqueo"]),
                IdTurno = Convert.ToInt64(lector["id_turno"]),
                IdMoneda = Convert.ToInt64(lector["id_moneda"]),
                Denominacion = Convert.ToDecimal(lector["denominacion"], CultureInfo.InvariantCulture),
                Cantidad = Convert.ToInt32(lector["cantidad"]),
                Subtotal = Convert.ToDecimal(lector["subtotal"], CultureInfo.InvariantCulture)
            };

            var codigoMoneda = Convert.ToString(lector["codigo_moneda"]) ?? string.Empty;

            return ((arqueo, codigoMoneda), new List<IEntidadBaseDatos>());
        }
    }
}
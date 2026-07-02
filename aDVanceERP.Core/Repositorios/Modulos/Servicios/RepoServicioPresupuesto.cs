using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioPresupuesto : RepoEntidadBaseDatos<ServicioPresupuesto, FiltroBusquedaServicioPresupuesto> {
        public RepoServicioPresupuesto() : base("adv__servicio_presupuesto", "id_servicio_presupuesto") { }

        protected override string GenerarComandoAdicionar(ServicioPresupuesto objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_presupuesto (
                    codigo,
                    id_cliente,
                    id_cuenta_usuario,
                    id_servicio_tipo,
                    fecha_emision,
                    fecha_validez,
                    fecha_aprobacion,
                    fecha_rechazo,
                    estado,
                    subtotal,
                    descuento_total,
                    impuesto_total,
                    importe_total,
                    observaciones,
                    terminos_condiciones,
                    id_venta_origen,
                    activo
                ) VALUES (
                    @codigo,
                    @id_cliente,
                    @id_cuenta_usuario,
                    @id_servicio_tipo,
                    @fecha_emision,
                    @fecha_validez,
                    @fecha_aprobacion,
                    @fecha_rechazo,
                    @estado,
                    @subtotal,
                    @descuento_total,
                    @impuesto_total,
                    @importe_total,
                    @observaciones,
                    @terminos_condiciones,
                    @id_venta_origen,
                    @activo
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@codigo", objeto.Codigo },
                { "@id_cliente", objeto.IdCliente },
                { "@id_cuenta_usuario", objeto.IdCuentaUsuario ?? (object)DBNull.Value },
                { "@id_servicio_tipo", objeto.IdServicioTipo ?? (object)DBNull.Value },
                { "@fecha_emision", objeto.FechaEmision },
                { "@fecha_validez", objeto.FechaValidez ?? (object)DBNull.Value },
                { "@fecha_aprobacion", objeto.FechaAprobacion ?? (object)DBNull.Value },
                { "@fecha_rechazo", objeto.FechaRechazo ?? (object)DBNull.Value },
                { "@estado", objeto.Estado },
                { "@subtotal", objeto.Subtotal },
                { "@descuento_total", objeto.DescuentoTotal },
                { "@impuesto_total", objeto.ImpuestoTotal },
                { "@importe_total", objeto.ImporteTotal },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@terminos_condiciones", objeto.TerminosCondiciones ?? string.Empty },
                { "@id_venta_origen", objeto.IdVentaOrigen ?? (object)DBNull.Value },
                { "@activo", objeto.Activo ? 1 : 0 }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioPresupuesto objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_presupuesto 
                SET 
                    codigo = @codigo,
                    id_cliente = @id_cliente,
                    id_cuenta_usuario = @id_cuenta_usuario,
                    id_servicio_tipo = @id_servicio_tipo,
                    fecha_emision = @fecha_emision,
                    fecha_validez = @fecha_validez,
                    fecha_aprobacion = @fecha_aprobacion,
                    fecha_rechazo = @fecha_rechazo,
                    estado = @estado,
                    subtotal = @subtotal,
                    descuento_total = @descuento_total,
                    impuesto_total = @impuesto_total,
                    importe_total = @importe_total,
                    observaciones = @observaciones,
                    terminos_condiciones = @terminos_condiciones,
                    id_venta_origen = @id_venta_origen,
                    activo = @activo
                WHERE id_servicio_presupuesto = @id_servicio_presupuesto;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@codigo", objeto.Codigo },
                { "@id_cliente", objeto.IdCliente },
                { "@id_cuenta_usuario", objeto.IdCuentaUsuario ?? (object)DBNull.Value },
                { "@id_servicio_tipo", objeto.IdServicioTipo ?? (object)DBNull.Value },
                { "@fecha_emision", objeto.FechaEmision },
                { "@fecha_validez", objeto.FechaValidez ?? (object)DBNull.Value },
                { "@fecha_aprobacion", objeto.FechaAprobacion ?? (object)DBNull.Value },
                { "@fecha_rechazo", objeto.FechaRechazo ?? (object)DBNull.Value },
                { "@estado", objeto.Estado },
                { "@subtotal", objeto.Subtotal },
                { "@descuento_total", objeto.DescuentoTotal },
                { "@impuesto_total", objeto.ImpuestoTotal },
                { "@importe_total", objeto.ImporteTotal },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@terminos_condiciones", objeto.TerminosCondiciones ?? string.Empty },
                { "@id_venta_origen", objeto.IdVentaOrigen ?? (object)DBNull.Value },
                { "@activo", objeto.Activo ? 1 : 0 },
                { "@id_servicio_presupuesto", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_presupuesto 
                WHERE id_servicio_presupuesto = @id_servicio_presupuesto;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_presupuesto", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioPresupuesto filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var fechaDesde = criteriosBusqueda.Length == 3 ? criteriosBusqueda[0] : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = criteriosBusqueda.Length == 3 ? criteriosBusqueda[1] : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var criterio = criteriosBusqueda.Length == 3 ? criteriosBusqueda[2] : criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consultaComun = $"""
                SELECT * 
                FROM adv__servicio_presupuesto sp
                WHERE sp.activo = @activo 
                {(criteriosBusqueda.Length == 3 ? "AND sp.fecha_emision >= @fecha_desde AND sp.fecha_emision <= @fecha_hasta" : string.Empty)} 
                """;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaServicioPresupuesto.Id => $"""
                    {consultaComun}
                    AND sp.id_servicio_presupuesto = @id_servicio_presupuesto;
                    """,
                FiltroBusquedaServicioPresupuesto.Codigo => $"""
                    {consultaComun}
                    AND sp.codigo LIKE @codigo;
                    """,
                //FiltroBusquedaServicioPresupuesto.IdCliente => """
                //    SELECT * FROM adv__servicio_presupuesto 
                //    WHERE id_cliente = @id_cliente;
                //    """,
                //FiltroBusquedaServicioPresupuesto.Estado => """
                //    SELECT * FROM adv__servicio_presupuesto 
                //    WHERE estado = @estado;
                //    """,
                //FiltroBusquedaServicioPresupuesto.FechaEmision => """
                //    SELECT * FROM adv__servicio_presupuesto 
                //    WHERE DATE(fecha_emision) = @fecha_emision;
                //    """,
                //FiltroBusquedaServicioPresupuesto.Activos => """
                //    SELECT * FROM adv__servicio_presupuesto 
                //    WHERE activo = 1;
                //    """,
                //FiltroBusquedaServicioPresupuesto.Inactivos => """
                //    SELECT * FROM adv__servicio_presupuesto 
                //    WHERE activo = 0;
                //    """,
                _ => $"{consultaComun};"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaServicioPresupuesto.Id => new Dictionary<string, object>
                {
                    { "@id_servicio_presupuesto", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioPresupuesto.Codigo => new Dictionary<string, object>
                {
                    { "@codigo", $"%{criterio}%" }
                },
                //FiltroBusquedaServicioPresupuesto.IdCliente => new Dictionary<string, object>
                //{
                //    { "@id_cliente", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                //},
                //FiltroBusquedaServicioPresupuesto.Estado => new Dictionary<string, object>
                //{
                //    { "@estado", criterio }
                //},
                //FiltroBusquedaServicioPresupuesto.FechaEmision => new Dictionary<string, object>
                //{
                //    { "@fecha_emision", criterio }
                //},
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (ServicioPresupuesto, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioPresupuesto {
                Id = Convert.ToInt64(lectorDatos["id_servicio_presupuesto"]),
                Codigo = Convert.ToString(lectorDatos["codigo"]) ?? string.Empty,
                IdCliente = Convert.ToInt64(lectorDatos["id_cliente"]),
                IdCuentaUsuario = lectorDatos["id_cuenta_usuario"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_cuenta_usuario"]) : null,
                IdServicioTipo = lectorDatos["id_servicio_tipo"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_servicio_tipo"]) : null,
                FechaEmision = Convert.ToDateTime(lectorDatos["fecha_emision"]),
                FechaValidez = lectorDatos["fecha_validez"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_validez"]) : null,
                FechaAprobacion = lectorDatos["fecha_aprobacion"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_aprobacion"]) : null,
                FechaRechazo = lectorDatos["fecha_rechazo"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_rechazo"]) : null,
                Estado = Convert.ToString(lectorDatos["estado"]) ?? "Borrador",
                Subtotal = Convert.ToDecimal(lectorDatos["subtotal"]),
                DescuentoTotal = Convert.ToDecimal(lectorDatos["descuento_total"]),
                ImpuestoTotal = Convert.ToDecimal(lectorDatos["impuesto_total"]),
                ImporteTotal = Convert.ToDecimal(lectorDatos["importe_total"]),
                Observaciones = lectorDatos["observaciones"] != DBNull.Value ? Convert.ToString(lectorDatos["observaciones"]) : string.Empty,
                TerminosCondiciones = lectorDatos["terminos_condiciones"] != DBNull.Value ? Convert.ToString(lectorDatos["terminos_condiciones"]) : string.Empty,
                IdVentaOrigen = lectorDatos["id_venta_origen"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_venta_origen"]) : null,
                Activo = Convert.ToBoolean(lectorDatos["activo"])
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioPresupuesto Instancia { get; } = new RepoServicioPresupuesto();
        #endregion
        #region UTILES
        public ServicioPresupuesto? ObtenerPorCodigo(string codigo) {
            var consulta = """
                SELECT * FROM adv__servicio_presupuesto 
                WHERE codigo = @codigo LIMIT 1;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@codigo", codigo }
            };

            var resultado = ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad).FirstOrDefault();
            return resultado.entidadBase;
        }

        public List<ServicioPresupuesto> ObtenerPorCliente(long idCliente) {
            var consulta = """
                SELECT * FROM adv__servicio_presupuesto 
                WHERE id_cliente = @id_cliente 
                ORDER BY fecha_emision DESC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_cliente", idCliente }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public bool CambiarEstado(long id, string nuevoEstado) {
            var consulta = """
                UPDATE adv__servicio_presupuesto 
                SET estado = @estado 
                WHERE id_servicio_presupuesto = @id;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@estado", nuevoEstado },
                { "@id", id }
            };

            ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros);

            _cache.Remove($"{NombreTabla}_Id_{id}");

            return true;
        }
        #endregion
    }
}
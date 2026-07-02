using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioPresupuestoDetalle : RepoEntidadBaseDatos<ServicioPresupuestoDetalle, FiltroBusquedaServicioPresupuestoDetalle> {
        public RepoServicioPresupuestoDetalle() : base("adv__servicio_presupuesto_detalle", "id_servicio_presupuesto_detalle") { }

        protected override string GenerarComandoAdicionar(ServicioPresupuestoDetalle objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_presupuesto_detalle (
                    id_servicio_presupuesto,
                    tipo_item,
                    id_servicio_actividad,
                    id_producto,
                    descripcion_personalizada,
                    cantidad,
                    precio_unitario,
                    descuento_item,
                    impuesto_item,
                    orden,
                    observaciones
                ) VALUES (
                    @id_servicio_presupuesto,
                    @tipo_item,
                    @id_servicio_actividad,
                    @id_producto,
                    @descripcion_personalizada,
                    @cantidad,
                    @precio_unitario,
                    @descuento_item,
                    @impuesto_item,
                    @orden,
                    @observaciones
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_presupuesto", objeto.IdServicioPresupuesto },
                { "@tipo_item", objeto.TipoItem },
                { "@id_servicio_actividad", objeto.IdServicioActividad ?? (object)DBNull.Value },
                { "@id_producto", objeto.IdProducto ?? (object)DBNull.Value },
                { "@descripcion_personalizada", objeto.DescripcionPersonalizada ?? string.Empty },
                { "@cantidad", objeto.Cantidad },
                { "@precio_unitario", objeto.PrecioUnitario },
                { "@descuento_item", objeto.DescuentoItem },
                { "@impuesto_item", objeto.ImpuestoItem },
                { "@orden", objeto.Orden },
                { "@observaciones", objeto.Observaciones ?? string.Empty }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioPresupuestoDetalle objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_presupuesto_detalle 
                SET 
                    id_servicio_presupuesto = @id_servicio_presupuesto,
                    tipo_item = @tipo_item,
                    id_servicio_actividad = @id_servicio_actividad,
                    id_producto = @id_producto,
                    descripcion_personalizada = @descripcion_personalizada,
                    cantidad = @cantidad,
                    precio_unitario = @precio_unitario,
                    descuento_item = @descuento_item,
                    impuesto_item = @impuesto_item,
                    orden = @orden,
                    observaciones = @observaciones
                WHERE id_servicio_presupuesto_detalle = @id_servicio_presupuesto_detalle;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_presupuesto", objeto.IdServicioPresupuesto },
                { "@tipo_item", objeto.TipoItem },
                { "@id_servicio_actividad", objeto.IdServicioActividad ?? (object)DBNull.Value },
                { "@id_producto", objeto.IdProducto ?? (object)DBNull.Value },
                { "@descripcion_personalizada", objeto.DescripcionPersonalizada ?? string.Empty },
                { "@cantidad", objeto.Cantidad },
                { "@precio_unitario", objeto.PrecioUnitario },
                { "@descuento_item", objeto.DescuentoItem },
                { "@impuesto_item", objeto.ImpuestoItem },
                { "@orden", objeto.Orden },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@id_servicio_presupuesto_detalle", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_presupuesto_detalle 
                WHERE id_servicio_presupuesto_detalle = @id_servicio_presupuesto_detalle;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_presupuesto_detalle", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioPresupuestoDetalle filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaServicioPresupuestoDetalle.Id => """
                    SELECT * FROM adv__servicio_presupuesto_detalle 
                    WHERE id_servicio_presupuesto_detalle = @id_servicio_presupuesto_detalle
                    ORDER BY orden ASC;
                    """,
                FiltroBusquedaServicioPresupuestoDetalle.IdPresupuesto => """
                    SELECT * FROM adv__servicio_presupuesto_detalle 
                    WHERE id_servicio_presupuesto = @id_servicio_presupuesto
                    ORDER BY orden ASC;
                    """,
                FiltroBusquedaServicioPresupuestoDetalle.TipoItem => """
                    SELECT * FROM adv__servicio_presupuesto_detalle 
                    WHERE tipo_item = @tipo_item
                    ORDER BY orden ASC;
                    """,
                _ => """
                    SELECT * FROM adv__servicio_presupuesto_detalle 
                    ORDER BY orden ASC;
                    """
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaServicioPresupuestoDetalle.Id => new Dictionary<string, object>
                {
                    { "@id_servicio_presupuesto_detalle", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioPresupuestoDetalle.IdPresupuesto => new Dictionary<string, object>
                {
                    { "@id_servicio_presupuesto", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioPresupuestoDetalle.TipoItem => new Dictionary<string, object>
                {
                    { "@tipo_item", criterio }
                },
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (ServicioPresupuestoDetalle, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioPresupuestoDetalle {
                Id = Convert.ToInt64(lectorDatos["id_servicio_presupuesto_detalle"]),
                IdServicioPresupuesto = Convert.ToInt64(lectorDatos["id_servicio_presupuesto"]),
                TipoItem = Convert.ToString(lectorDatos["tipo_item"]) ?? "Actividad",
                IdServicioActividad = lectorDatos["id_servicio_actividad"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_servicio_actividad"]) : null,
                IdProducto = lectorDatos["id_producto"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_producto"]) : null,
                DescripcionPersonalizada = lectorDatos["descripcion_personalizada"] != DBNull.Value ? Convert.ToString(lectorDatos["descripcion_personalizada"]) : string.Empty,
                Cantidad = Convert.ToDecimal(lectorDatos["cantidad"]),
                PrecioUnitario = Convert.ToDecimal(lectorDatos["precio_unitario"]),
                DescuentoItem = Convert.ToDecimal(lectorDatos["descuento_item"]),
                ImpuestoItem = Convert.ToDecimal(lectorDatos["impuesto_item"]),
                Orden = Convert.ToInt32(lectorDatos["orden"]),
                Observaciones = lectorDatos["observaciones"] != DBNull.Value ? Convert.ToString(lectorDatos["observaciones"]) : string.Empty
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioPresupuestoDetalle Instancia { get; } = new RepoServicioPresupuestoDetalle();
        #endregion

        #region UTILES

        public List<ServicioPresupuestoDetalle> ObtenerPorPresupuesto(long idPresupuesto) {
            var consulta = """
                SELECT * FROM adv__servicio_presupuesto_detalle 
                WHERE id_servicio_presupuesto = @id_servicio_presupuesto 
                ORDER BY orden ASC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_servicio_presupuesto", idPresupuesto }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        #endregion
    }
}

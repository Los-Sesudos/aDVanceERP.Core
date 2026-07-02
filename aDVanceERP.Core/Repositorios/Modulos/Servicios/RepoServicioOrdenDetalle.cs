using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioOrdenDetalle : RepoEntidadBaseDatos<ServicioOrdenDetalle, FiltroBusquedaServicioOrdenDetalle> {
        public RepoServicioOrdenDetalle() : base("adv__servicio_orden_detalle", "id_servicio_orden_detalle") { }

        protected override string GenerarComandoAdicionar(ServicioOrdenDetalle objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_orden_detalle (
                    id_servicio_orden,
                    tipo_item,
                    id_servicio_actividad,
                    id_producto,
                    id_almacen_origen,
                    descripcion_personalizada,
                    cantidad_planificada,
                    cantidad_ejecutada,
                    precio_unitario,
                    descuento_item,
                    impuesto_item,
                    orden,
                    observaciones,
                    ejecutado_por,
                    fecha_ejecucion,
                    id_movimiento_salida
                ) VALUES (
                    @id_servicio_orden,
                    @tipo_item,
                    @id_servicio_actividad,
                    @id_producto,
                    @id_almacen_origen,
                    @descripcion_personalizada,
                    @cantidad_planificada,
                    @cantidad_ejecutada,
                    @precio_unitario,
                    @descuento_item,
                    @impuesto_item,
                    @orden,
                    @observaciones,
                    @ejecutado_por,
                    @fecha_ejecucion,
                    @id_movimiento_salida
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", objeto.IdServicioOrden },
                { "@tipo_item", objeto.TipoItem },
                { "@id_servicio_actividad", objeto.IdServicioActividad ?? (object)DBNull.Value },
                { "@id_producto", objeto.IdProducto ?? (object)DBNull.Value },
                { "@id_almacen_origen", objeto.IdAlmacenOrigen ?? (object)DBNull.Value },
                { "@descripcion_personalizada", objeto.DescripcionPersonalizada ?? string.Empty },
                { "@cantidad_planificada", objeto.CantidadPlanificada },
                { "@cantidad_ejecutada", objeto.CantidadEjecutada ?? (object)DBNull.Value },
                { "@precio_unitario", objeto.PrecioUnitario },
                { "@descuento_item", objeto.DescuentoItem },
                { "@impuesto_item", objeto.ImpuestoItem },
                { "@orden", objeto.Orden },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@ejecutado_por", objeto.EjecutadoPor ?? (object)DBNull.Value },
                { "@fecha_ejecucion", objeto.FechaEjecucion ?? (object)DBNull.Value },
                { "@id_movimiento_salida", objeto.IdMovimientoSalida ?? (object)DBNull.Value }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioOrdenDetalle objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_orden_detalle 
                SET 
                    id_servicio_orden = @id_servicio_orden,
                    tipo_item = @tipo_item,
                    id_servicio_actividad = @id_servicio_actividad,
                    id_producto = @id_producto,
                    id_almacen_origen = @id_almacen_origen,
                    descripcion_personalizada = @descripcion_personalizada,
                    cantidad_planificada = @cantidad_planificada,
                    cantidad_ejecutada = @cantidad_ejecutada,
                    precio_unitario = @precio_unitario,
                    descuento_item = @descuento_item,
                    impuesto_item = @impuesto_item,
                    orden = @orden,
                    observaciones = @observaciones,
                    ejecutado_por = @ejecutado_por,
                    fecha_ejecucion = @fecha_ejecucion,
                    id_movimiento_salida = @id_movimiento_salida
                WHERE id_servicio_orden_detalle = @id_servicio_orden_detalle;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", objeto.IdServicioOrden },
                { "@tipo_item", objeto.TipoItem },
                { "@id_servicio_actividad", objeto.IdServicioActividad ?? (object)DBNull.Value },
                { "@id_producto", objeto.IdProducto ?? (object)DBNull.Value },
                { "@id_almacen_origen", objeto.IdAlmacenOrigen ?? (object)DBNull.Value },
                { "@descripcion_personalizada", objeto.DescripcionPersonalizada ?? string.Empty },
                { "@cantidad_planificada", objeto.CantidadPlanificada },
                { "@cantidad_ejecutada", objeto.CantidadEjecutada ?? (object)DBNull.Value },
                { "@precio_unitario", objeto.PrecioUnitario },
                { "@descuento_item", objeto.DescuentoItem },
                { "@impuesto_item", objeto.ImpuestoItem },
                { "@orden", objeto.Orden },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@ejecutado_por", objeto.EjecutadoPor ?? (object)DBNull.Value },
                { "@fecha_ejecucion", objeto.FechaEjecucion ?? (object)DBNull.Value },
                { "@id_movimiento_salida", objeto.IdMovimientoSalida ?? (object)DBNull.Value },
                { "@id_servicio_orden_detalle", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_orden_detalle 
                WHERE id_servicio_orden_detalle = @id_servicio_orden_detalle;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden_detalle", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioOrdenDetalle filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            // Para este repositorio, usamos búsqueda por id_servicio_orden
            var idOrden = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = """
                SELECT * FROM adv__servicio_orden_detalle 
                WHERE id_servicio_orden = @id_servicio_orden 
                ORDER BY orden ASC;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", Convert.ToInt64(string.IsNullOrEmpty(idOrden) ? "0" : idOrden) }
            };

            return consulta;
        }

        protected override (ServicioOrdenDetalle, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioOrdenDetalle {
                Id = Convert.ToInt64(lectorDatos["id_servicio_orden_detalle"]),
                IdServicioOrden = Convert.ToInt64(lectorDatos["id_servicio_orden"]),
                TipoItem = Convert.ToString(lectorDatos["tipo_item"]) ?? "Actividad",
                IdServicioActividad = lectorDatos["id_servicio_actividad"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_servicio_actividad"]) : null,
                IdProducto = lectorDatos["id_producto"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_producto"]) : null,
                IdAlmacenOrigen = lectorDatos["id_almacen_origen"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_almacen_origen"]) : null,
                DescripcionPersonalizada = lectorDatos["descripcion_personalizada"] != DBNull.Value ? Convert.ToString(lectorDatos["descripcion_personalizada"]) : string.Empty,
                CantidadPlanificada = Convert.ToDecimal(lectorDatos["cantidad_planificada"]),
                CantidadEjecutada = lectorDatos["cantidad_ejecutada"] != DBNull.Value ? Convert.ToDecimal(lectorDatos["cantidad_ejecutada"]) : null,
                PrecioUnitario = Convert.ToDecimal(lectorDatos["precio_unitario"]),
                DescuentoItem = Convert.ToDecimal(lectorDatos["descuento_item"]),
                ImpuestoItem = Convert.ToDecimal(lectorDatos["impuesto_item"]),
                Orden = Convert.ToInt32(lectorDatos["orden"]),
                Observaciones = lectorDatos["observaciones"] != DBNull.Value ? Convert.ToString(lectorDatos["observaciones"]) : string.Empty,
                EjecutadoPor = lectorDatos["ejecutado_por"] != DBNull.Value ? Convert.ToInt64(lectorDatos["ejecutado_por"]) : null,
                FechaEjecucion = lectorDatos["fecha_ejecucion"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_ejecucion"]) : null,
                IdMovimientoSalida = lectorDatos["id_movimiento_salida"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_movimiento_salida"]) : null
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioOrdenDetalle Instancia { get; } = new RepoServicioOrdenDetalle();
        #endregion
        #region UTILES
        public List<ServicioOrdenDetalle> ObtenerPorOrden(long idServicioOrden) {
            var consulta = """
                SELECT * FROM adv__servicio_orden_detalle 
                WHERE id_servicio_orden = @id_servicio_orden 
                ORDER BY orden ASC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", idServicioOrden }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public bool RegistrarEjecucion(long idDetalle, decimal cantidadEjecutada, long ejecutadoPor) {
            var consulta = """
                UPDATE adv__servicio_orden_detalle 
                SET cantidad_ejecutada = @cantidad_ejecutada, 
                    ejecutado_por = @ejecutado_por, 
                    fecha_ejecucion = NOW() 
                WHERE id_servicio_orden_detalle = @id;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@cantidad_ejecutada", cantidadEjecutada },
                { "@ejecutado_por", ejecutadoPor },
                { "@id", idDetalle }
            };

            ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros);
            _cache.Remove($"{NombreTabla}_Id_{idDetalle}");
            return true;
        }
        #endregion
    }
}
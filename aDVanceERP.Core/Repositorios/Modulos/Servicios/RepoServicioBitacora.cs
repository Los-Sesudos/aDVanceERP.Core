using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioBitacora : RepoEntidadBaseDatos<ServicioBitacora, FiltroBusquedaServicioBitacora> {
        public RepoServicioBitacora() : base("adv__servicio_bitacora", "id_servicio_bitacora") { }

        protected override string GenerarComandoAdicionar(ServicioBitacora objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_bitacora (
                    id_servicio_orden,
                    id_cuenta_usuario,
                    fecha_registro,
                    tipo_evento,
                    descripcion,
                    foto_url,
                    ubicacion_latitud,
                    ubicacion_longitud
                ) VALUES (
                    @id_servicio_orden,
                    @id_cuenta_usuario,
                    @fecha_registro,
                    @tipo_evento,
                    @descripcion,
                    @foto_url,
                    @ubicacion_latitud,
                    @ubicacion_longitud
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", objeto.IdServicioOrden },
                { "@id_cuenta_usuario", objeto.IdCuentaUsuario ?? (object)DBNull.Value },
                { "@fecha_registro", objeto.FechaRegistro },
                { "@tipo_evento", objeto.TipoEvento },
                { "@descripcion", objeto.Descripcion },
                { "@foto_url", objeto.FotoUrl ?? string.Empty },
                { "@ubicacion_latitud", objeto.UbicacionLatitud ?? (object)DBNull.Value },
                { "@ubicacion_longitud", objeto.UbicacionLongitud ?? (object)DBNull.Value }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioBitacora objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            // Normalmente la bitácora no se edita, pero implementamos por si acaso
            var consulta = """
                UPDATE adv__servicio_bitacora 
                SET 
                    descripcion = @descripcion,
                    foto_url = @foto_url
                WHERE id_servicio_bitacora = @id_servicio_bitacora;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@descripcion", objeto.Descripcion },
                { "@foto_url", objeto.FotoUrl ?? string.Empty },
                { "@id_servicio_bitacora", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_bitacora 
                WHERE id_servicio_bitacora = @id_servicio_bitacora;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_bitacora", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioBitacora filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var idOrden = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = """
                SELECT * FROM adv__servicio_bitacora 
                WHERE id_servicio_orden = @id_servicio_orden 
                ORDER BY fecha_registro DESC;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", Convert.ToInt64(string.IsNullOrEmpty(idOrden) ? "0" : idOrden) }
            };

            return consulta;
        }

        protected override (ServicioBitacora, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioBitacora {
                Id = Convert.ToInt64(lectorDatos["id_servicio_bitacora"]),
                IdServicioOrden = Convert.ToInt64(lectorDatos["id_servicio_orden"]),
                IdCuentaUsuario = lectorDatos["id_cuenta_usuario"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_cuenta_usuario"]) : null,
                FechaRegistro = Convert.ToDateTime(lectorDatos["fecha_registro"]),
                TipoEvento = Convert.ToString(lectorDatos["tipo_evento"]) ?? "Nota",
                Descripcion = Convert.ToString(lectorDatos["descripcion"]) ?? string.Empty,
                FotoUrl = lectorDatos["foto_url"] != DBNull.Value ? Convert.ToString(lectorDatos["foto_url"]) : null,
                UbicacionLatitud = lectorDatos["ubicacion_latitud"] != DBNull.Value ? Convert.ToDecimal(lectorDatos["ubicacion_latitud"]) : null,
                UbicacionLongitud = lectorDatos["ubicacion_longitud"] != DBNull.Value ? Convert.ToDecimal(lectorDatos["ubicacion_longitud"]) : null
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioBitacora Instancia { get; } = new RepoServicioBitacora();
        #endregion
        #region UTILES
        public List<ServicioBitacora> ObtenerPorOrden(long idServicioOrden) {
            var consulta = """
                SELECT * FROM adv__servicio_bitacora 
                WHERE id_servicio_orden = @id_servicio_orden 
                ORDER BY fecha_registro DESC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", idServicioOrden }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public long RegistrarEvento(long idServicioOrden, long? idCuentaUsuario, string tipoEvento, string descripcion,
            string? fotoUrl = null, decimal? latitud = null, decimal? longitud = null) {
            var bitacora = new ServicioBitacora {
                IdServicioOrden = idServicioOrden,
                IdCuentaUsuario = idCuentaUsuario,
                FechaRegistro = DateTime.Now,
                TipoEvento = tipoEvento,
                Descripcion = descripcion,
                FotoUrl = fotoUrl,
                UbicacionLatitud = latitud,
                UbicacionLongitud = longitud
            };

            return Adicionar(bitacora);
        }
        #endregion
    }
}
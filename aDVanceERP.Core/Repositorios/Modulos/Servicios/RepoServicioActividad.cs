using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioActividad : RepoEntidadBaseDatos<ServicioActividad, FiltroBusquedaServicioActividad> {
        public RepoServicioActividad() : base("adv__servicio_actividad", "id_servicio_actividad") { }

        protected override string GenerarComandoAdicionar(ServicioActividad objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_actividad (
                    id_servicio_tipo,
                    codigo,
                    nombre,
                    descripcion,
                    precio_base,
                    unidad_medida,
                    tiempo_estimado_minutos,
                    requiere_materiales,
                    activo
                ) VALUES (
                    @id_servicio_tipo,
                    @codigo,
                    @nombre,
                    @descripcion,
                    @precio_base,
                    @unidad_medida,
                    @tiempo_estimado_minutos,
                    @requiere_materiales,
                    @activo
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_tipo", objeto.IdServicioTipo ?? (object)DBNull.Value },
                { "@codigo", objeto.Codigo },
                { "@nombre", objeto.Nombre },
                { "@descripcion", objeto.Descripcion ?? string.Empty },
                { "@precio_base", objeto.PrecioBase },
                { "@unidad_medida", objeto.UnidadMedida },
                { "@tiempo_estimado_minutos", objeto.TiempoEstimadoMinutos ?? (object)DBNull.Value },
                { "@requiere_materiales", objeto.RequiereMateriales ? 1 : 0 },
                { "@activo", objeto.Activo ? 1 : 0 }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioActividad objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_actividad 
                SET 
                    id_servicio_tipo = @id_servicio_tipo,
                    codigo = @codigo,
                    nombre = @nombre,
                    descripcion = @descripcion,
                    precio_base = @precio_base,
                    unidad_medida = @unidad_medida,
                    tiempo_estimado_minutos = @tiempo_estimado_minutos,
                    requiere_materiales = @requiere_materiales,
                    activo = @activo
                WHERE id_servicio_actividad = @id_servicio_actividad;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_tipo", objeto.IdServicioTipo ?? (object)DBNull.Value },
                { "@codigo", objeto.Codigo },
                { "@nombre", objeto.Nombre },
                { "@descripcion", objeto.Descripcion ?? string.Empty },
                { "@precio_base", objeto.PrecioBase },
                { "@unidad_medida", objeto.UnidadMedida },
                { "@tiempo_estimado_minutos", objeto.TiempoEstimadoMinutos ?? (object)DBNull.Value },
                { "@requiere_materiales", objeto.RequiereMateriales ? 1 : 0 },
                { "@activo", objeto.Activo ? 1 : 0 },
                { "@id_servicio_actividad", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_actividad 
                WHERE id_servicio_actividad = @id_servicio_actividad;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_actividad", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioActividad filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaServicioActividad.Id => """
                    SELECT * FROM adv__servicio_actividad 
                    WHERE id_servicio_actividad = @id_servicio_actividad;
                    """,
                FiltroBusquedaServicioActividad.Codigo => """
                    SELECT * FROM adv__servicio_actividad 
                    WHERE codigo LIKE @codigo;
                    """,
                FiltroBusquedaServicioActividad.Nombre => """
                    SELECT * FROM adv__servicio_actividad 
                    WHERE nombre LIKE @nombre;
                    """,
                FiltroBusquedaServicioActividad.IdServicioTipo => """
                    SELECT * FROM adv__servicio_actividad 
                    WHERE id_servicio_tipo = @id_servicio_tipo;
                    """,
                FiltroBusquedaServicioActividad.Activos => """
                    SELECT * FROM adv__servicio_actividad 
                    WHERE activo = 1;
                    """,
                FiltroBusquedaServicioActividad.Inactivos => """
                    SELECT * FROM adv__servicio_actividad 
                    WHERE activo = 0;
                    """,
                _ => "SELECT * FROM adv__servicio_actividad;"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaServicioActividad.Id => new Dictionary<string, object>
                {
                    { "@id_servicio_actividad", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioActividad.Codigo => new Dictionary<string, object>
                {
                    { "@codigo", $"%{criterio}%" }
                },
                FiltroBusquedaServicioActividad.Nombre => new Dictionary<string, object>
                {
                    { "@nombre", $"%{criterio}%" }
                },
                FiltroBusquedaServicioActividad.IdServicioTipo => new Dictionary<string, object>
                {
                    { "@id_servicio_tipo", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (ServicioActividad, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioActividad {
                Id = Convert.ToInt64(lectorDatos["id_servicio_actividad"]),
                IdServicioTipo = lectorDatos["id_servicio_tipo"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_servicio_tipo"]) : null,
                Codigo = Convert.ToString(lectorDatos["codigo"]) ?? string.Empty,
                Nombre = Convert.ToString(lectorDatos["nombre"]) ?? string.Empty,
                Descripcion = lectorDatos["descripcion"] != DBNull.Value ? Convert.ToString(lectorDatos["descripcion"]) : string.Empty,
                PrecioBase = Convert.ToDecimal(lectorDatos["precio_base"]),
                UnidadMedida = Convert.ToString(lectorDatos["unidad_medida"]) ?? "Servicio",
                TiempoEstimadoMinutos = lectorDatos["tiempo_estimado_minutos"] != DBNull.Value ? Convert.ToInt32(lectorDatos["tiempo_estimado_minutos"]) : null,
                RequiereMateriales = Convert.ToBoolean(lectorDatos["requiere_materiales"]),
                Activo = Convert.ToBoolean(lectorDatos["activo"])
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioActividad Instancia { get; } = new RepoServicioActividad();
        #endregion
    }
}
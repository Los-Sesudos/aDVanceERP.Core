using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioTipo : RepoEntidadBaseDatos<ServicioTipo, FiltroBusquedaServicioTipo> {
        public RepoServicioTipo() : base("adv__servicio_tipo", "id_servicio_tipo") { }

        protected override string GenerarComandoAdicionar(ServicioTipo objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_tipo (
                    nombre,
                    descripcion,
                    activo
                ) VALUES (
                    @nombre,
                    @descripcion,
                    @activo
                );
                """;

            parametros = new Dictionary<string, object> {
                { "@nombre", objeto.Nombre },
                { "@descripcion", objeto.Descripcion ?? string.Empty },
                { "@activo", objeto.Activo ? 1 : 0 }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioTipo objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_tipo 
                SET 
                    nombre = @nombre,
                    descripcion = @descripcion,
                    activo = @activo
                WHERE id_servicio_tipo = @id_servicio_tipo;
                """;

            parametros = new Dictionary<string, object> {
                { "@nombre", objeto.Nombre },
                { "@descripcion", objeto.Descripcion ?? string.Empty },
                { "@activo", objeto.Activo ? 1 : 0 },
                { "@id_servicio_tipo", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_tipo 
                WHERE id_servicio_tipo = @id_servicio_tipo;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_tipo", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioTipo filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaServicioTipo.Id => """
                    SELECT * FROM adv__servicio_tipo 
                    WHERE id_servicio_tipo = @id_servicio_tipo;
                    """,
                FiltroBusquedaServicioTipo.Nombre => """
                    SELECT * FROM adv__servicio_tipo 
                    WHERE nombre LIKE @nombre;
                    """,
                FiltroBusquedaServicioTipo.Activos => """
                    SELECT * FROM adv__servicio_tipo 
                    WHERE activo = 1;
                    """,
                FiltroBusquedaServicioTipo.Inactivos => """
                    SELECT * FROM adv__servicio_tipo 
                    WHERE activo = 0;
                    """,
                _ => "SELECT * FROM adv__servicio_tipo;"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaServicioTipo.Id => new Dictionary<string, object> {
                    { "@id_servicio_tipo", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioTipo.Nombre => new Dictionary<string, object> {
                    { "@nombre", $"%{criterio}%" }
                },
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (ServicioTipo, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioTipo {
                Id = Convert.ToInt64(lectorDatos["id_servicio_tipo"]),
                Nombre = Convert.ToString(lectorDatos["nombre"]) ?? string.Empty,
                Descripcion = lectorDatos["descripcion"] != DBNull.Value ? Convert.ToString(lectorDatos["descripcion"]) : string.Empty,
                Activo = Convert.ToBoolean(lectorDatos["activo"])
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON

        public static RepoServicioTipo Instancia { get; } = new RepoServicioTipo();

        #endregion
    }
}
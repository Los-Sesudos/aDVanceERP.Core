using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Comun;
using aDVanceERP.Core.Modelos.Modulos.Maestros;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Comun {
    public class RepoEmpleado : RepoEntidadBaseDatos<Empleado, FiltroBusquedaEmpleado> {
        public RepoEmpleado() : base("adv__empleado", "id_empleado") {
        }

        protected override string GenerarComandoAdicionar(Empleado entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var comando = $"""
                INSERT INTO adv__empleado (
                    id_persona
                ) VALUES (
                    @id_persona
                );
                """;

            parametros = new Dictionary<string, object> {
                { "@id_persona", entidad.IdPersona }
            };

            return comando;
        }

        protected override string GenerarComandoEditar(Empleado entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var comando = $"""
                UPDATE adv__empleado 
                SET 
                    id_persona = @id_persona
                WHERE id_empleado = @id_empleado
                """;

            parametros = new Dictionary<string, object> {
                { "@id_empleado", entidad.Id },
                { "@id_persona", entidad.IdPersona }
            };

            return comando;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var comando = $"""
                DELETE FROM adv__telefono_contacto 
                WHERE id_persona = (SELECT id_persona FROM adv__empleado WHERE id_empleado = @id_empleado);
                DELETE FROM adv__persona 
                WHERE id_persona = (SELECT id_persona FROM adv__empleado WHERE id_empleado = @id_empleado);
                DELETE FROM adv__empleado 
                WHERE id_empleado = @id_empleado;
                """;

            parametros = new Dictionary<string, object> {
                { "@id_empleado", id }
            };

            return comando;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaEmpleado filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;
            var consultaComun = $"""
                SELECT 
                    e.id_empleado,
                    e.id_persona,
                    p.nombre_completo,
                    p.tipo_documento,
                    p.numero_documento,
                    p.fecha_registro,
                    p.activo
                FROM adv__empleado e
                JOIN adv__persona p ON e.id_persona = p.id_persona
                """;
            var consulta = filtroBusqueda switch {
                FiltroBusquedaEmpleado.Todos => consultaComun,
                FiltroBusquedaEmpleado.Id => consultaComun + " WHERE e.id_empleado = @criterio",
                FiltroBusquedaEmpleado.NombreCompleto => consultaComun + " WHERE p.nombre_completo LIKE @criterio",
                FiltroBusquedaEmpleado.NumeroDocumento => consultaComun + " WHERE p.numero_documento = @criterio",
                _ => throw new ArgumentOutOfRangeException(nameof(filtroBusqueda), filtroBusqueda, null)
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaEmpleado.Todos => new Dictionary<string, object>(),
                FiltroBusquedaEmpleado.Id => new Dictionary<string, object> {
                    { "@criterio", criterio }
                },
                FiltroBusquedaEmpleado.NombreCompleto => new Dictionary<string, object> {
                    { "@criterio", $"%{criterio}%" }
                },
                FiltroBusquedaEmpleado.NumeroDocumento => new Dictionary<string, object> {
                    { "@criterio", criterio }
                },
                _ => throw new ArgumentOutOfRangeException(nameof(filtroBusqueda), filtroBusqueda, null)
            };

            return consulta;
        }

        protected override (Empleado, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            var persona = lector.VisibleFieldCount == 7 ? new Persona() {
                Id = Convert.ToInt64(lector["id_persona"]),
                NombreCompleto = Convert.ToString(lector["nombre_completo"]) ?? "N/A",
                TipoDocumento = Enum.TryParse<TipoDocumentoEnum>(Convert.ToString(lector["tipo_documento"]) ?? "NI", out var tipoDocumento) ? tipoDocumento : TipoDocumentoEnum.CI,
                NumeroDocumento = Convert.ToString(lector["numero_documento"]) ?? "N/A",
                DireccionPrincipal = "N/A",
                FechaRegistro = Convert.ToDateTime(lector["fecha_registro"]),
                Activo = Convert.ToBoolean(lector["activo"])
            } : null;

            return (new Empleado() {
                Id = Convert.ToInt64(lector["id_empleado"]),
                IdPersona = Convert.ToInt64(lector["id_persona"])
            }, [persona!]);
        }

        #region SINGLETON

        public static RepoEmpleado Instancia { get; } = new RepoEmpleado();

        #endregion

        #region UTILES

        public string[] ObtenerNombres() {
            var consulta = $"""
                SELECT p.nombre_completo
                FROM adv__persona p
                LEFT JOIN adv__empleado e ON p.id_persona = e.id_persona
                WHERE p.activo = 1; -- Opcional: solo personas activas
                """;
            var parametros = new Dictionary<string, object>();

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearNombreCompleto).Select(result => result.entidadBase).ToArray() ?? [];
        }

        private (string, List<IEntidadBaseDatos>) MapearNombreCompleto(MySqlDataReader lector) {
            return (Convert.ToString(lector["nombre_completo"]) ?? string.Empty, []);
        }

        #endregion
    }
}

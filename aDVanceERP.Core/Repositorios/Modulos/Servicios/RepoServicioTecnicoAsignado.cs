using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioTecnicoAsignado : RepoEntidadBaseDatos<ServicioTecnicoAsignado, FiltroBusquedaServicioTecnicoAsignado> {
        public RepoServicioTecnicoAsignado() : base("adv__servicio_tecnico_asignado", "id_asignacion") { }

        protected override string GenerarComandoAdicionar(ServicioTecnicoAsignado objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_tecnico_asignado (
                    id_servicio_orden,
                    id_empleado,
                    id_cuenta_usuario_asigno,
                    rol,
                    horas_trabajadas,
                    fecha_asignacion,
                    fecha_desasignacion,
                    activo
                ) VALUES (
                    @id_servicio_orden,
                    @id_empleado,
                    @id_cuenta_usuario_asigno,
                    @rol,
                    @horas_trabajadas,
                    @fecha_asignacion,
                    @fecha_desasignacion,
                    @activo
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", objeto.IdServicioOrden },
                { "@id_empleado", objeto.IdEmpleado },
                { "@id_cuenta_usuario_asigno", objeto.IdCuentaUsuarioAsigno },
                { "@rol", objeto.Rol },
                { "@horas_trabajadas", objeto.HorasTrabajadas ?? (object)DBNull.Value },
                { "@fecha_asignacion", objeto.FechaAsignacion },
                { "@fecha_desasignacion", objeto.FechaDesasignacion ?? (object)DBNull.Value },
                { "@activo", objeto.Activo ? 1 : 0 }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioTecnicoAsignado objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_tecnico_asignado 
                SET 
                    rol = @rol,
                    horas_trabajadas = @horas_trabajadas,
                    fecha_desasignacion = @fecha_desasignacion,
                    activo = @activo
                WHERE id_asignacion = @id_asignacion;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@rol", objeto.Rol },
                { "@horas_trabajadas", objeto.HorasTrabajadas ?? (object)DBNull.Value },
                { "@fecha_desasignacion", objeto.FechaDesasignacion ?? (object)DBNull.Value },
                { "@activo", objeto.Activo ? 1 : 0 },
                { "@id_asignacion", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_tecnico_asignado 
                WHERE id_asignacion = @id_asignacion;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_asignacion", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioTecnicoAsignado filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var idOrden = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = """
                SELECT * FROM adv__servicio_tecnico_asignado 
                WHERE id_servicio_orden = @id_servicio_orden AND activo = 1
                ORDER BY CASE WHEN rol = 'Principal' THEN 0 ELSE 1 END;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", Convert.ToInt64(string.IsNullOrEmpty(idOrden) ? "0" : idOrden) }
            };

            return consulta;
        }

        protected override (ServicioTecnicoAsignado, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioTecnicoAsignado {
                Id = Convert.ToInt64(lectorDatos["id_asignacion"]),
                IdServicioOrden = Convert.ToInt64(lectorDatos["id_servicio_orden"]),
                IdEmpleado = Convert.ToInt64(lectorDatos["id_empleado"]),
                IdCuentaUsuarioAsigno = Convert.ToInt64(lectorDatos["id_cuenta_usuario_asigno"]),
                Rol = Convert.ToString(lectorDatos["rol"]) ?? "Secundario",
                HorasTrabajadas = lectorDatos["horas_trabajadas"] != DBNull.Value ? Convert.ToDecimal(lectorDatos["horas_trabajadas"]) : null,
                FechaAsignacion = Convert.ToDateTime(lectorDatos["fecha_asignacion"]),
                FechaDesasignacion = lectorDatos["fecha_desasignacion"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_desasignacion"]) : null,
                Activo = Convert.ToBoolean(lectorDatos["activo"])
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioTecnicoAsignado Instancia { get; } = new RepoServicioTecnicoAsignado();

        public List<ServicioTecnicoAsignado> ObtenerPorOrden(long idServicioOrden) {
            var consulta = """
                SELECT * FROM adv__servicio_tecnico_asignado 
                WHERE id_servicio_orden = @id_servicio_orden AND activo = 1
                ORDER BY CASE WHEN rol = 'Principal' THEN 0 ELSE 1 END;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", idServicioOrden }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public List<ServicioTecnicoAsignado> ObtenerPorEmpleado(long idEmpleado) {
            var consulta = """
                SELECT * FROM adv__servicio_tecnico_asignado 
                WHERE id_empleado = @id_empleado AND activo = 1;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_empleado", idEmpleado }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public bool Desasignar(long idAsignacion) {
            var consulta = """
                UPDATE adv__servicio_tecnico_asignado 
                SET activo = 0, fecha_desasignacion = NOW() 
                WHERE id_asignacion = @id;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id", idAsignacion }
            };

            ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros);
            _cache.Remove($"{NombreTabla}_Id_{idAsignacion}");
            return true;
        }

        public bool RegistrarHoras(long idAsignacion, decimal horas) {
            var consulta = """
                UPDATE adv__servicio_tecnico_asignado 
                SET horas_trabajadas = horas_trabajadas + @horas 
                WHERE id_asignacion = @id;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@horas", horas },
                { "@id", idAsignacion }
            };

            ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros);
            _cache.Remove($"{NombreTabla}_Id_{idAsignacion}");
            return true;
        }
        #endregion
    }
}
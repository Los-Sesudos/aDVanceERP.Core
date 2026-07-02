using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Servicios;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Servicios {
    public sealed class RepoServicioOrden : RepoEntidadBaseDatos<ServicioOrden, FiltroBusquedaServicioOrden> {
        public RepoServicioOrden() : base("adv__servicio_orden", "id_servicio_orden") { }

        protected override string GenerarComandoAdicionar(ServicioOrden objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                INSERT INTO adv__servicio_orden (
                    codigo,
                    id_cliente,
                    id_cuenta_usuario,
                    id_servicio_tipo,
                    id_servicio_presupuesto,
                    id_venta,
                    fecha_creacion,
                    fecha_inicio_programada,
                    fecha_fin_programada,
                    fecha_inicio_real,
                    fecha_fin_real,
                    estado,
                    prioridad,
                    ubicacion_servicio,
                    contacto_cliente,
                    telefono_contacto,
                    subtotal,
                    descuento_total,
                    impuesto_total,
                    importe_total,
                    observaciones,
                    notas_internas,
                    activo,
                    id_movimiento_ingreso
                ) VALUES (
                    @codigo,
                    @id_cliente,
                    @id_cuenta_usuario,
                    @id_servicio_tipo,
                    @id_servicio_presupuesto,
                    @id_venta,
                    @fecha_creacion,
                    @fecha_inicio_programada,
                    @fecha_fin_programada,
                    @fecha_inicio_real,
                    @fecha_fin_real,
                    @estado,
                    @prioridad,
                    @ubicacion_servicio,
                    @contacto_cliente,
                    @telefono_contacto,
                    @subtotal,
                    @descuento_total,
                    @impuesto_total,
                    @importe_total,
                    @observaciones,
                    @notas_internas,
                    @activo,
                    @id_movimiento_ingreso
                );
                """;

            parametros = new Dictionary<string, object>
            {
                { "@codigo", objeto.Codigo },
                { "@id_cliente", objeto.IdCliente },
                { "@id_cuenta_usuario", objeto.IdCuentaUsuario ?? (object)DBNull.Value },
                { "@id_servicio_tipo", objeto.IdServicioTipo ?? (object)DBNull.Value },
                { "@id_servicio_presupuesto", objeto.IdServicioPresupuesto ?? (object)DBNull.Value },
                { "@id_venta", objeto.IdVenta ?? (object)DBNull.Value },
                { "@fecha_creacion", objeto.FechaCreacion },
                { "@fecha_inicio_programada", objeto.FechaInicioProgramada ?? (object)DBNull.Value },
                { "@fecha_fin_programada", objeto.FechaFinProgramada ?? (object)DBNull.Value },
                { "@fecha_inicio_real", objeto.FechaInicioReal ?? (object)DBNull.Value },
                { "@fecha_fin_real", objeto.FechaFinReal ?? (object)DBNull.Value },
                { "@estado", objeto.Estado },
                { "@prioridad", objeto.Prioridad },
                { "@ubicacion_servicio", objeto.UbicacionServicio ?? string.Empty },
                { "@contacto_cliente", objeto.ContactoCliente ?? string.Empty },
                { "@telefono_contacto", objeto.TelefonoContacto ?? string.Empty },
                { "@subtotal", objeto.Subtotal },
                { "@descuento_total", objeto.DescuentoTotal },
                { "@impuesto_total", objeto.ImpuestoTotal },
                { "@importe_total", objeto.ImporteTotal },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@notas_internas", objeto.NotasInternas ?? string.Empty },
                { "@activo", objeto.Activo ? 1 : 0 },
                { "@id_movimiento_ingreso", objeto.IdMovimientoIngreso ?? (object)DBNull.Value }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(ServicioOrden objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = """
                UPDATE adv__servicio_orden 
                SET 
                    codigo = @codigo,
                    id_cliente = @id_cliente,
                    id_cuenta_usuario = @id_cuenta_usuario,
                    id_servicio_tipo = @id_servicio_tipo,
                    id_servicio_presupuesto = @id_servicio_presupuesto,
                    id_venta = @id_venta,
                    fecha_creacion = @fecha_creacion,
                    fecha_inicio_programada = @fecha_inicio_programada,
                    fecha_fin_programada = @fecha_fin_programada,
                    fecha_inicio_real = @fecha_inicio_real,
                    fecha_fin_real = @fecha_fin_real,
                    estado = @estado,
                    prioridad = @prioridad,
                    ubicacion_servicio = @ubicacion_servicio,
                    contacto_cliente = @contacto_cliente,
                    telefono_contacto = @telefono_contacto,
                    subtotal = @subtotal,
                    descuento_total = @descuento_total,
                    impuesto_total = @impuesto_total,
                    importe_total = @importe_total,
                    observaciones = @observaciones,
                    notas_internas = @notas_internas,
                    activo = @activo,
                    id_movimiento_ingreso = @id_movimiento_ingreso
                WHERE id_servicio_orden = @id_servicio_orden;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@codigo", objeto.Codigo },
                { "@id_cliente", objeto.IdCliente },
                { "@id_cuenta_usuario", objeto.IdCuentaUsuario ?? (object)DBNull.Value },
                { "@id_servicio_tipo", objeto.IdServicioTipo ?? (object)DBNull.Value },
                { "@id_servicio_presupuesto", objeto.IdServicioPresupuesto ?? (object)DBNull.Value },
                { "@id_venta", objeto.IdVenta ?? (object)DBNull.Value },
                { "@fecha_creacion", objeto.FechaCreacion },
                { "@fecha_inicio_programada", objeto.FechaInicioProgramada ?? (object)DBNull.Value },
                { "@fecha_fin_programada", objeto.FechaFinProgramada ?? (object)DBNull.Value },
                { "@fecha_inicio_real", objeto.FechaInicioReal ?? (object)DBNull.Value },
                { "@fecha_fin_real", objeto.FechaFinReal ?? (object)DBNull.Value },
                { "@estado", objeto.Estado },
                { "@prioridad", objeto.Prioridad },
                { "@ubicacion_servicio", objeto.UbicacionServicio ?? string.Empty },
                { "@contacto_cliente", objeto.ContactoCliente ?? string.Empty },
                { "@telefono_contacto", objeto.TelefonoContacto ?? string.Empty },
                { "@subtotal", objeto.Subtotal },
                { "@descuento_total", objeto.DescuentoTotal },
                { "@impuesto_total", objeto.ImpuestoTotal },
                { "@importe_total", objeto.ImporteTotal },
                { "@observaciones", objeto.Observaciones ?? string.Empty },
                { "@notas_internas", objeto.NotasInternas ?? string.Empty },
                { "@activo", objeto.Activo ? 1 : 0 },
                { "@id_movimiento_ingreso", objeto.IdMovimientoIngreso ?? (object)DBNull.Value },
                { "@id_servicio_orden", objeto.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = """
                DELETE FROM adv__servicio_orden 
                WHERE id_servicio_orden = @id_servicio_orden;
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_servicio_orden", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaServicioOrden filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaServicioOrden.Id => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE id_servicio_orden = @id_servicio_orden;
                    """,
                FiltroBusquedaServicioOrden.Codigo => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE codigo LIKE @codigo;
                    """,
                FiltroBusquedaServicioOrden.IdCliente => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE id_cliente = @id_cliente;
                    """,
                FiltroBusquedaServicioOrden.Estado => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE estado = @estado;
                    """,
                FiltroBusquedaServicioOrden.Prioridad => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE prioridad = @prioridad;
                    """,
                FiltroBusquedaServicioOrden.IdServicioTipo => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE id_servicio_tipo = @id_servicio_tipo;
                    """,
                FiltroBusquedaServicioOrden.FechaCreacion => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE DATE(fecha_creacion) = @fecha_creacion;
                    """,
                FiltroBusquedaServicioOrden.Activas => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE activo = 1;
                    """,
                FiltroBusquedaServicioOrden.Inactivas => """
                    SELECT * FROM adv__servicio_orden 
                    WHERE activo = 0;
                    """,
                _ => "SELECT * FROM adv__servicio_orden;"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaServicioOrden.Id => new Dictionary<string, object>
                {
                    { "@id_servicio_orden", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioOrden.Codigo => new Dictionary<string, object>
                {
                    { "@codigo", $"%{criterio}%" }
                },
                FiltroBusquedaServicioOrden.IdCliente => new Dictionary<string, object>
                {
                    { "@id_cliente", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioOrden.Estado => new Dictionary<string, object>
                {
                    { "@estado", criterio }
                },
                FiltroBusquedaServicioOrden.Prioridad => new Dictionary<string, object>
                {
                    { "@prioridad", criterio }
                },
                FiltroBusquedaServicioOrden.IdServicioTipo => new Dictionary<string, object>
                {
                    { "@id_servicio_tipo", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) }
                },
                FiltroBusquedaServicioOrden.FechaCreacion => new Dictionary<string, object>
                {
                    { "@fecha_creacion", criterio }
                },
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (ServicioOrden, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lectorDatos) {
            var entidad = new ServicioOrden {
                Id = Convert.ToInt64(lectorDatos["id_servicio_orden"]),
                Codigo = Convert.ToString(lectorDatos["codigo"]) ?? string.Empty,
                IdCliente = Convert.ToInt64(lectorDatos["id_cliente"]),
                IdCuentaUsuario = lectorDatos["id_cuenta_usuario"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_cuenta_usuario"]) : null,
                IdServicioTipo = lectorDatos["id_servicio_tipo"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_servicio_tipo"]) : null,
                IdServicioPresupuesto = lectorDatos["id_servicio_presupuesto"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_servicio_presupuesto"]) : null,
                IdVenta = lectorDatos["id_venta"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_venta"]) : null,
                FechaCreacion = Convert.ToDateTime(lectorDatos["fecha_creacion"]),
                FechaInicioProgramada = lectorDatos["fecha_inicio_programada"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_inicio_programada"]) : null,
                FechaFinProgramada = lectorDatos["fecha_fin_programada"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_fin_programada"]) : null,
                FechaInicioReal = lectorDatos["fecha_inicio_real"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_inicio_real"]) : null,
                FechaFinReal = lectorDatos["fecha_fin_real"] != DBNull.Value ? Convert.ToDateTime(lectorDatos["fecha_fin_real"]) : null,
                Estado = Convert.ToString(lectorDatos["estado"]) ?? "Pendiente",
                Prioridad = Convert.ToString(lectorDatos["prioridad"]) ?? "Media",
                UbicacionServicio = lectorDatos["ubicacion_servicio"] != DBNull.Value ? Convert.ToString(lectorDatos["ubicacion_servicio"]) : string.Empty,
                ContactoCliente = lectorDatos["contacto_cliente"] != DBNull.Value ? Convert.ToString(lectorDatos["contacto_cliente"]) : string.Empty,
                TelefonoContacto = lectorDatos["telefono_contacto"] != DBNull.Value ? Convert.ToString(lectorDatos["telefono_contacto"]) : string.Empty,
                Subtotal = Convert.ToDecimal(lectorDatos["subtotal"]),
                DescuentoTotal = Convert.ToDecimal(lectorDatos["descuento_total"]),
                ImpuestoTotal = Convert.ToDecimal(lectorDatos["impuesto_total"]),
                ImporteTotal = Convert.ToDecimal(lectorDatos["importe_total"]),
                Observaciones = lectorDatos["observaciones"] != DBNull.Value ? Convert.ToString(lectorDatos["observaciones"]) : string.Empty,
                NotasInternas = lectorDatos["notas_internas"] != DBNull.Value ? Convert.ToString(lectorDatos["notas_internas"]) : string.Empty,
                Activo = Convert.ToBoolean(lectorDatos["activo"]),
                IdMovimientoIngreso = lectorDatos["id_movimiento_ingreso"] != DBNull.Value ? Convert.ToInt64(lectorDatos["id_movimiento_ingreso"]) : null
            };

            return (entidad, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON
        public static RepoServicioOrden Instancia { get; } = new RepoServicioOrden();

        public ServicioOrden? ObtenerPorCodigo(string codigo) {
            var consulta = """
                SELECT * FROM adv__servicio_orden 
                WHERE codigo = @codigo LIMIT 1;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@codigo", codigo }
            };

            var resultado = ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad).FirstOrDefault();
            return resultado.entidadBase;
        }

        public List<ServicioOrden> ObtenerPorCliente(long idCliente) {
            var consulta = """
                SELECT * FROM adv__servicio_orden 
                WHERE id_cliente = @id_cliente 
                ORDER BY fecha_creacion DESC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_cliente", idCliente }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public List<ServicioOrden> ObtenerPorEstado(string estado) {
            var consulta = """
                SELECT * FROM adv__servicio_orden 
                WHERE estado = @estado 
                ORDER BY prioridad DESC, fecha_creacion ASC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@estado", estado }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public List<ServicioOrden> ObtenerPorTecnico(long idEmpleado) {
            var consulta = """
                SELECT so.* FROM adv__servicio_orden so
                INNER JOIN adv__servicio_tecnico_asignado sta ON so.id_servicio_orden = sta.id_servicio_orden
                WHERE sta.id_empleado = @id_empleado AND sta.activo = 1
                ORDER BY so.fecha_creacion DESC;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id_empleado", idEmpleado }
            };

            return ContextoBaseDatos.EjecutarConsulta(consulta, parametros, MapearEntidad)
                .Select(r => r.entidadBase)
                .ToList();
        }

        public bool CambiarEstado(long id, string nuevoEstado) {
            var consulta = """
                UPDATE adv__servicio_orden 
                SET estado = @estado 
                WHERE id_servicio_orden = @id;
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

        public bool RegistrarInicioReal(long id) {
            var consulta = """
                UPDATE adv__servicio_orden 
                SET fecha_inicio_real = NOW(), estado = 'EnProgreso' 
                WHERE id_servicio_orden = @id;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id", id }
            };

            ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros);
            _cache.Remove($"{NombreTabla}_Id_{id}");
            return true;
        }

        public bool RegistrarFinReal(long id) {
            var consulta = """
                UPDATE adv__servicio_orden 
                SET fecha_fin_real = NOW(), estado = 'Completada' 
                WHERE id_servicio_orden = @id;
                """;
            var parametros = new Dictionary<string, object>
            {
                { "@id", id }
            };

            ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros);
            _cache.Remove($"{NombreTabla}_Id_{id}");
            return true;
        }
        #endregion
    }
}
using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Modulos.Servicios.Estadisticas;

namespace aDVanceERP.Core.Repositorios.Modulos.Estadisticas {
    public sealed class RepoEstadisticasServicios {
        public RepoEstadisticasServicios() { }

        public MetricasServicios ObtenerMetricas() {
            var m = new MetricasServicios();

            // KPIs principales
            m.OrdenesActivas = ObtenerOrdenesActivas();
            m.OrdenesCompletadasMes = ObtenerOrdenesCompletadasMes(DateTime.Today.Year, DateTime.Today.Month);
            m.PresupuestosEnviados = ObtenerPresupuestosPorEstado("Enviado");
            m.PresupuestosVencenProximos = ObtenerPresupuestosPorVencer(3); // próximos 3 días
            m.FacturadoMes = ObtenerFacturadoMes(DateTime.Today.Year, DateTime.Today.Month);
            m.GarantiasActivas = ObtenerGarantiasActivas();
            m.OrdenesUrgentesCola = ObtenerOrdenesPorPrioridadYEstado("Urgente", new[] { "Pendiente", "Programada", "EnProgreso" });
            m.TasaConversionPresupuestos = ObtenerTasaConversionPresupuestos();

            // Datos para gráficas
            m.EvolucionOrdenesCompletadas = ObtenerEvolucionOrdenesCompletadas(30);
            m.DistribucionPorTipoServicio = ObtenerDistribucionPorTipoServicio();
            m.TopClientesFacturado = ObtenerTopClientesFacturado(5);
            //m.OrdenesActivasResumen = ObtenerOrdenesActivasResumen(10); // últimas 10 activas

            return m;
        }

        // ============================== KPIs ESCALARES ==============================

        /// <summary>Órdenes en estado Pendiente, Programada, EnProgreso o EnPausa.</summary>
        public int ObtenerOrdenesActivas() {
            string sql = @"
                SELECT COUNT(*)
                FROM adv__servicio_orden
                WHERE estado IN ('Pendiente', 'Programada', 'EnProgreso', 'EnPausa')
                  AND activo = 1";
            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(sql);
        }

        /// <summary>Órdenes completadas en un mes específico.</summary>
        public int ObtenerOrdenesCompletadasMes(int anio, int mes) {
            string sql = @"
                SELECT COUNT(*)
                FROM adv__servicio_orden
                WHERE estado = 'Completada'
                  AND YEAR(fecha_creacion) = @anio
                  AND MONTH(fecha_creacion) = @mes
                  AND activo = 1";
            var parametros = new Dictionary<string, object>
            {
                { "@anio", anio },
                { "@mes", mes }
            };
            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(sql, parametros);
        }

        /// <summary>Presupuestos en un estado específico (Ej: 'Enviado').</summary>
        public int ObtenerPresupuestosPorEstado(string estado) {
            string sql = @"
                SELECT COUNT(*)
                FROM adv__servicio_presupuesto
                WHERE estado = @estado
                  AND activo = 1";
            var parametros = new Dictionary<string, object> { { "@estado", estado } };
            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(sql, parametros);
        }

        /// <summary>Presupuestos que vencen en los próximos 'dias' días (incluye hoy).</summary>
        public int ObtenerPresupuestosPorVencer(int dias) {
            string sql = @"
                SELECT COUNT(*)
                FROM adv__servicio_presupuesto
                WHERE estado = 'Enviado'
                  AND fecha_validez BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)
                  AND activo = 1";
            var parametros = new Dictionary<string, object> { { "@dias", dias } };
            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(sql, parametros);
        }

        /// <summary>Total facturado (importe_total) en un mes específico (órdenes facturadas).</summary>
        public decimal ObtenerFacturadoMes(int anio, int mes) {
            string sql = @"
                SELECT COALESCE(SUM(importe_total), 0)
                FROM adv__servicio_orden
                WHERE estado = 'Facturada'
                  AND YEAR(fecha_creacion) = @anio
                  AND MONTH(fecha_creacion) = @mes
                  AND activo = 1";
            var parametros = new Dictionary<string, object>
            {
                { "@anio", anio },
                { "@mes", mes }
            };
            return ContextoBaseDatos.EjecutarConsultaEscalar<decimal>(sql, parametros);
        }

        /// <summary>Garantías en estado 'Activa'.</summary>
        public int ObtenerGarantiasActivas() {
            string sql = @"
                SELECT COUNT(*)
                FROM adv__servicio_garantia
                WHERE estado = 'Activa'";
            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(sql);
        }

        /// <summary>Órdenes con prioridad 'Urgente' y en estados especificados.</summary>
        public int ObtenerOrdenesPorPrioridadYEstado(string prioridad, string[] estados) {
            if (estados == null || estados.Length == 0)
                return 0;
            string placeholders = string.Join(",", estados.Select((_, i) => $"@estado{i}"));
            string sql = $@"
                SELECT COUNT(*)
                FROM adv__servicio_orden
                WHERE prioridad = @prioridad
                  AND estado IN ({placeholders})
                  AND activo = 1";
            var parametros = new Dictionary<string, object> { { "@prioridad", prioridad } };
            for (int i = 0; i < estados.Length; i++)
                parametros.Add($"@estado{i}", estados[i]);
            return ContextoBaseDatos.EjecutarConsultaEscalar<int>(sql, parametros);
        }

        /// <summary>Tasa de conversión de presupuestos aprobados o convertidos vs. enviados.</summary>
        public decimal ObtenerTasaConversionPresupuestos() {
            string sql = @"
                SELECT 
                    COALESCE(
                        (SELECT COUNT(*) FROM adv__servicio_presupuesto WHERE estado IN ('Aprobado', 'Convertido') AND activo = 1) * 100.0 /
                        NULLIF((SELECT COUNT(*) FROM adv__servicio_presupuesto WHERE estado = 'Enviado' AND activo = 1), 0)
                    , 0) AS tasa";
            return ContextoBaseDatos.EjecutarConsultaEscalar<decimal>(sql);
        }

        // ============================== SERIES / TABLAS ==============================

        /// <summary>Evolución de órdenes completadas por día (últimos N días).</summary>
        public List<SerieDiariaOrdenes> ObtenerEvolucionOrdenesCompletadas(int dias = 30) {
            string sql = $@"
                SELECT
                    DATE(fecha_creacion) AS fecha,
                    COUNT(*) AS cantidad
                FROM adv__servicio_orden
                WHERE estado = 'Completada'
                  AND fecha_creacion >= DATE_SUB(CURDATE(), INTERVAL {dias} DAY)
                  AND activo = 1
                GROUP BY DATE(fecha_creacion)
                ORDER BY fecha ASC";
            return ContextoBaseDatos
                .EjecutarConsulta(sql, null, lector => {
                    var item = new SerieDiariaOrdenes {
                        Fecha = Convert.ToDateTime(lector["fecha"]),
                        Cantidad = Convert.ToInt32(lector["cantidad"])
                    };
                    return (item, new List<Core.Modelos.Comun.Interfaces.IEntidadBaseDatos>());
                })
                .Select(r => r.entidadBase)
                .ToList();
        }

        /// <summary>Distribución porcentual de órdenes completadas por tipo de servicio (últimos 12 meses).</summary>
        public List<DistribucionTipoServicio> ObtenerDistribucionPorTipoServicio() {
            string sql = @"
                SELECT
                    COALESCE(st.nombre, 'Otros') AS tipo_servicio,
                    COUNT(so.id_servicio_orden) AS total_ordenes,
                    ROUND(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER(), 1) AS porcentaje
                FROM adv__servicio_orden so
                LEFT JOIN adv__servicio_tipo st ON so.id_servicio_tipo = st.id_servicio_tipo
                WHERE so.estado = 'Completada'
                  AND so.fecha_creacion >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)
                  AND so.activo = 1
                GROUP BY st.id_servicio_tipo, st.nombre
                ORDER BY total_ordenes DESC";
            return ContextoBaseDatos
                .EjecutarConsulta(sql, null, lector => {
                    var item = new DistribucionTipoServicio {
                        TipoServicio = Convert.ToString(lector["tipo_servicio"]) ?? "Otros",
                        TotalOrdenes = Convert.ToInt32(lector["total_ordenes"]),
                        Porcentaje = Convert.ToDecimal(lector["porcentaje"])
                    };
                    return (item, new List<Core.Modelos.Comun.Interfaces.IEntidadBaseDatos>());
                })
                .Select(r => r.entidadBase)
                .ToList();
        }

        /// <summary>Top clientes según el importe total facturado en servicios (órdenes facturadas).</summary>
        public List<ClienteTopFacturacion> ObtenerTopClientesFacturado(int top = 5) {
            string sql = $@"
                SELECT
                    p.nombre_completo AS cliente,
                    SUM(so.importe_total) AS total_facturado
                FROM adv__servicio_orden so
                INNER JOIN adv__cliente c ON so.id_cliente = c.id_cliente
                INNER JOIN adv__persona p ON c.id_persona = p.id_persona
                WHERE so.estado = 'Facturada'
                  AND so.activo = 1
                GROUP BY c.id_cliente, p.nombre_completo
                ORDER BY total_facturado DESC
                LIMIT {top}";
            return ContextoBaseDatos
                .EjecutarConsulta(sql, null, lector => {
                    var item = new ClienteTopFacturacion {
                        Cliente = Convert.ToString(lector["cliente"]) ?? string.Empty,
                        TotalFacturado = Convert.ToDecimal(lector["total_facturado"])
                    };
                    return (item, new List<Core.Modelos.Comun.Interfaces.IEntidadBaseDatos>());
                })
                .Select(r => r.entidadBase)
                .ToList();
        }

        /// <summary>Lista resumida de órdenes activas (para la tabla inferior del dashboard).</summary>
        public List<OrdenActivaResumen> ObtenerOrdenesActivasResumen(int limite = 10) {
            string sql = $@"
                SELECT
                    so.codigo,
                    p.nombre_completo AS cliente,
                    COALESCE(st.nombre, 'Sin tipo') AS tipo_servicio,
                    so.estado,
                    so.prioridad,
                    so.fecha_inicio_programada AS inicio_programado,
                    so.fecha_fin_programada AS fin_programado,
                    CASE 
                        WHEN so.estado = 'EnProgreso' THEN
                            ROUND(TIMESTAMPDIFF(HOUR, so.fecha_inicio_real, NOW()) * 100.0 / 
                                  NULLIF(TIMESTAMPDIFF(HOUR, so.fecha_inicio_programada, so.fecha_fin_programada), 0), 0)
                        ELSE 0
                    END AS porcentaje_avance
                FROM adv__servicio_orden so
                INNER JOIN adv__cliente c ON so.id_cliente = c.id_cliente
                INNER JOIN adv__persona p ON c.id_persona = p.id_persona
                LEFT JOIN adv__servicio_tipo st ON so.id_servicio_tipo = st.id_servicio_tipo
                WHERE so.estado IN ('Pendiente', 'Programada', 'EnProgreso', 'EnPausa')
                  AND so.activo = 1
                ORDER BY 
                    CASE so.prioridad
                        WHEN 'Urgente' THEN 1
                        WHEN 'Alta' THEN 2
                        WHEN 'Media' THEN 3
                        ELSE 4
                    END,
                    so.fecha_inicio_programada
                LIMIT {limite}";
            return ContextoBaseDatos
                .EjecutarConsulta(sql, null, lector => {
                    var item = new OrdenActivaResumen {
                        Codigo = Convert.ToString(lector["codigo"]) ?? string.Empty,
                        Cliente = Convert.ToString(lector["cliente"]) ?? string.Empty,
                        TipoServicio = Convert.ToString(lector["tipo_servicio"]) ?? string.Empty,
                        Estado = Convert.ToString(lector["estado"]) ?? string.Empty,
                        Prioridad = Convert.ToString(lector["prioridad"]) ?? string.Empty,
                        InicioProgramado = lector["inicio_programado"] != DBNull.Value ? Convert.ToDateTime(lector["inicio_programado"]) : null,
                        FinProgramado = lector["fin_programado"] != DBNull.Value ? Convert.ToDateTime(lector["fin_programado"]) : null,
                        PorcentajeAvance = lector["porcentaje_avance"] != DBNull.Value ? Convert.ToInt32(lector["porcentaje_avance"]) : 0
                    };
                    return (item, new List<Core.Modelos.Comun.Interfaces.IEntidadBaseDatos>());
                })
                .Select(r => r.entidadBase)
                .ToList();
        }

        #region SINGLETON
        public static RepoEstadisticasServicios Instancia => new RepoEstadisticasServicios();
        #endregion
    }
}
namespace aDVanceERP.Core.Modelos.Modulos.Servicios.Estadisticas {
    public class MetricasServicios {
        // KPIs escalares
        public int OrdenesActivas { get; set; }
        public int OrdenesCompletadasMes { get; set; }
        public int PresupuestosEnviados { get; set; }
        public int PresupuestosVencenProximos { get; set; }
        public decimal FacturadoMes { get; set; }
        public int GarantiasActivas { get; set; }
        public int OrdenesUrgentesCola { get; set; }
        public decimal TasaConversionPresupuestos { get; set; }

        // Series / tablas
        public List<SerieDiariaOrdenes> EvolucionOrdenesCompletadas { get; set; } = new();
        public List<DistribucionTipoServicio> DistribucionPorTipoServicio { get; set; } = new();
        public List<ClienteTopFacturacion> TopClientesFacturado { get; set; } = new();
        public List<OrdenActivaResumen> OrdenesActivasResumen { get; set; } = new();
    }
}

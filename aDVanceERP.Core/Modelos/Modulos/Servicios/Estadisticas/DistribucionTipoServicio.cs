namespace aDVanceERP.Core.Modelos.Modulos.Servicios.Estadisticas {
    public class DistribucionTipoServicio {
        public string TipoServicio { get; set; } = string.Empty;
        public int TotalOrdenes { get; set; }
        public decimal Porcentaje { get; set; }
    }
}

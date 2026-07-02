namespace aDVanceERP.Core.Modelos.Modulos.Compra.Estadisticas {
    public class CompraPendientePago {
        public long IdCompra { get; set; }
        public string CodigoCompra { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int PagosPendientesConfirmacion { get; set; }
        public string CodigoProveedor { get; set; } = string.Empty;
        public string NombreProveedor { get; set; } = string.Empty;
        public string EstadoCompra { get; set; } = string.Empty;

        // Cuántos días lleva sin pagarse desde la fecha de orden
        public int DiasDesdeOrden => (DateTime.Today - Fecha.Date).Days;

        public string Urgencia => DiasDesdeOrden switch {
            >= 60 => "Crítica",
            >= 30 => "Alta",
            >= 15 => "Media",
            _ => "Normal"
        };

        public override string ToString() {
            return CodigoCompra ?? string.Empty;
        }
    }
}

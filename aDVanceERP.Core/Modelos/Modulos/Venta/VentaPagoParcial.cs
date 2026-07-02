using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Venta {
    public class VentaPagoParcial : IEntidadBaseDatos {
        public long Id { get; set; }
        public long IdVenta { get; set; }
        public string NumeroFacturaTicket { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int CantidadPagos { get; set; }
        public string CodigoCliente { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;

        public decimal PorcentajePagado => ImporteTotal > 0 ? (TotalPagado / ImporteTotal) * 100 : 0;

        public string EstadoPago => PorcentajePagado switch {
            >= 75 => "Casi Completo",
            >= 50 => "Medio Pagado",
            >= 25 => "Poco Avance",
            _ => "Inicial"
        };

        // Implementación de IEntidadBaseDatos
        public long ObtenerId() => Id;
        public void EstablecerId(long id) => Id = id;
    }
}
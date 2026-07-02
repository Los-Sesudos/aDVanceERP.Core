using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Venta {
    public class VentaPendientePago : IEntidadBaseDatos, IComparable<VentaPendientePago> {
        public long Id { get; set; }
        public long IdVenta { get; set; }
        public string NumeroFacturaTicket { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int PagosPendientesConfirmacion { get; set; }
        public string CodigoCliente { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;

        // Implementación de IEntidadBaseDatos
        public long ObtenerId() => Id;
        public void EstablecerId(long id) => Id = id;

        public int CompareTo(VentaPendientePago? other) {
            return NumeroFacturaTicket.CompareTo(other?.NumeroFacturaTicket ?? string.Empty);
        }

        public override string ToString() {
            return NumeroFacturaTicket;
        }
    }
}
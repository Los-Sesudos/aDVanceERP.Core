using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Venta {
    public class VentaSinPago : IEntidadBaseDatos {
        public long Id { get; set; }
        public long IdVenta { get; set; }
        public string NumeroFacturaTicket { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal ImporteTotal { get; set; }
        public string CodigoCliente { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public int DiasSinPago { get; set; }

        public string Urgencia => DiasSinPago switch {
            >= 60 => "Crítica",
            >= 30 => "Alta",
            >= 15 => "Media",
            _ => "Normal"
        };

        public bool EsVencida => DiasSinPago > 30;

        // Implementación de IEntidadBaseDatos
        public long ObtenerId() => Id;
        public void EstablecerId(long id) => Id = id;
    }
}
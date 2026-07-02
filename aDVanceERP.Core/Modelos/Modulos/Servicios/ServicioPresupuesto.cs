using aDVanceERP.Core.Modelos.Comun.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioPresupuesto : IEntidadBaseDatos {
        public ServicioPresupuesto() {
            Codigo = string.Empty;
            FechaEmision = DateTime.Now;
            FechaValidez = null;
            Estado = "Borrador";
            Subtotal = 0.0m;
            DescuentoTotal = 0.0m;
            ImpuestoTotal = 0.0m;
            ImporteTotal = 0.0m;
            Observaciones = string.Empty;
            TerminosCondiciones = string.Empty;
            Activo = true;
        }

        public long Id { get; set; }
        public string Codigo { get; set; }
        public long IdCliente { get; set; }
        public long? IdCuentaUsuario { get; set; }
        public long? IdServicioTipo { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaValidez { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaRechazo { get; set; }
        public string Estado { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public decimal ImporteTotal { get; set; }
        public string? Observaciones { get; set; }
        public string? TerminosCondiciones { get; set; }
        public long? IdVentaOrigen { get; set; }
        public bool Activo { get; set; }

        public override string ToString() {
            return $"{Codigo} - {Estado}";
        }
    }

    public enum FiltroBusquedaServicioPresupuesto {
        Todos,
        Id,
        [Display(Name = "Código")]
        Codigo
    }
}
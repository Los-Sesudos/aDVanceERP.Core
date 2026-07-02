using aDVanceERP.Core.Modelos.Comun.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Compra {
    public sealed class Compra : IEntidadBaseDatos {
        public Compra() {
            Codigo = "N/A";
            Observaciones = null;
            Activo = true;
            Fecha = DateTime.Now;
            Subtotal = 0;
            DescuentoTotal = 0;
            ImpuestoTotal = 0;
            ImporteTotal = 0;
            IdMoneda = 1;
            TasaCambioAplicada = 1.0m;
        }

        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public long IdProveedor { get; set; }
        public long? IdCuentaUsuario { get; set; }
        public long IdAlmacenDestino { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public decimal ImporteTotal { get; set; }
        public EstadoCompraEnum EstadoCompra { get; set; }
        public string? Observaciones { get; set; }
        public bool Activo { get; set; }
        public int IdMoneda { get; set; }
        public decimal TasaCambioAplicada { get; set; }

        public override string ToString() {
            return Codigo ?? string.Empty;
        }
    }

    public enum FiltroBusquedaCompra {
        Todas,
        Id,
        [Display(Name = "Código")]
        Codigo,
        [Display(Name = "Id del Proveedor")]
        IdProveedor,
        Estado
    }
}
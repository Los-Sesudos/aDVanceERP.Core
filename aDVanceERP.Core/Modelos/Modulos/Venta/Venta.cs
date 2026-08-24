using aDVanceERP.Core.Modelos.Comun.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Venta {
    public sealed class Venta : IEntidadBaseDatos {
        public Venta() {
            Fecha = DateTime.Now;
            Activo = true;
            Subtotal = 0.0m;
            DescuentoTotal = 0.0m;
            ImpuestoTotal = 0.0m;
            ImporteTotal = 0.0m;
            EstadoVenta = EstadoVentaEnum.Pendiente;
            IdMoneda = 1;
            TasaCambioAplicada = 1.0m;
        }

        public long Id { get; set; }
        public long? IdPedido { get; set; }
        public long IdEmpleado { get; set; }
        public long? IdCliente { get; set; }
        public long? IdCuentaUsuario { get; set; }
        public long IdAlmacenOrigen { get; set; }
        public string? NumeroFacturaTicket { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public decimal ImporteTotal { get; set; }
        public string? CanalPagoPrincipal { get; set; }
        public EstadoVentaEnum EstadoVenta { get; set; }
        public string? ObservacionesVenta { get; set; }
        public bool Activo { get; set; }

        public int IdMoneda { get; set; }
        public decimal TasaCambioAplicada { get; set; }

        public override string ToString() {
            return NumeroFacturaTicket ?? string.Empty;
        }
    }

    public enum FiltroBusquedaVenta {
        Todas,
        Id,
        [Display(Name = "Nombre del empleado")]
        NombreEmpleado,
        [Display(Name = "Nombre del cliente")]
        NombreCliente,
        [Display(Name = "Número de factura/Ticket")]
        NumeroFactura,
        Estado,
        Inactivos
    }
}
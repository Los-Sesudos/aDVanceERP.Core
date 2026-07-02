using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioOrden : IEntidadBaseDatos {
        public ServicioOrden() {
            Codigo = string.Empty;
            FechaCreacion = DateTime.Now;
            Estado = "Pendiente";
            Prioridad = "Media";
            Subtotal = 0.0m;
            DescuentoTotal = 0.0m;
            ImpuestoTotal = 0.0m;
            ImporteTotal = 0.0m;
            Observaciones = string.Empty;
            NotasInternas = string.Empty;
            Activo = true;
        }

        public long Id { get; set; }
        public string Codigo { get; set; }
        public long IdCliente { get; set; }
        public long? IdCuentaUsuario { get; set; }
        public long? IdServicioTipo { get; set; }
        public long? IdServicioPresupuesto { get; set; }
        public long? IdVenta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaInicioProgramada { get; set; }
        public DateTime? FechaFinProgramada { get; set; }
        public DateTime? FechaInicioReal { get; set; }
        public DateTime? FechaFinReal { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public string? UbicacionServicio { get; set; }
        public string? ContactoCliente { get; set; }
        public string? TelefonoContacto { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public decimal ImporteTotal { get; set; }
        public string? Observaciones { get; set; }
        public string? NotasInternas { get; set; }
        public bool Activo { get; set; }
        public long? IdMovimientoIngreso { get; set; }

        public override string ToString() {
            return $"{Codigo} - {Estado}";
        }
    }

    public enum FiltroBusquedaServicioOrden {
        Todas,
        Id,
        Codigo,
        IdCliente,
        Estado,
        Prioridad,
        IdServicioTipo,
        FechaCreacion,
        Activas,
        Inactivas
    }
}
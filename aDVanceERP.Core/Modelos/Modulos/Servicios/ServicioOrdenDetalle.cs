using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioOrdenDetalle : IEntidadBaseDatos {
        public ServicioOrdenDetalle() {
            TipoItem = "Actividad";
            DescripcionPersonalizada = string.Empty;
            CantidadPlanificada = 1.0m;
            PrecioUnitario = 0.0m;
            DescuentoItem = 0.0m;
            ImpuestoItem = 0.0m;
            Orden = 0;
            Observaciones = string.Empty;
        }

        public long Id { get; set; }
        public long IdServicioOrden { get; set; }
        public string TipoItem { get; set; }
        public long? IdServicioActividad { get; set; }
        public long? IdProducto { get; set; }
        public long? IdAlmacenOrigen { get; set; }
        public string? DescripcionPersonalizada { get; set; }
        public decimal CantidadPlanificada { get; set; }
        public decimal? CantidadEjecutada { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoItem { get; set; }
        public decimal ImpuestoItem { get; set; }
        public int Orden { get; set; }
        public string? Observaciones { get; set; }
        public long? EjecutadoPor { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public long? IdMovimientoSalida { get; set; }

        // Propiedades calculadas
        public decimal Subtotal => CantidadPlanificada * PrecioUnitario;
        public decimal TotalItem => (CantidadPlanificada * PrecioUnitario) - DescuentoItem + ImpuestoItem;

        public override string ToString() {
            return DescripcionPersonalizada ?? $"{TipoItem} - {CantidadPlanificada} x {PrecioUnitario}";
        }
    }

    public enum FiltroBusquedaServicioOrdenDetalle {
        Todos,
        Id,
        IdServicioOrden,
        TipoItem,
        IdServicioActividad,
        IdProducto,
        Activos,
        Inactivos
    }
}
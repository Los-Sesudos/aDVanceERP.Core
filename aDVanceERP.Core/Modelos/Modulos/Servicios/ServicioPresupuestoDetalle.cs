using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioPresupuestoDetalle : IEntidadBaseDatos {
        public ServicioPresupuestoDetalle() {
            TipoItem = "Actividad";
            DescripcionPersonalizada = string.Empty;
            Cantidad = 1.0m;
            PrecioUnitario = 0.0m;
            DescuentoItem = 0.0m;
            ImpuestoItem = 0.0m;
            Orden = 0;
            Observaciones = string.Empty;
        }

        public long Id { get; set; }
        public long IdServicioPresupuesto { get; set; }
        public string TipoItem { get; set; }
        public long? IdServicioActividad { get; set; }
        public long? IdProducto { get; set; }
        public string? DescripcionPersonalizada { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoItem { get; set; }
        public decimal ImpuestoItem { get; set; }
        public int Orden { get; set; }
        public string? Observaciones { get; set; }

        // Propiedades calculadas (no se almacenan directamente en BD)
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal TotalItem => (Cantidad * PrecioUnitario) - DescuentoItem + ImpuestoItem;

        public override string ToString() {
            return DescripcionPersonalizada ?? $"{TipoItem} - {Cantidad} x {PrecioUnitario}";
        }
    }

    public enum FiltroBusquedaServicioPresupuestoDetalle {
        Todos,
        Id,
        IdPresupuesto,
        TipoItem
    }
}
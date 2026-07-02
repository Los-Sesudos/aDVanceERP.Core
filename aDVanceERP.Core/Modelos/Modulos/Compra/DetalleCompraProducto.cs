using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Compra {
    public sealed class DetalleCompraProducto : IEntidadBaseDatos {
        public DetalleCompraProducto() {
            IdPresentacion = 0;
            Cantidad = 0;
            CostoUnitario = 0;
            DescuentoItem = 0;
            ImpuestoAdicionalItem = 0;
        }

        public long Id { get; set; }
        public long IdCompra { get; set; }
        public long IdProducto { get; set; }
        public long? IdPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal DescuentoItem { get; set; }
        public decimal ImpuestoAdicionalItem { get; set; }

        // Propiedades calculadas
        public decimal Subtotal => Cantidad * CostoUnitario;
        public decimal Total => Subtotal - DescuentoItem + ImpuestoAdicionalItem;
    }

    public enum FiltroBusquedaDetalleCompra {
        Todos,
        Id,
        IdCompra,
        IdProducto
    }
}
using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Compra {
    public sealed class DetalleCompraProducto : IEntidadBaseDatos {
        public DetalleCompraProducto() {
            IdPresentacion = 0;
            Cantidad = 0;
            CostoUnitario = 0;
            DescuentoItem = 0;
            ImpuestoAdicionalItem = 0;
            TasaCambioAplicada = 1m;
            FactorPresentacion = 1m;
        }

        public long Id { get; set; }
        public long IdCompra { get; set; }
        public long IdProducto { get; set; }
        public long? IdPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal DescuentoItem { get; set; }
        public decimal ImpuestoAdicionalItem { get; set; }

        // --- Moneda y presentación por línea (V001) ---
        // CostoUnitario está en la moneda de la línea (IdMoneda) y por presentación.
        // CostoUnitarioBase = CostoUnitario × TasaCambioAplicada: en moneda base, por presentación, congelado con la tasa del día de la compra.
        /// <summary>Moneda en que se pactó el costo de la línea (null en registros anteriores a V001).</summary>
        public long? IdMoneda { get; set; }
        public decimal TasaCambioAplicada { get; set; }
        public decimal CostoUnitarioBase { get; set; }
        /// <summary>Unidades base que trae una presentación (1 para la unidad base). Se guarda para que editar la presentación después no altere compras ya hechas.</summary>
        public decimal FactorPresentacion { get; set; }

        // Propiedades calculadas
        public decimal Subtotal => Cantidad * CostoUnitario;
        public decimal Total => Subtotal - DescuentoItem + ImpuestoAdicionalItem;

        /// <summary>Unidades base de la línea (lo que entra al inventario en la recepción).</summary>
        public decimal CantidadBase => Cantidad * FactorPresentacion;
        /// <summary>Costo por unidad base, en moneda base (lo que alimenta el costo promedio).</summary>
        public decimal CostoBasePorUnidadBase => FactorPresentacion > 0 ? CostoUnitarioBase / FactorPresentacion : CostoUnitarioBase;
        /// <summary>Subtotal en moneda base (antes de descuento e impuesto).</summary>
        public decimal SubtotalBase => Cantidad * CostoUnitarioBase;
        /// <summary>Total en moneda base. Descuento e impuesto adicional son porcentajes (hoy no se usan: 0).</summary>
        public decimal TotalBase => SubtotalBase * (1m - DescuentoItem / 100m + ImpuestoAdicionalItem / 100m);
    }

    public enum FiltroBusquedaDetalleCompra {
        Todos,
        Id,
        IdCompra,
        IdProducto
    }
}
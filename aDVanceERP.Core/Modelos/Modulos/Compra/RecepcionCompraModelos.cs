namespace aDVanceERP.Core.Modelos.Modulos.Compra {
    /// <summary>Línea de una compra con lo pendiente de recibir (para la ventana de Recepción).</summary>
    public class LineaPendienteRecepcion {
        public long IdDetalleCompra { get; init; }
        public long IdProducto { get; init; }
        public long IdPresentacion { get; init; }
        public string NombreProducto { get; init; } = string.Empty;
        public string UnidadMedida { get; init; } = string.Empty;
        /// <summary>Unidades base por presentación, congelado al comprar.</summary>
        public decimal Factor { get; init; } = 1m;
        /// <summary>Todas las cantidades en unidades de la presentación de la línea.</summary>
        public decimal Pedido { get; init; }
        public decimal Recibido { get; init; }
        public decimal Devuelto { get; init; }
        public decimal Pendiente => Math.Max(0m, Pedido - Recibido);
        /// <summary>Costo por unidad base, moneda base (lo que se valorará en inventario).</summary>
        public decimal CostoBasePorUnidadBase { get; init; }
        public decimal ExistenciaActual { get; init; }
        public decimal CostoPromedioActual { get; init; }
    }

    /// <summary>Cantidad que el usuario confirma recibir de una línea (en unidades de su presentación).</summary>
    public class LineaRecepcion {
        public long IdDetalleCompra { get; init; }
        public decimal Cantidad { get; init; }
    }

    public class ResultadoLineaRecepcion {
        public long IdProducto { get; init; }
        public decimal CantidadBase { get; init; }
        public decimal CostoBase { get; init; }
        public decimal CostoPromedioAnterior { get; init; }
        public decimal CostoPromedioNuevo { get; init; }
        public decimal ExistenciaNueva { get; init; }
    }

    public class ResultadoRecepcion {
        public long IdRecepcion { get; init; }
        public string EstadoRecepcion { get; init; } = "Parcial";
        public IReadOnlyList<ResultadoLineaRecepcion> Lineas { get; init; } = [];
    }
}

namespace aDVanceERP.Core.Modelos.Modulos.Compra {
    public enum EstadoRecepcionCompraEnum {
        SinRecibir,
        Parcial,
        Completa
    }

    public enum EstadoPagoCompraEnum {
        SinPago,
        Parcial,
        Pagada
    }

    /// <summary>Totales del período mostrados en la cabecera de Gestión de compras. Todo en moneda base.</summary>
    public sealed class ResumenComprasPeriodo {
        public int Cantidad { get; set; }
        /// <summary>Valor de lo comprado y aún no recibido (pendiente × costo base de cada línea).</summary>
        public decimal PorRecibir { get; set; }
        /// <summary>Importe de las compras menos los pagos confirmados.</summary>
        public decimal PorPagar { get; set; }
        /// <summary>Importe total de las compras no anuladas.</summary>
        public decimal TotalInvertido { get; set; }
    }
}

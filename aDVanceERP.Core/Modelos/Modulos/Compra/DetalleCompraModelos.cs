namespace aDVanceERP.Core.Modelos.Modulos.Compra {
    /// <summary>
    /// Tipos de la bitácora de la compra (adv__compra_evento.tipo). Cada nombre coincide EXACTAMENTE con el valor
    /// del enum de la BD (V001): se guarda con ToString().
    /// </summary>
    public enum TipoEventoCompraEnum {
        Registro,
        Edicion,
        Recepcion,
        Pago,
        PagoAnulado,
        Devolucion,
        Anulacion
    }

    /// <summary>Cabecera de solo lectura de la ventana Detalle. Todos los importes en moneda base (CUP).</summary>
    public sealed class CabeceraDetalleCompra {
        public long Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public DateTime Fecha { get; init; }
        public string NombreProveedor { get; init; } = string.Empty;
        public string NombreAlmacenDestino { get; init; } = string.Empty;
        public string RegistradaPor { get; init; } = string.Empty;
        public string Observaciones { get; init; } = string.Empty;
        public EstadoCompraEnum EstadoCompra { get; init; }
        public EstadoRecepcionCompraEnum EstadoRecepcion { get; init; }
        public EstadoPagoCompraEnum EstadoPago { get; init; }
        /// <summary>Importe total en moneda base.</summary>
        public decimal ImporteTotal { get; init; }
        /// <summary>Código de la moneda de las líneas; "MIXTA" si hay varias; vacío si no hay líneas.</summary>
        public string MonedaCodigo { get; init; } = string.Empty;
        /// <summary>Importe en la moneda de las líneas. Nulo cuando la moneda es mixta.</summary>
        public decimal? ImporteEnMoneda { get; init; }
        /// <summary>De 0 a 100, por cantidad de las líneas en su presentación.</summary>
        public decimal PorcentajeRecibido { get; init; }
        /// <summary>Suma de pagos confirmados (Pago está en pausa: hoy normalmente 0).</summary>
        public decimal TotalPagadoConfirmado { get; init; }
    }

    /// <summary>Línea de la compra tal como se pactó, con lo recibido y lo devuelto. Cantidades en unidades de la presentación.</summary>
    public sealed class LineaDetalleCompra {
        public long IdDetalleCompra { get; init; }
        public string NombreProducto { get; init; } = string.Empty;
        /// <summary>Abreviatura de la unidad de la presentación comprada.</summary>
        public string Unidad { get; init; } = string.Empty;
        public decimal Cantidad { get; init; }
        public decimal Recibida { get; init; }
        public decimal Devuelta { get; init; }
        /// <summary>Código de la moneda en que se pactó el costo (vacío en registros anteriores a V001).</summary>
        public string MonedaCodigo { get; init; } = string.Empty;
        /// <summary>Costo por presentación en la moneda pactada.</summary>
        public decimal CostoUnitario { get; init; }
        public decimal Tasa { get; init; } = 1m;
        /// <summary>Costo por presentación en moneda base, congelado con la tasa del día de la compra.</summary>
        public decimal CostoUnitarioBase { get; init; }
        public decimal DescuentoPct { get; init; }
        public decimal ImpuestoPct { get; init; }

        public decimal Pendiente => Math.Max(0m, Cantidad - Recibida);
        public decimal SubtotalBase => Cantidad * CostoUnitarioBase * (1m - DescuentoPct / 100m + ImpuestoPct / 100m);
        public decimal ValorRecibidoBase => Recibida * CostoUnitarioBase;
        public decimal ValorPendienteBase => Pendiente * CostoUnitarioBase;
    }

    public sealed class RecepcionDetalleCompra {
        public long IdRecepcion { get; init; }
        public DateTime Fecha { get; init; }
        public string RecibidoPor { get; init; } = string.Empty;
        public int Lineas { get; init; }
        /// <summary>Valor recibido en moneda base (cantidad × factor × costo por unidad base).</summary>
        public decimal ValorBase { get; init; }
        public string Observaciones { get; init; } = string.Empty;
    }

    public sealed class PagoDetalleCompra {
        public long IdPago { get; init; }
        public DateTime? Fecha { get; init; }
        public string Canal { get; init; } = string.Empty;
        /// <summary>Monto tal como lo guarda adv__pago (C-35: sin moneda hasta que Pago guarde moneda/tasa).</summary>
        public decimal Monto { get; init; }
        public string Estado { get; init; } = string.Empty;
    }

    /// <summary>Fila de la bitácora (adv__compra_evento): solo se agrega, nunca se modifica ni se borra.</summary>
    public sealed class RegistroBitacoraCompra {
        public long Id { get; init; }
        public DateTime Fecha { get; init; }
        public TipoEventoCompraEnum Tipo { get; init; }
        public string Usuario { get; init; } = string.Empty;
        public string Motivo { get; init; } = string.Empty;
        public string Detalle { get; init; } = string.Empty;
    }
}

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

        // Ejes del ciclo de la compra (adv__compra.estado_recepcion / estado_pago). El eje de pago lo gobernará el módulo Pago.
        public EstadoRecepcionCompraEnum EstadoRecepcion { get; set; } = EstadoRecepcionCompraEnum.SinRecibir;
        public EstadoPagoCompraEnum EstadoPago { get; set; } = EstadoPagoCompraEnum.SinPago;

        // Solo lectura: se calculan en la consulta de búsqueda (no se persisten).
        public string NombreProveedor { get; set; } = string.Empty;
        /// <summary>Código de la moneda en que se pactaron las líneas; "MIXTA" si hay varias; vacío si no hay líneas.</summary>
        public string MonedaCodigo { get; set; } = string.Empty;
        /// <summary>Importe en la moneda de las líneas. Nulo cuando la moneda es mixta.</summary>
        public decimal? ImporteEnMoneda { get; set; }
        /// <summary>Fracción recibida de lo comprado, de 0 a 100 (por cantidad de líneas en su presentación).</summary>
        public decimal PorcentajeRecibido { get; set; }

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
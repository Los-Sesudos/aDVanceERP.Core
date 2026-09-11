using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Comun;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Caja {

    public sealed class CajaMovimiento : IEntidadBaseDatos {
        public CajaMovimiento() {
            Tipo = TipoMovimientoCajaEnum.EntradaManual;
            CanalPago = CanalPagoEnum.NA;
            Monto = 0m;
            FechaMovimiento = DateTime.Now;
        }

        public long Id { get; set; }

        public long IdTurno { get; set; }

        public long IdMoneda { get; set; } = 1;

        public long? IdVenta { get; set; }

        public long IdCuentaUsuario { get; set; }

        public TipoMovimientoCajaEnum Tipo { get; set; }

        public CanalPagoEnum CanalPago { get; set; }

        /// <summary>
        /// Positivo = entrada a la caja. Negativo = salida de la caja.
        /// </summary>
        public decimal Monto { get; set; }

        public string? Descripcion { get; set; }

        public DateTime FechaMovimiento { get; set; }

        public string? NumeroFactura { get; set; }

        public string? NombreUsuario { get; set; }

        /// <summary>
        /// Monto ya convertido a moneda base, congelado con la tasa vigente
        /// al momento del movimiento (0 si nunca se recalculó — filas antiguas).
        /// </summary>
        public decimal MontoMonedaBase { get; set; }

        /// <summary>
        /// Tasa aplicada al convertir Monto → MontoMonedaBase. 1 si es la moneda base.
        /// </summary>
        public decimal TasaCambioAplicada { get; set; } = 1m;

    }


    public enum TipoMovimientoCajaEnum {
        Venta,
        [Display(Name = "Devolución venta")]
        DevolucionVenta,
        [Display(Name = "Entrada manual")]
        EntradaManual,
        [Display(Name = "Salida manual")]
        SalidaManual,
        [Display(Name = "Ajuste arqueo")]
        AjusteArqueo
    }

    public enum FiltroBusquedaCajaMovimiento {
        Todos,
        IdTurno,
        Tipo,
        Canal,
        Fecha
    }
}
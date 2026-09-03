using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Comun;
using aDVanceERP.Core.Modelos.Modulos.Monedas;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Caja {

    public sealed class CajaMovimiento : IEntidadBaseDatos {

        public CajaMovimiento() {
            Tipo = TipoMovimientoCajaEnum.EntradaManual;
            CanalPago = CanalPagoEnum.NA;
            Monto = 0m;
            FechaMovimiento = DateTime.Now;
        }

        public CajaMovimiento(
            long id,
            long idTurno,
            TipoMovimientoCajaEnum tipo,
            CanalPagoEnum canalPago,
            long idMoneda,
            long? idVenta,
            decimal monto,
            string? descripcion,
            long idCuentaUsuario,
            DateTime fechaMovimiento) {

            Id = id;
            IdTurno = idTurno;
            Tipo = tipo;
            CanalPago = canalPago;
            IdMoneda = idMoneda;
            IdVenta = idVenta;
            Monto = monto;
            Descripcion = descripcion;
            IdCuentaUsuario = idCuentaUsuario;
            FechaMovimiento = fechaMovimiento;
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
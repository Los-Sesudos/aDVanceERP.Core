using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Caja {

    /// <summary>
    /// Representa una fila del conteo físico de efectivo: una denominación y su cantidad de piezas,
    /// en una moneda determinada (IdMoneda). El Subtotal (denominacion × cantidad) es columna
    /// STORED en BD — se asigna al leer, nunca al escribir.
    /// </summary>
    public sealed class CajaArqueo : IEntidadBaseDatos {
        public CajaArqueo() { }

        /// <summary>Constructor de compatibilidad: asume moneda base (id_moneda = 1).</summary>
        public CajaArqueo(long id, long idTurno, decimal denominacion, int cantidad)
            : this(id, idTurno, denominacion, cantidad, idMoneda: 1) { }

        public CajaArqueo(long id, long idTurno, decimal denominacion, int cantidad, long idMoneda) {
            Id = id;
            IdTurno = idTurno;
            Denominacion = denominacion;
            Cantidad = cantidad;
            IdMoneda = idMoneda;
            Subtotal = denominacion * cantidad;
        }

        public CajaArqueo(long id, long idTurno, decimal denominacion, int cantidad, long idMoneda, decimal subtotal) {
            Id = id;
            IdTurno = idTurno;
            Denominacion = denominacion;
            Cantidad = cantidad;
            IdMoneda = idMoneda;
            Subtotal = subtotal;
        }

        public long Id { get; set; }
        public long IdTurno { get; set; }

        /// <summary>Valor de billete o moneda. Ej: 500, 200, 100, 50, 20, 10, 5, 1, 0.50</summary>
        public decimal Denominacion { get; set; }

        /// <summary>Cantidad de piezas contadas.</summary>
        public int Cantidad { get; set; }

        /// <summary>id_moneda del catálogo adv__moneda. Default 1 = moneda base (CUP).</summary>
        public long IdMoneda { get; set; } = 1;

        /// <summary>Columna STORED en BD (Denominacion × Cantidad). Solo lectura desde la app.</summary>
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// DTO de solo lectura: resultado consolidado del arqueo de un turno para UNA moneda.
    /// Lo produce <see cref="RepoCajaTurno.ObtenerResumenArqueoPorMoneda"/>.
    /// </summary>
    public sealed class ResumenArqueo {
        public long IdTurno { get; set; }

        /// <summary>id_moneda a la que corresponde este resumen.</summary>
        public long IdMoneda { get; set; } = 1;

        /// <summary>Código de la moneda, ej. "CUP", "USD" — para mostrar en la UI sin otro join.</summary>
        public string CodigoMoneda { get; set; } = string.Empty;

        public List<CajaArqueo> Denominaciones { get; set; } = [];

        /// <summary>Total contado en la moneda propia (sin convertir).</summary>
        public decimal TotalContado { get; set; }

        /// <summary>Tasa aplicada para convertir a moneda base (1 si ya es la moneda base).</summary>
        public decimal TasaCambioAplicada { get; set; } = 1m;

        /// <summary>Total contado ya convertido a moneda base (TotalContado × TasaCambioAplicada).</summary>
        public decimal TotalContadoBase => TotalContado * TasaCambioAplicada;
    }

    /// <summary>
    /// DTO de solo lectura: totales de movimientos por canal para el cierre.
    /// Lo produce <see cref="RepoCajaMovimiento.ObtenerTotalesPorCanal"/>.
    /// </summary>
    public sealed class TotalesCierreCaja {
        public long IdTurno { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTransferencias { get; set; }
    }
}
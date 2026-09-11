using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Comun;

namespace aDVanceERP.Core.Modelos.Modulos.Caja {
    public sealed class CajaConciliacionMoneda : IEntidadBaseDatos {
        public long Id { get; set; }
        public long IdTurno { get; set; }
        public long IdMoneda { get; set; }
        public CanalPagoEnum CanalPago { get; set; }
        public decimal MontoCalculado { get; set; }
        public decimal MontoDeclarado { get; set; }
        public decimal Diferencia { get; set; } // columna STORED, solo lectura

        /// <summary>
        /// Tasa de cambio hacia la moneda base, congelada al momento del cierre
        /// (1 si IdMoneda ya es la moneda base). Con esto MontoCalculado/MontoDeclarado
        /// se pueden convertir a base de forma histórica, sin depender de la tasa
        /// vigente el día que se genera un reporte.
        /// </summary>
        public decimal TasaCambioAplicada { get; set; } = 1m;

        public decimal MontoCalculadoBase => MontoCalculado * TasaCambioAplicada;
        public decimal MontoDeclaradoBase => MontoDeclarado * TasaCambioAplicada;
        public decimal DiferenciaBase => MontoDeclaradoBase - MontoCalculadoBase;

        // Auxiliares de tupla
        public string? CodigoMoneda { get; set; }
    }
}
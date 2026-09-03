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

        // Auxiliares de tupla
        public string? CodigoMoneda { get; set; }
    }


}

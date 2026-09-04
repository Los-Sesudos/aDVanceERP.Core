using aDVanceERP.Core.Modelos.Modulos.Caja;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoConfirmacionCierreTurno {
        public required CajaTurno Turno { get; set; }
        public required List<CajaArqueo> ArqueoCaja { get; set; }
        public required List<CajaConciliacionMoneda> Conciliacion { get; set; }
        public string? Observaciones { get; set; }
    }
}

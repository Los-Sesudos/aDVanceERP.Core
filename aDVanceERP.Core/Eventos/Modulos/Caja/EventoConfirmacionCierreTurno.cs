using aDVanceERP.Core.Modelos.Modulos.Caja;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoConfirmacionCierreTurno {
        public CajaTurno? Turno { get; set; }
        public IEnumerable<CajaArqueo> ArqueoCaja { get; set; } = null!;
        public List<CajaConciliacionMoneda> Conciliacion { get; set; } = null!;
        public string? Observaciones { get; set; }

        public List<TotalesCierreCaja>? TotalesCierreCaja { get; set; }
        public decimal MontoEfectivoDeclarado { get; set; }
        public decimal MontoTransferenciasDeclarado { get; set; }
        public decimal DiferenciaEfectivo {  get; set; }
    }
}

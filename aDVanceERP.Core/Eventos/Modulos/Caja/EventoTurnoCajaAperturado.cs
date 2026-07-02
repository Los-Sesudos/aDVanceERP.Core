using aDVanceERP.Core.Modelos.Modulos.Caja;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoTurnoCajaAperturado {
        public CajaTurno Turno { get; set; } = null!;
        public Modelos.Modulos.Venta.Venta Venta { get; set; } = null!;
    }
}

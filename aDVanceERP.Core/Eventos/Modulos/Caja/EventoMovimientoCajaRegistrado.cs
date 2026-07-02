using aDVanceERP.Core.Modelos.Modulos.Caja;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoMovimientoCajaRegistrado {
        public CajaMovimiento Movimiento { get; set; } = null!;
    }
}

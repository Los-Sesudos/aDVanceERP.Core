using aDVanceERP.Core.Modelos.Modulos.Caja;
using aDVanceERP.Core.Modelos.Modulos.Inventario;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoMostrarVistaRegistroMovimientoCaja {
        public CajaTurno Turno { get; set; } = null!;
        public Almacen? Almacen { get; set; }
    }
}

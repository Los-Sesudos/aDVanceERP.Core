

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoMostrarVistaAperturaTurno {
        public Modelos.Modulos.Venta.Venta Venta { get; set; } = null!;
        public long IdAlmacen { get; set; }
        public Modelos.Modulos.Compra.Compra Compra { get; set; }
    }
}

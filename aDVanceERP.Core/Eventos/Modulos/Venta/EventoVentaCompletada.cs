using aDVanceERP.Core.Modelos.Modulos.Comun;
using aDVanceERP.Core.Modelos.Modulos.Venta;

namespace aDVanceERP.Core.Eventos.Modulos.Venta {
    public class EventoVentaCompletada {
        public Modelos.Modulos.Venta.Venta? Venta { get; set; }
        public List<DetalleVentaProducto> Detalles { get; set; } = new List<DetalleVentaProducto>();
    }
}

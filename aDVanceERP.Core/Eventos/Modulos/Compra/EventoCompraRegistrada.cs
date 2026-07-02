
using aDVanceERP.Core.Modelos.Modulos.Compra;

namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    public class EventoCompraRegistrada {
        public Modelos.Modulos.Compra.Compra Compra { get; set; } = null!;
        public IEnumerable<DetalleCompraProducto> Detalles { get; set; } = new List<DetalleCompraProducto>();
        public long IdAlmacenDestino { get; set; } = 0;
    }
}

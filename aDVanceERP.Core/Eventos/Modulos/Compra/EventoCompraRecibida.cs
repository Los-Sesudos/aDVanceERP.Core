using aDVanceERP.Core.Modelos.Modulos.Compra;

namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    public class EventoCompraRecibida {
        public Modelos.Modulos.Compra.Compra Compra { get; init; } = null!;
        public IEnumerable<DetalleCompraProducto> Detalles { get; init; } = new List<DetalleCompraProducto>();
        public long IdAlmacenDestino { get; init; } = 0;
    }
}

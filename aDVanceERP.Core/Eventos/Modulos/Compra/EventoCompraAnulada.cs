using aDVanceERP.Core.Modelos.Modulos.Compra;

namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    public class EventoCompraAnulada {
        public Modelos.Modulos.Compra.Compra Compra { get; init; } = null!;
        public IEnumerable<DetalleCompraProducto> Detalles { get; init; } = new List<DetalleCompraProducto>();
        public long IdAlmacenOrigen { get; init; } = 0;
    }
}

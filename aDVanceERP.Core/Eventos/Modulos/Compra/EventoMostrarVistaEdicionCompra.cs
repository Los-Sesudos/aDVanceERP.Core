using aDVanceERP.Core.Modelos.Modulos.Compra;

namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    public class EventoMostrarVistaEdicionCompra {
        public Modelos.Modulos.Compra.Compra Compra { get; init; } = null!;
        public IEnumerable<DetalleCompraProducto> Detalles { get; init; } = new List<DetalleCompraProducto>();
    }
}

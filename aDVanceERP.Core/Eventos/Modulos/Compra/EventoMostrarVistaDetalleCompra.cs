namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    /// <summary>Abre la ventana de Detalle (solo lectura: líneas, recepciones, pagos y bitácora) de una compra.</summary>
    public class EventoMostrarVistaDetalleCompra {
        public long IdCompra { get; init; }
    }
}

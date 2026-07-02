using aDVanceERP.Core.Modelos.Modulos.Comun;

namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    public class EventoPagoCompraRegistrado {
        public Pago? Pago { get; set; }
        public DetallePagoTransferencia? DetallePagoTransferencia { get; set; }
    }
}

using aDVanceERP.Core.Modelos.Modulos.Comun;

namespace aDVanceERP.Core.Eventos.Modulos.Venta {
    public class EventoPagoVentaRegistrado {
        public Pago? Pago { get; set; }
        public DetallePagoTransferencia? DetallePagoTransferencia { get; set; }
    }
}

namespace aDVanceERP.Core.Modelos.Modulos.Venta.Estadisticas {
    public class VentasPorMetodoPago {
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public int Cantidad { get; set; }
    }
}

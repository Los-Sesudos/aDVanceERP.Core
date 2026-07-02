using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Inventario {
    public class KardexValorizado : IEntidadBaseDatos {
        public long Id { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string NombreAlmacen { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public decimal CantidadAnterior { get; set; }
        public decimal CantidadNueva { get; set; }
        public decimal Movimiento { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal CostoPromedio { get; set; }
        public decimal SaldoValorizado { get; set; }
    }

    public enum FiltroBusquedaKardexValorizado {
        Todos
    }
}

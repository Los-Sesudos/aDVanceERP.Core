using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioGarantia : IEntidadBaseDatos {
        public ServicioGarantia() {
            Codigo = string.Empty;
            FechaInicio = DateTime.Now;
            Estado = "Activa";
            DescripcionCobertura = string.Empty;
            Observaciones = string.Empty;
        }

        public long Id { get; set; }
        public long IdServicioOrden { get; set; }
        public string Codigo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasGarantia { get; set; }
        public string Estado { get; set; }
        public string? DescripcionCobertura { get; set; }
        public string? Observaciones { get; set; }
        public long? IdServicioOrdenReclamo { get; set; }

        public override string ToString() {
            return $"{Codigo} - {Estado}";
        }
    }
}
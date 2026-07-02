using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioActividad : IEntidadBaseDatos {
        public ServicioActividad() {
            Codigo = string.Empty;
            Nombre = string.Empty;
            Descripcion = string.Empty;
            UnidadMedida = "Servicio";
            PrecioBase = 0.0m;
            TiempoEstimadoMinutos = null;
            RequiereMateriales = false;
            Activo = true;
        }

        public long Id { get; set; }
        public long? IdServicioTipo { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioBase { get; set; }
        public string UnidadMedida { get; set; }
        public int? TiempoEstimadoMinutos { get; set; }
        public bool RequiereMateriales { get; set; }
        public bool Activo { get; set; }

        public override string ToString() {
            return $"{Codigo} - {Nombre}";
        }
    }

    public enum FiltroBusquedaServicioActividad {
        Todos,
        Id,
        Codigo,
        Nombre,
        IdServicioTipo,
        Activos,
        Inactivos
    }
}
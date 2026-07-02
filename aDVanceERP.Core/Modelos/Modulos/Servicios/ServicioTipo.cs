using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioTipo : IEntidadBaseDatos {
        public ServicioTipo() {
            Nombre = string.Empty;
            Descripcion = string.Empty;
            Activo = true;
        }

        public long Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }

        public override string ToString() {
            return Nombre;
        }
    }

    public enum FiltroBusquedaServicioTipo {
        Todos,
        Id,
        Nombre,
        Activos,
        Inactivos
    }
}
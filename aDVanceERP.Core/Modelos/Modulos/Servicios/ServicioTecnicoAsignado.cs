using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioTecnicoAsignado : IEntidadBaseDatos {
        public ServicioTecnicoAsignado() {
            Rol = "Secundario";
            HorasTrabajadas = 0.0m;
            FechaAsignacion = DateTime.Now;
            Activo = true;
        }

        public long Id { get; set; }
        public long IdServicioOrden { get; set; }
        public long IdEmpleado { get; set; }
        public long IdCuentaUsuarioAsigno { get; set; }
        public string Rol { get; set; }
        public decimal? HorasTrabajadas { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaDesasignacion { get; set; }
        public bool Activo { get; set; }

        public override string ToString() {
            return $"{Rol} - {IdEmpleado}";
        }
    }

    public enum FiltroBusquedaServicioTecnicoAsignado {
        Todos,
        Id,
        IdServicioOrden,
        IdEmpleado,
        Rol,
        Activos,
        Inactivos
    }
}
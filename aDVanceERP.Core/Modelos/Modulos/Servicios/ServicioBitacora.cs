using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class ServicioBitacora : IEntidadBaseDatos {
        public ServicioBitacora() {
            TipoEvento = "Nota";
            Descripcion = string.Empty;
            FechaRegistro = DateTime.Now;
        }

        public long Id { get; set; }
        public long IdServicioOrden { get; set; }
        public long? IdCuentaUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoEvento { get; set; }
        public string Descripcion { get; set; }
        public string? FotoUrl { get; set; }
        public decimal? UbicacionLatitud { get; set; }
        public decimal? UbicacionLongitud { get; set; }

        public override string ToString() {
            return $"{FechaRegistro:dd/MM/yyyy HH:mm} - {TipoEvento}";
        }
    }

    public enum FiltroBusquedaServicioBitacora {
        Todos,
        Id,
        IdServicioOrden,
        IdCuentaUsuario,
        TipoEvento,
        FechaRegistro
    }
}
using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Servicios {
    public sealed class TecnicoZona : IEntidadBaseDatos {
        public TecnicoZona() {
            ZonaNombre = string.Empty;
            ZonaDescripcion = string.Empty;
            Activo = true;
        }

        public long Id { get; set; }
        public long IdCuentaUsuario { get; set; }
        public long? IdServicioTipo { get; set; }
        public string ZonaNombre { get; set; }
        public string? ZonaDescripcion { get; set; }
        public bool Activo { get; set; }

        public override string ToString() {
            return ZonaNombre;
        }
    }
}
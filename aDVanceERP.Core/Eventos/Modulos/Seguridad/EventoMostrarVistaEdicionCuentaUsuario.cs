using aDVanceERP.Core.Modelos.Modulos.Maestros;
using aDVanceERP.Core.Modelos.Modulos.Seguridad;

namespace aDVanceERP.Core.Eventos.Modulos.Seguridad {
    public class EventoMostrarVistaEdicionCuentaUsuario {
        public CuentaUsuario CuentaUsuario { get; set; } = null!;
        public Rol? Rol { get; set; } = null!;
        public Persona? Persona { get; set; } = null!;
        public CorreoContacto? CorreoContacto { get; set; } = null!;
    }
}

using aDVanceERP.Core.Modelos.Modulos.Maestros;

namespace aDVanceERP.Core.Eventos.Modulos.Seguridad {
    public class EventoCuentaUsuarioEditada {
        public Persona Persona { get; set; } = null!;
        public CorreoContacto CorreoContacto { get; set; } = null!;
    }
}

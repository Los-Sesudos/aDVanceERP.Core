using aDVanceERP.Core.Modelos.Modulos.Maestros;

namespace aDVanceERP.Core.Eventos.Modulos.Empresa {
    public class EventoEmpresaEditada {
        public Persona Persona { get; set; } = null!;
        public TelefonoContacto TelefonoContacto { get; set; } = null!;
        public CorreoContacto CorreoContacto { get; set; } = null!;
    }
}

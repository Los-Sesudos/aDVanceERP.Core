using aDVanceERP.Core.Modelos.Modulos.Maestros;

namespace aDVanceERP.Core.Eventos.Modulos.Empresa {
    public class EventoEmpresaRegistrada {
        public Modelos.Modulos.Empresas.Empresa Empresa { get; set; } = null!;
        public Persona Persona { get; set; } = null!;
        public TelefonoContacto TelefonoContacto { get; set;} = null!;
        public CorreoContacto CorreoContacto { get; set;} = null!;
    }
}

using aDVanceERP.Core.Modelos.Modulos.Maestros;

namespace aDVanceERP.Core.Eventos.Modulos.Empresa {
    public class EventoMostrarVistaEdicionEmpresa {
        public Modelos.Modulos.Empresas.Empresa Empresa { get; set; } = null!;
        public Persona Persona { get; set; } = null!;
        public TelefonoContacto Telefono { get; set; } = null!;
        public CorreoContacto CorreoElectronico { get; set; } = null!;
    }
}

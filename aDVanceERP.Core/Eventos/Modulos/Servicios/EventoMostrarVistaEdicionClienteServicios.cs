using aDVanceERP.Core.Modelos.Modulos.Comun;
using aDVanceERP.Core.Modelos.Modulos.Maestros;

namespace aDVanceERP.Core.Eventos.Modulos.Servicios {
    public class EventoMostrarVistaEdicionClienteServicios {
        public Cliente Cliente { get; set; } = null!;
        public Persona Persona { get; set; } = null!;
        public IEnumerable<TelefonoContacto> Telefonos { get; set; } = null!;
        public IEnumerable<CorreoContacto> DireccionesCorreo { get; set; } = null!;
    }
}

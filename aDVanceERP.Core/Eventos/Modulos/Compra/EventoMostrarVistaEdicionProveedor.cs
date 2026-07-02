using aDVanceERP.Core.Modelos.Modulos.Compra;
using aDVanceERP.Core.Modelos.Modulos.Maestros;

namespace aDVanceERP.Core.Eventos.Modulos.Compra {
    public class EventoMostrarVistaEdicionProveedor {
        public Proveedor Proveedor { get; set; } = null!;
        public Persona Persona { get; set; } = null!;
        public IEnumerable<TelefonoContacto> Telefonos { get; set; } = null!;
        public IEnumerable<CorreoContacto> DireccionesCorreo { get; set; } = null!;
    }
}

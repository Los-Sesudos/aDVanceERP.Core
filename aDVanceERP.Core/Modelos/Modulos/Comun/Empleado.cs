using aDVanceERP.Core.Modelos.Comun.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Comun {
    public class Empleado : IEntidadBaseDatos {
        public long Id { get; set; }
        public long IdPersona { get; set; }
    }

    public enum FiltroBusquedaEmpleado{
        Todos,
        Id,
        [Display(Name = "Nombre completo")]
        NombreCompleto,
        [Display(Name = "Número de documento")]
        NumeroDocumento
    }
}

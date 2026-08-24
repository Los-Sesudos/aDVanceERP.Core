using aDVanceERP.Core.Modelos.Comun.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Maestros {
    public class Persona : IEntidadBaseDatos, IComparable<Persona> {
        public Persona() {
            NombreCompleto = "N/A";
            TipoDocumento = TipoDocumentoEnum.CI;
            NumeroDocumento = "N/A";
            FechaRegistro = DateTime.Now;
            Activo = true;
        }

        public Persona(long id, string nombreCompleto, TipoDocumentoEnum tipoDocumento, string numeroDocumento, string? direccionPrincipal, DateTime fechaRegistro, bool activo) {
            Id = id;
            NombreCompleto = nombreCompleto;
            TipoDocumento = tipoDocumento;
            NumeroDocumento = numeroDocumento;
            DireccionPrincipal = direccionPrincipal;
            FechaRegistro = fechaRegistro;
            Activo = activo;
        }

        public long Id { get; set; }
        public string NombreCompleto { get; set; }
        public TipoDocumentoEnum TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string? DireccionPrincipal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }

        public int CompareTo(Persona? other) {
            return NombreCompleto
                .ToLower(System.Globalization.CultureInfo.CurrentCulture)
                .CompareTo(other?
                    .NombreCompleto
                    .ToLower(System.Globalization.CultureInfo.CurrentCulture));
        }

        public override string ToString() {
            return NombreCompleto;
        }
    }

    public enum FiltroBusquedaPersona {
        Todos,
        Id,
        [Display(Name = "Nombre completo")]
        NombreCompleto,
        [Display(Name = "Número de documento")]
        NumeroDocumento
    }
}

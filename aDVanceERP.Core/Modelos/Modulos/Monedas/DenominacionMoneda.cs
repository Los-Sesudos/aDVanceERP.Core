using aDVanceERP.Core.Modelos.Comun.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Monedas {
    public class DenominacionMoneda : IEntidadBaseDatos {
        public long Id { get; set; }
        public long IdMoneda { get; set; }
        public decimal Valor { get; set; }
    }

    public enum FiltroBusquedaDenominacionMoneda {
        Id,
        [Display(Name = "Id de moneda")]
        IdMoneda
    }
}

using System.ComponentModel.DataAnnotations;

namespace aDVanceERP.Core.Modelos.Modulos.Comun {
    public enum CanalPagoEnum {
        Efectivo,
        [Display(Name = "Transferencia")]
        TransferenciaBancaria,
        Mixto,
        [Display(Name = "N/A")]
        NA
    }
}

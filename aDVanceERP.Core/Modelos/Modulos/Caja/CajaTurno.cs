using aDVanceERP.Core.Modelos.Comun.Interfaces;

namespace aDVanceERP.Core.Modelos.Modulos.Caja {

    public sealed class CajaTurno : IEntidadBaseDatos {
        public CajaTurno() {
            Codigo = string.Empty;
            FechaApertura = DateTime.Now;
            MontoApertura = 0m;
            Estado = EstadoCajaTurnoEnum.Abierto;
        }

        public CajaTurno(
            long id,
            string codigo,
            long idAlmacen,
            long idCuentaApertura,
            long? idCuentaCierre,
            DateTime fechaApertura,
            DateTime? fechaCierre,
            decimal montoApertura,
            decimal? montoEfectivoCalculado,
            decimal? montoEfectivoDeclarado,
            decimal? montoTransferenciasCalculado,
            decimal? montoTransferenciasDeclarado,
            EstadoCajaTurnoEnum estado,
            string? observacionesApertura,
            string? observacionesCierre) {
            Id = id;
            Codigo = codigo;
            IdAlmacen = idAlmacen;
            IdCuentaApertura = idCuentaApertura;
            IdCuentaCierre = idCuentaCierre;
            FechaApertura = fechaApertura;
            FechaCierre = fechaCierre;
            MontoApertura = montoApertura;
            MontoEfectivoCalculado = montoEfectivoCalculado;
            MontoEfectivoDeclarado = montoEfectivoDeclarado;
            MontoTransferenciasCalculado = montoTransferenciasCalculado;
            MontoTransferenciasDeclarado = montoTransferenciasDeclarado;
            Estado = estado;
            ObservacionesApertura = observacionesApertura;
            ObservacionesCierre = observacionesCierre;
        }

        public long Id { get; set; }
        public string Codigo { get; set; }
        public long IdAlmacen { get; set; }
        public long IdCuentaApertura { get; set; }
        public long? IdCuentaCierre { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoApertura { get; set; }
        public decimal? MontoEfectivoCalculado { get; set; }
        public decimal? MontoEfectivoDeclarado { get; set; }
        public decimal? DiferenciaEfectivo { get; set; }
        public decimal? MontoTransferenciasCalculado { get; set; }
        public decimal? MontoTransferenciasDeclarado { get; set; }
        public decimal? DiferenciaTransferencias { get; set; }
        public EstadoCajaTurnoEnum Estado { get; set; }
        public string? ObservacionesApertura { get; set; }
        public string? ObservacionesCierre { get; set; }
        public string? NombreAlmacen { get; set; }
        public string? NombreUsuarioApertura { get; set; }
    }

    public enum EstadoCajaTurnoEnum {
        Abierto,
        Cerrado,
        Anulado
    }

    public enum FiltroBusquedaCajaTurno {
        Todos,
        Id,
        Codigo,
        Estado
    }
}
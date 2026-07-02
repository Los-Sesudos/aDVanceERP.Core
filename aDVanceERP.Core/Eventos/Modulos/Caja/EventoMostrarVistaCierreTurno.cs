using aDVanceERP.Core.Modelos.Modulos.Caja;
using aDVanceERP.Core.Repositorios.Modulos.Caja;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoMostrarVistaCierreTurno {
        public string CodigoTurno { get; set; } = string.Empty;
        public long IdAlmacen { get; set; }
        public CajaTurno Turno { 
            get => RepoCajaTurno.Instancia
                .Buscar(FiltroBusquedaCajaTurno.Codigo, CodigoTurno)
                .resultadosBusqueda
                .FirstOrDefault()
                .entidadBase;
        }
        public TotalesCierreCaja Totales {
            get => RepoCajaMovimiento.Instancia.ObtenerTotalesPorCanal(Turno.Id);
        }
    }
}

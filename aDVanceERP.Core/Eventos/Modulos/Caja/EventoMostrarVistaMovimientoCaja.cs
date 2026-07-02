using aDVanceERP.Core.Modelos.Modulos.Caja;
using aDVanceERP.Core.Modelos.Modulos.Inventario;
using aDVanceERP.Core.Repositorios.Modulos.Caja;
using aDVanceERP.Core.Repositorios.Modulos.Inventario;

namespace aDVanceERP.Core.Eventos.Modulos.Caja {
    public class EventoMostrarVistaMovimientoCaja {
        public string CodigoTurno { get; set; } = string.Empty;
        public long IdAlmacen { get; set; }
        public CajaTurno? Turno {
            get => RepoCajaTurno.Instancia
                .Buscar(FiltroBusquedaCajaTurno.Codigo, CodigoTurno)
                .resultadosBusqueda
                .FirstOrDefault()
                .entidadBase;
        }
        public Almacen? Almacen {
            get => RepoAlmacen.Instancia
                .ObtenerPorId(IdAlmacen);
        }
    }
}

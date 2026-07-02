using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Inventario;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Inventario {
    public class RepoKardexValorizado : RepoEntidadBaseDatos<KardexValorizado, FiltroBusquedaKardexValorizado> {
        public RepoKardexValorizado() : base("v_kardex_valorizado", "") { }

        protected override string GenerarComandoAdicionar(KardexValorizado entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            throw new NotImplementedException();
        }

        protected override string GenerarComandoEditar(KardexValorizado entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            throw new NotImplementedException();
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            throw new NotImplementedException();
        }

        protected override string GenerarComandoObtener(FiltroBusquedaKardexValorizado filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var fechaDesde = criteriosBusqueda.Length == 3 ? criteriosBusqueda[0] : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = criteriosBusqueda.Length == 3 ? criteriosBusqueda[1] : DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var criterio = criteriosBusqueda.Length == 3 ? criteriosBusqueda[2] : criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;

            var consulta = $"""
                SELECT * FROM v_kardex_valorizado
                WHERE producto = @producto
                {(criteriosBusqueda.Length == 3 ? "AND fecha_registro >= @fecha_desde AND fecha_registro <= @fecha_hasta" : string.Empty)}
                """;
            parametros = new Dictionary<string, object>() {
                { "@producto" , criterio }
            };

            if (criteriosBusqueda.Length == 3) {
                parametros.Add("@fecha_desde", DateTime.Parse(fechaDesde).ToString("yyyy-MM-dd 00:00:00"));
                parametros.Add("@fecha_hasta", DateTime.Parse(fechaHasta).ToString("yyyy-MM-dd 23:59:59"));
            }

            return consulta;
        }

        protected override (KardexValorizado, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            return (new KardexValorizado {
                Id = 0,
                NombreProducto = Convert.ToString(lector["producto"]) ?? string.Empty,
                NombreAlmacen = Convert.ToString(lector["almacen"]) ?? string.Empty,
                FechaRegistro = Convert.ToDateTime(lector["fecha_registro"]),
                CantidadAnterior = Convert.ToDecimal(lector["cantidad_anterior"]),
                CantidadNueva = Convert.ToDecimal(lector["cantidad_nueva"]),
                Movimiento = Convert.ToDecimal(lector["movimiento"]),
                TipoMovimiento = Convert.ToString(lector["tipo"]) ?? string.Empty,
                CostoPromedio = Convert.ToDecimal(lector["costo_promedio"]),
                SaldoValorizado = Convert.ToDecimal(lector["saldo_valorizado"])
            }, new List<IEntidadBaseDatos>());
        }
    }
}

using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Compra;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Compra {
    public class RepoDetalleCompraProducto : RepoEntidadBaseDatos<DetalleCompraProducto, FiltroBusquedaDetalleCompra> {
        public RepoDetalleCompraProducto() : base("adv__detalle_compra_producto", "id_detalle_compra_producto") {
        }

        protected override string GenerarComandoAdicionar(DetalleCompraProducto entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = $"""
                INSERT INTO adv__detalle_compra_producto (
                    id_compra,
                    id_producto,
                    id_presentacion,
                    cantidad,
                    costo_unitario,
                    descuento_item,
                    impuesto_adicional_item
                ) VALUES (
                    @id_compra,
                    @id_producto,
                    @id_presentacion,
                    @cantidad,
                    @costo_unitario,
                    @descuento_item,
                    @impuesto_adicional_item
                )
                """;

            parametros = new Dictionary<string, object> {
                { "@id_compra", entidad.IdCompra },
                { "@id_producto", entidad.IdProducto },
                { "@id_presentacion", entidad.IdPresentacion.HasValue ? entidad.IdPresentacion.Value : DBNull.Value },
                { "@cantidad", entidad.Cantidad },
                { "@costo_unitario", entidad.CostoUnitario },
                { "@descuento_item", entidad.DescuentoItem },
                { "@impuesto_adicional_item", entidad.ImpuestoAdicionalItem }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(DetalleCompraProducto entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = $"""
                UPDATE adv__detalle_compra_producto 
                SET 
                    id_compra = @id_compra,
                    id_producto = @id_producto,
                    id_presentacion = @id_presentacion,
                    cantidad = @cantidad,
                    costo_unitario = @costo_unitario,
                    descuento_item = @descuento_item,
                    impuesto_adicional_item = @impuesto_adicional_item
                WHERE id_detalle_compra_producto = @id_detalle
                """;

            parametros = new Dictionary<string, object> {
                { "@id_compra", entidad.IdCompra },
                { "@id_producto", entidad.IdProducto },
                { "@id_presentacion", entidad.IdPresentacion.HasValue ? entidad.IdPresentacion.Value : DBNull.Value },
                { "@cantidad", entidad.Cantidad },
                { "@costo_unitario", entidad.CostoUnitario },
                { "@descuento_item", entidad.DescuentoItem },
                { "@impuesto_adicional_item", entidad.ImpuestoAdicionalItem },
                { "@id_detalle", entidad.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            const string consulta = """
                DELETE FROM adv__detalle_compra_producto
                WHERE id_detalle_compra_producto = @id_detalle
                """;

            parametros = new Dictionary<string, object>
            {
                { "@id_detalle", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaDetalleCompra filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;
            var consultaComun = """
                SELECT d.*, p.nombre as nombre_producto, p.codigo as codigo_producto
                FROM adv__detalle_compra_producto d
                LEFT JOIN adv__producto p ON d.id_producto = p.id_producto
                """;

            var consulta = filtroBusqueda switch {
                FiltroBusquedaDetalleCompra.Id => $"""
                    {consultaComun}
                    WHERE d.id_detalle_compra_producto = @id_detalle
                    """,
                FiltroBusquedaDetalleCompra.IdCompra => $"""
                    {consultaComun}
                    WHERE d.id_compra = @id_compra
                    """,
                FiltroBusquedaDetalleCompra.IdProducto => $"""
                    {consultaComun}
                    WHERE d.id_producto = @id_producto
                    """,
                _ => consultaComun
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaDetalleCompra.Id => new Dictionary<string, object>
                {
                    { "@id_detalle", long.Parse(criterio) }
                },
                FiltroBusquedaDetalleCompra.IdCompra => new Dictionary<string, object>
                {
                    { "@id_compra", long.Parse(criterio) }
                },
                FiltroBusquedaDetalleCompra.IdProducto => new Dictionary<string, object>
                {
                    { "@id_producto", long.Parse(criterio) }
                },
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (DetalleCompraProducto, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            var detalle = new DetalleCompraProducto {
                Id = Convert.ToInt64(lector["id_detalle_compra_producto"]),
                IdCompra = Convert.ToInt64(lector["id_compra"]),
                IdProducto = Convert.ToInt64(lector["id_producto"]),
                Cantidad = Convert.ToDecimal(lector["cantidad"]),
                CostoUnitario = Convert.ToDecimal(lector["costo_unitario"]),
                DescuentoItem = Convert.ToDecimal(lector["descuento_item"]),
                ImpuestoAdicionalItem = Convert.ToDecimal(lector["impuesto_adicional_item"]),
                IdPresentacion = lector["id_presentacion"] != DBNull.Value ? Convert.ToInt64(lector["id_presentacion"]) : null
            };

            var entidadesExtra = new List<IEntidadBaseDatos>();

            return (detalle, entidadesExtra);
        }

        #region SINGLETON

        public static RepoDetalleCompraProducto Instancia { get; } = new RepoDetalleCompraProducto();

        #endregion

        #region UTILES

        public List<DetalleCompraProducto> ObtenerPorIdCompra(long idCompra) {
            var (_, resultados) = Buscar(FiltroBusquedaDetalleCompra.IdCompra, idCompra.ToString());
            return resultados.Select(r => r.entidadBase).ToList();
        }

        public bool EliminarDetallesPorCompra(long idCompra) {
            var consulta = $"""
                DELETE FROM adv__detalle_compra_producto
                WHERE id_compra = @id_compra;
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_compra", idCompra }
            };

            return ContextoBaseDatos.EjecutarComandoNoQuery(consulta, parametros) > 0;
        }

        #endregion
    }
}
using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Compra;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Compra {
    /// <summary>
    /// Lecturas de solo lectura para la ventana Detalle de la compra. SQL directo a propósito: RepoCompra.ObtenerPorId
    /// usa una caché de 5 minutos por proceso (C-13) y aquí se necesita el estado real tras cada recepción.
    /// Requiere la migración V001 (estado_recepcion, estado_pago, cantidad_recibida/devuelta, adv__compra_evento).
    /// </summary>
    public class RepoConsultaDetalleCompra {
        public static RepoConsultaDetalleCompra Instancia { get; } = new();

        public CabeceraDetalleCompra? ObtenerCabecera(long idCompra) {
            const string consulta = """
                SELECT c.id_compra, c.codigo, c.fecha, c.importe_total, c.estado_compra, c.estado_recepcion, c.estado_pago, c.observaciones,
                       prov.razon_social AS proveedor, al.nombre AS almacen, cu.nombre AS usuario,
                       (SELECT CASE WHEN COUNT(*) = 0 THEN NULL
                                    WHEN COUNT(DISTINCT COALESCE(d.id_moneda, c.id_moneda)) = 1 THEN MAX(m.codigo)
                                    ELSE 'MIXTA' END
                        FROM adv__detalle_compra_producto d
                        LEFT JOIN adv__moneda m ON m.id_moneda = COALESCE(d.id_moneda, c.id_moneda)
                        WHERE d.id_compra = c.id_compra) AS moneda_codigo,
                       (SELECT CASE WHEN COUNT(DISTINCT COALESCE(d.id_moneda, c.id_moneda)) = 1
                                    THEN SUM(d.cantidad * d.costo_unitario) END
                        FROM adv__detalle_compra_producto d
                        WHERE d.id_compra = c.id_compra) AS importe_moneda,
                       (SELECT COALESCE(SUM(LEAST(d.cantidad_recibida, d.cantidad)) * 100 / NULLIF(SUM(d.cantidad), 0), 0)
                        FROM adv__detalle_compra_producto d
                        WHERE d.id_compra = c.id_compra) AS porcentaje_recibido,
                       (SELECT COALESCE(SUM(p.monto_pagado), 0)
                        FROM adv__pago p
                        WHERE p.id_compra = c.id_compra AND p.estado_pago = 'Confirmado') AS pagado
                FROM adv__compra c
                LEFT JOIN adv__proveedor prov ON prov.id_proveedor = c.id_proveedor
                LEFT JOIN adv__almacen al ON al.id_almacen = c.id_almacen_destino
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario = c.id_cuenta_usuario
                WHERE c.id_compra = @id
                LIMIT 1;
                """;

            return Consultar(consulta, idCompra, l => new CabeceraDetalleCompra {
                Id = Convert.ToInt64(l["id_compra"], CultureInfo.InvariantCulture),
                Codigo = Txt(l["codigo"]),
                Fecha = Convert.ToDateTime(l["fecha"], CultureInfo.InvariantCulture),
                NombreProveedor = Txt(l["proveedor"]),
                NombreAlmacenDestino = Txt(l["almacen"]),
                RegistradaPor = Txt(l["usuario"]),
                Observaciones = Txt(l["observaciones"]),
                EstadoCompra = Enum.TryParse<EstadoCompraEnum>(Txt(l["estado_compra"]), out var ec) ? ec : EstadoCompraEnum.Pendiente,
                EstadoRecepcion = Enum.TryParse<EstadoRecepcionCompraEnum>(Txt(l["estado_recepcion"]), out var er) ? er : EstadoRecepcionCompraEnum.SinRecibir,
                EstadoPago = Enum.TryParse<EstadoPagoCompraEnum>(Txt(l["estado_pago"]), out var ep) ? ep : EstadoPagoCompraEnum.SinPago,
                ImporteTotal = Dec(l["importe_total"]),
                MonedaCodigo = Txt(l["moneda_codigo"]),
                ImporteEnMoneda = l["importe_moneda"] == DBNull.Value ? null : Dec(l["importe_moneda"]),
                PorcentajeRecibido = Dec(l["porcentaje_recibido"]),
                TotalPagadoConfirmado = Dec(l["pagado"])
            }).FirstOrDefault();
        }

        public List<LineaDetalleCompra> ObtenerLineas(long idCompra) {
            // Mismo criterio de unidad que RepoRecepcionCompra.ObtenerLineas: presentación si la hay, unidad base si no.
            const string consulta = """
                SELECT d.id_detalle_compra_producto, p.nombre AS producto, IFNULL(um.abreviatura, '-') AS unidad,
                       d.cantidad, d.cantidad_recibida, d.cantidad_devuelta,
                       COALESCE(m.codigo, '') AS moneda, d.costo_unitario, d.tasa_cambio_aplicada, d.costo_unitario_base,
                       d.descuento_item, d.impuesto_adicional_item
                FROM adv__detalle_compra_producto d
                INNER JOIN adv__producto p ON p.id_producto = d.id_producto
                LEFT JOIN adv__precio_presentacion pp ON pp.id_presentacion = d.id_presentacion
                LEFT JOIN adv__unidad_medida um ON um.id_unidad_medida = IF(d.id_presentacion IS NULL OR d.id_presentacion = 0, p.id_unidad_medida, pp.id_unidad_medida)
                LEFT JOIN adv__moneda m ON m.id_moneda = d.id_moneda
                WHERE d.id_compra = @id
                ORDER BY d.id_detalle_compra_producto;
                """;

            return Consultar(consulta, idCompra, l => new LineaDetalleCompra {
                IdDetalleCompra = Convert.ToInt64(l["id_detalle_compra_producto"], CultureInfo.InvariantCulture),
                NombreProducto = Txt(l["producto"]),
                Unidad = Txt(l["unidad"]),
                Cantidad = Dec(l["cantidad"]),
                Recibida = Dec(l["cantidad_recibida"]),
                Devuelta = Dec(l["cantidad_devuelta"]),
                MonedaCodigo = Txt(l["moneda"]),
                CostoUnitario = Dec(l["costo_unitario"]),
                Tasa = Dec(l["tasa_cambio_aplicada"], 1m),
                CostoUnitarioBase = Dec(l["costo_unitario_base"]),
                DescuentoPct = Dec(l["descuento_item"]),
                ImpuestoPct = Dec(l["impuesto_adicional_item"])
            });
        }

        public List<RecepcionDetalleCompra> ObtenerRecepciones(long idCompra) {
            const string consulta = """
                SELECT r.id_recepcion_compra, r.fecha_recepcion, cu.nombre AS usuario, r.observaciones,
                       COUNT(dr.id_detalle_recepcion_compra) AS lineas,
                       COALESCE(SUM(dr.cantidad_recibida * d.factor_presentacion * dr.costo_unitario_base), 0) AS valor
                FROM adv__recepcion_compra r
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario = r.recibido_por
                LEFT JOIN adv__detalle_recepcion_compra dr ON dr.id_recepcion_compra = r.id_recepcion_compra
                LEFT JOIN adv__detalle_compra_producto d ON d.id_detalle_compra_producto = dr.id_detalle_compra_producto
                WHERE r.id_compra = @id
                GROUP BY r.id_recepcion_compra, r.fecha_recepcion, cu.nombre, r.observaciones
                ORDER BY r.fecha_recepcion DESC, r.id_recepcion_compra DESC;
                """;

            return Consultar(consulta, idCompra, l => new RecepcionDetalleCompra {
                IdRecepcion = Convert.ToInt64(l["id_recepcion_compra"], CultureInfo.InvariantCulture),
                Fecha = Convert.ToDateTime(l["fecha_recepcion"], CultureInfo.InvariantCulture),
                RecibidoPor = Txt(l["usuario"]),
                Lineas = Convert.ToInt32(l["lineas"], CultureInfo.InvariantCulture),
                ValorBase = Dec(l["valor"]),
                Observaciones = Txt(l["observaciones"])
            });
        }

        public List<PagoDetalleCompra> ObtenerPagos(long idCompra) {
            const string consulta = """
                SELECT p.id_pago, p.fecha_pago, p.metodo_pago, p.monto_pagado, p.estado_pago
                FROM adv__pago p
                WHERE p.id_compra = @id
                ORDER BY p.id_pago DESC;
                """;

            return Consultar(consulta, idCompra, l => new PagoDetalleCompra {
                IdPago = Convert.ToInt64(l["id_pago"], CultureInfo.InvariantCulture),
                Fecha = l["fecha_pago"] == DBNull.Value ? null : Convert.ToDateTime(l["fecha_pago"], CultureInfo.InvariantCulture),
                Canal = Txt(l["metodo_pago"]),
                Monto = Dec(l["monto_pagado"]),
                Estado = Txt(l["estado_pago"])
            });
        }

        #region AUX

        /// <summary>Ejecuta la consulta y materializa todo (ToList): la conexión local solo se cierra al enumerar completo (C-12).</summary>
        private static List<T> Consultar<T>(string consulta, long idCompra, Func<MySqlDataReader, T> mapa) =>
            [.. ContextoBaseDatos
                .EjecutarConsulta<T>(consulta, new Dictionary<string, object> { { "@id", idCompra } },
                    lector => (mapa(lector), new List<IEntidadBaseDatos>()))
                .Select(r => r.entidadBase)];

        private static decimal Dec(object? valor, decimal porDefecto = 0m) =>
            valor == null || valor == DBNull.Value ? porDefecto : Convert.ToDecimal(valor, CultureInfo.InvariantCulture);

        private static string Txt(object? valor) =>
            valor == null || valor == DBNull.Value ? string.Empty : Convert.ToString(valor, CultureInfo.InvariantCulture) ?? string.Empty;

        #endregion
    }
}

using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Compra;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Compra {
    /// <summary>
    /// Bitácora de la compra (adv__compra_evento, V001). Solo inserciones: no hay Editar ni Eliminar a propósito.
    /// Punto de extensión: Registrar acepta la conexión y la transacción del llamador, de modo que Anular/Devolver
    /// escriban su evento dentro de la MISMA transacción que mueve el stock.
    /// </summary>
    public class RepoCompraEvento {
        public static RepoCompraEvento Instancia { get; } = new();

        private const int LongitudMotivo = 255;

        public long Registrar(long idCompra, TipoEventoCompraEnum tipo, long? idUsuario, string? motivo = null, string? detalle = null,
                              MySqlConnection? conexion = null, MySqlTransaction? transaccion = null) {
            const string consulta = """
                INSERT INTO adv__compra_evento (id_compra, tipo, id_cuenta_usuario, motivo, detalle)
                VALUES (@id_compra, @tipo, @usuario, @motivo, @detalle);
                """;

            var parametros = new Dictionary<string, object> {
                { "@id_compra", idCompra },
                { "@tipo", tipo.ToString() },
                { "@usuario", idUsuario is > 0 ? (object) idUsuario.Value : DBNull.Value },
                { "@motivo", string.IsNullOrWhiteSpace(motivo) ? DBNull.Value : (object) Recortar(motivo.Trim(), LongitudMotivo) },
                { "@detalle", string.IsNullOrWhiteSpace(detalle) ? DBNull.Value : (object) detalle }
            };

            return ContextoBaseDatos.EjecutarComandoInsert(consulta, parametros, conexion, transaccion);
        }

        /// <summary>Eventos de la compra, el más reciente primero.</summary>
        public List<RegistroBitacoraCompra> ObtenerPorCompra(long idCompra) {
            const string consulta = """
                SELECT e.id_compra_evento, e.fecha, e.tipo, e.motivo, e.detalle, cu.nombre AS usuario
                FROM adv__compra_evento e
                LEFT JOIN adv__cuenta_usuario cu ON cu.id_cuenta_usuario = e.id_cuenta_usuario
                WHERE e.id_compra = @id
                ORDER BY e.fecha DESC, e.id_compra_evento DESC;
                """;

            // ToList(): EjecutarConsulta es un iterador y la conexión solo se cierra al enumerar todo (C-12).
            return [.. ContextoBaseDatos
                .EjecutarConsulta<RegistroBitacoraCompra>(consulta, new Dictionary<string, object> { { "@id", idCompra } }, Mapear)
                .Select(r => r.entidadBase)];
        }

        private static (RegistroBitacoraCompra, List<IEntidadBaseDatos>) Mapear(MySqlDataReader lector) {
            var evento = new RegistroBitacoraCompra {
                Id = Convert.ToInt64(lector["id_compra_evento"], CultureInfo.InvariantCulture),
                Fecha = Convert.ToDateTime(lector["fecha"], CultureInfo.InvariantCulture),
                Tipo = Enum.TryParse<TipoEventoCompraEnum>(Convert.ToString(lector["tipo"]), out var tipo) ? tipo : TipoEventoCompraEnum.Edicion,
                Usuario = lector["usuario"] == DBNull.Value ? string.Empty : Convert.ToString(lector["usuario"]) ?? string.Empty,
                Motivo = lector["motivo"] == DBNull.Value ? string.Empty : Convert.ToString(lector["motivo"]) ?? string.Empty,
                Detalle = lector["detalle"] == DBNull.Value ? string.Empty : Convert.ToString(lector["detalle"]) ?? string.Empty
            };

            return (evento, new List<IEntidadBaseDatos>());
        }

        private static string Recortar(string texto, int maximo) => texto.Length <= maximo ? texto : texto[..maximo];
    }
}

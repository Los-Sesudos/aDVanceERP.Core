using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Monedas;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Monedas {
    public class RepoTasaCambio : RepoEntidadBaseDatos<TasaCambio, FiltroBusquedaTasaCambio> {
        public RepoTasaCambio() : base("adv__tasa_cambio", "id_tasa_cambio") { }

        protected override string GenerarComandoAdicionar(TasaCambio objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            parametros = new Dictionary<string, object> {
                { "@origen",          objeto.IdMonedaOrigen  },
                { "@destino",         objeto.IdMonedaDestino },
                { "@fecha",           objeto.Fecha.ToString("yyyy-MM-dd") },
                { "@tasa",            objeto.Tasa            },
                { "@fuente",          (object?)objeto.Fuente ?? DBNull.Value },
                { "@aplica_efectivo", objeto.AplicaEfectivo ? 1 : 0 }
            };

            return """
                INSERT INTO adv__tasa_cambio
                    (id_moneda_origen, id_moneda_destino, fecha, tasa, fuente, aplica_efectivo)
                VALUES
                    (@origen, @destino, @fecha, @tasa, @fuente, @aplica_efectivo);
                """;
        }

        protected override string GenerarComandoEditar(TasaCambio objeto, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            parametros = new Dictionary<string, object> {
                { "@id",              objeto.Id              },
                { "@tasa",            objeto.Tasa            },
                { "@fuente",          (object?)objeto.Fuente ?? DBNull.Value },
                { "@aplica_efectivo", objeto.AplicaEfectivo ? 1 : 0 }
            };

            return """
                UPDATE adv__tasa_cambio
                SET tasa = @tasa, fuente = @fuente, aplica_efectivo = @aplica_efectivo
                WHERE id_tasa_cambio = @id;
                """;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            parametros = new Dictionary<string, object> {
                { "@id", id }
            };

            return "DELETE FROM adv__tasa_cambio WHERE id_tasa_cambio = @id;";
        }

        protected override string GenerarComandoObtener(FiltroBusquedaTasaCambio filtroBusqueda, out Dictionary<string, object> parametros, params string[] criterios) {
            var c0 = criterios.Length > 0 ? criterios[0] : "0";
            var c1 = criterios.Length > 1 ? criterios[1] : "0";

            var consulta = filtroBusqueda switch {
                FiltroBusquedaTasaCambio.Id =>
                    "SELECT * FROM adv__tasa_cambio WHERE id_tasa_cambio = @id;",

                FiltroBusquedaTasaCambio.MonedaOrigenDestino => """
                    SELECT * FROM adv__tasa_cambio
                    WHERE id_moneda_origen = @origen AND id_moneda_destino = @destino
                    ORDER BY fecha DESC;
                    """,

                FiltroBusquedaTasaCambio.VigenteHoy => """
                    SELECT * FROM adv__tasa_cambio
                    WHERE id_moneda_origen  = @origen
                      AND id_moneda_destino = @destino
                      AND fecha <= CURDATE()
                    ORDER BY fecha DESC
                    LIMIT 1;
                    """,

                FiltroBusquedaTasaCambio.Fecha =>
                    "SELECT * FROM adv__tasa_cambio WHERE fecha = @fecha;",
                _ =>
                    "SELECT * FROM adv__tasa_cambio ORDER BY fecha DESC;"
            };

            parametros = filtroBusqueda switch {
                FiltroBusquedaTasaCambio.Id =>
                    new() { { "@id", Convert.ToInt64(c0) } },

                FiltroBusquedaTasaCambio.MonedaOrigenDestino or
                FiltroBusquedaTasaCambio.VigenteHoy =>
                    new() { { "@origen",  Convert.ToInt64(c0) },
                            { "@destino", Convert.ToInt64(c1) } },

                FiltroBusquedaTasaCambio.Fecha =>
                    new() { { "@fecha", c0 } },

                _ => new()
            };

            return consulta;
        }

        protected override (TasaCambio, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) =>
            (new TasaCambio() {
                Id = Convert.ToInt64(lector["id_tasa_cambio"]),
                IdMonedaOrigen = Convert.ToInt64(lector["id_moneda_origen"]),
                IdMonedaDestino = Convert.ToInt64(lector["id_moneda_destino"]),
                Fecha = Convert.ToDateTime(lector["fecha"]),
                Tasa = Convert.ToDecimal(lector["tasa"]),
                Fuente = lector["fuente"] != DBNull.Value ? Convert.ToString(lector["fuente"]) : null,
                IdCuentaUsuario = lector["id_cuenta_usuario"] != DBNull.Value ? Convert.ToInt64(lector["id_cuenta_usuario"]) : 0,
                FechaRegistro = Convert.ToDateTime(lector["fecha_registro"]),
                AplicaEfectivo = Convert.ToBoolean(lector["aplica_efectivo"])
            }, new List<IEntidadBaseDatos>());

        // ── SINGLETON ──────────────────────────────────────────────────────────
        public static RepoTasaCambio Instancia { get; } = new RepoTasaCambio();

        // ── UTILIDADES ─────────────────────────────────────────────────────────

        /// <summary>
        /// Obtiene la tasa vigente más reciente (≤ hoy) para el par origen → destino.
        /// Devuelve 1 si no existe ninguna tasa (monedas iguales o sin datos).
        /// </summary>
        public decimal ObtenerTasaVigente(long idMonedaOrigen, long idMonedaDestino) {
            return ObtenerTasaVigente(idMonedaOrigen, idMonedaDestino, DateTime.Today);
        }

        /// <summary>
        /// Igual que <see cref="ObtenerTasaVigente(long,long)"/> pero resuelta a una fecha
        /// específica en vez de CURDATE(). Es la que hay que usar siempre que se esté
        /// registrando o recalculando algo que ocurrió en una fecha distinta a hoy
        /// (ej. el fondo de apertura de un turno, o un backfill histórico) — así el
        /// monto_moneda_base que se guarda queda congelado con la tasa que realmente
        /// aplicaba ese día, y no se mueve si la tasa cambia después.
        /// </summary>
        public decimal ObtenerTasaVigente(long idMonedaOrigen, long idMonedaDestino, DateTime fecha) {
            if (idMonedaOrigen == idMonedaDestino)
                return 1m;

            var consulta = """
                SELECT tasa
                FROM adv__tasa_cambio
                WHERE id_moneda_origen  = @origen
                  AND id_moneda_destino = @destino
                  AND fecha <= @fecha
                ORDER BY fecha DESC
                LIMIT 1;
                """;

            var parametros = new Dictionary<string, object> {
                { "@origen",  idMonedaOrigen  },
                { "@destino", idMonedaDestino },
                { "@fecha",   fecha.Date }
            };

            var resultado = ContextoBaseDatos.EjecutarConsultaEscalar<object>(consulta, parametros);

            return resultado != null && resultado != DBNull.Value
                ? Convert.ToDecimal(resultado)
                : 1m;
        }

        /// <summary>
        /// Igual que <see cref="ObtenerTasaVigenteEfectivo(long,long)"/> pero a una fecha
        /// específica — la que corresponde usar al registrar un CajaMovimiento o un
        /// CajaArqueo, para congelar la tasa de efectivo del día del movimiento.
        /// </summary>
        public decimal ObtenerTasaVigenteEfectivo(long idMonedaOrigen, long idMonedaDestino, DateTime fecha) {
            if (idMonedaOrigen == idMonedaDestino)
                return 1m;

            var consulta = """
                SELECT tasa
                FROM adv__tasa_cambio
                WHERE id_moneda_origen  = @origen
                  AND id_moneda_destino = @destino
                  AND fecha <= @fecha
                  AND aplica_efectivo = 1
                ORDER BY fecha DESC
                LIMIT 1;
                """;

            var parametros = new Dictionary<string, object> {
                { "@origen",  idMonedaOrigen  },
                { "@destino", idMonedaDestino },
                { "@fecha",   fecha }
            };

            var resultado = ContextoBaseDatos.EjecutarConsultaEscalar<object>(consulta, parametros);

            return resultado != null && resultado != DBNull.Value
                ? Convert.ToDecimal(resultado)
                : 1m;
        }

        /// <summary>
        /// Convierte un monto a una fecha específica en vez de con la tasa de hoy.
        /// </summary>
        public decimal Convertir(decimal monto, long idMonedaOrigen, long idMonedaDestino, DateTime fecha) =>
            monto * ObtenerTasaVigente(idMonedaOrigen, idMonedaDestino, fecha);
    }
}
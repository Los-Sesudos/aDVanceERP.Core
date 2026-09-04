using aDVanceERP.Core.Infraestructura.Extensiones.BD;
using aDVanceERP.Core.Infraestructura.Globales;
using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Caja;
using aDVanceERP.Core.Modelos.Modulos.Comun;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Repositorios.Modulos.Caja {
    public class RepoCajaConciliacionMoneda : RepoEntidadBaseDatos<CajaConciliacionMoneda, FiltroBusquedaCajaConciliacionMoneda> {
        public RepoCajaConciliacionMoneda() : base("adv__caja_conciliacion_moneda", "id_conciliacion") { }

        public static RepoCajaConciliacionMoneda Instancia { get; } = new RepoCajaConciliacionMoneda();

        /// <summary>Reemplaza toda la conciliación de un turno en una sola transacción.</summary>
        public void GuardarConciliacion(long idTurno, List<CajaConciliacionMoneda> conciliacion) {
            using var conexion = new MySqlConnection(ContextoBaseDatos.Configuracion.ToStringConexion());
            conexion.Open();
            using var transaccion = conexion.BeginTransaction();

            try {
                using (var borrar = new MySqlCommand("DELETE FROM adv__caja_conciliacion_moneda WHERE id_turno = @id_turno;", conexion, transaccion)) {
                    borrar.Parameters.AddWithValue("@id_turno", idTurno);
                    borrar.ExecuteNonQuery();
                }

                const string insertar = """
                    INSERT INTO adv__caja_conciliacion_moneda
                        (id_turno, id_moneda, canal_pago, monto_calculado, monto_declarado)
                    VALUES
                        (@id_turno, @id_moneda, @canal_pago, @monto_calculado, @monto_declarado);
                    """;

                foreach (var fila in conciliacion) {
                    using var cmd = new MySqlCommand(insertar, conexion, transaccion);
                    cmd.Parameters.AddWithValue("@id_turno", idTurno);
                    cmd.Parameters.AddWithValue("@id_moneda", fila.IdMoneda);
                    cmd.Parameters.AddWithValue("@canal_pago", fila.CanalPago.ToString());
                    cmd.Parameters.AddWithValue("@monto_calculado", fila.MontoCalculado);
                    cmd.Parameters.AddWithValue("@monto_declarado", fila.MontoDeclarado);
                    cmd.ExecuteNonQuery();
                }

                transaccion.Commit();
            } catch {
                transaccion.Rollback();
                throw;
            }
        }

        public List<CajaConciliacionMoneda> ObtenerPorTurno(long idTurno) =>
            ObtenerTodos()
                .Select(r => r.entidadBase)
                .Where(c => c.IdTurno == idTurno)
                .ToList();

        protected override string GenerarComandoAdicionar(CajaConciliacionMoneda o, out Dictionary<string, object> p, params IEntidadBaseDatos[] extra) {
            p = new() { { "@id_turno", o.IdTurno }, { "@id_moneda", o.IdMoneda }, { "@canal_pago", o.CanalPago.ToString() },
                        { "@monto_calculado", o.MontoCalculado }, { "@monto_declarado", o.MontoDeclarado } };
            return "INSERT INTO adv__caja_conciliacion_moneda (id_turno, id_moneda, canal_pago, monto_calculado, monto_declarado) VALUES (@id_turno, @id_moneda, @canal_pago, @monto_calculado, @monto_declarado);";
        }

        protected override string GenerarComandoEditar(CajaConciliacionMoneda o, out Dictionary<string, object> p, params IEntidadBaseDatos[] extra) {
            p = new() { { "@id", o.Id }, { "@monto_calculado", o.MontoCalculado }, { "@monto_declarado", o.MontoDeclarado } };
            return "UPDATE adv__caja_conciliacion_moneda SET monto_calculado = @monto_calculado, monto_declarado = @monto_declarado WHERE id_conciliacion = @id;";
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> p) {
            p = new() { { "@id", id } };
            return "DELETE FROM adv__caja_conciliacion_moneda WHERE id_conciliacion = @id;";
        }

        protected override string GenerarComandoObtener(FiltroBusquedaCajaConciliacionMoneda filtro, out Dictionary<string, object> p, params string[] criterios) {
            p = filtro == FiltroBusquedaCajaConciliacionMoneda.IdTurno
                ? new() { { "@id_turno", Convert.ToInt64(criterios.FirstOrDefault() ?? "0") } }
                : new();

            return filtro == FiltroBusquedaCajaConciliacionMoneda.IdTurno
                ? "SELECT * FROM adv__caja_conciliacion_moneda WHERE id_turno = @id_turno;"
                : "SELECT * FROM adv__caja_conciliacion_moneda;";
        }

        protected override (CajaConciliacionMoneda, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader r) =>
            (new CajaConciliacionMoneda {
                Id = Convert.ToInt64(r["id_conciliacion"]),
                IdTurno = Convert.ToInt64(r["id_turno"]),
                IdMoneda = Convert.ToInt64(r["id_moneda"]),
                CanalPago = Enum.Parse<CanalPagoEnum>(Convert.ToString(r["canal_pago"]) ?? "Efectivo"),
                MontoCalculado = Convert.ToDecimal(r["monto_calculado"]),
                MontoDeclarado = Convert.ToDecimal(r["monto_declarado"]),
                Diferencia = Convert.ToDecimal(r["diferencia"])
            }, new List<IEntidadBaseDatos>());
    }

    public enum FiltroBusquedaCajaConciliacionMoneda { Todos, IdTurno }
}
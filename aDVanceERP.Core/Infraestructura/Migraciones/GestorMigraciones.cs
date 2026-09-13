using aDVanceERP.Core.Infraestructura.Extensiones.BD;
using aDVanceERP.Core.Infraestructura.Globales;

using MySql.Data.MySqlClient;

namespace aDVanceERP.Core.Infraestructura.Migraciones {
    public class GestorMigraciones {
        private readonly string _carpetaMigraciones;

        public GestorMigraciones(string carpetaMigraciones) {
            _carpetaMigraciones = carpetaMigraciones;
        }

        public void EjecutarPendientes() {
            //AsegurarTablaControl();

            //var aplicadas = ObtenerAplicadas();
            var archivos = Directory.GetFiles(_carpetaMigraciones, "V*__*.sql")
                .OrderBy(ExtraerVersion)
                .ToList();

            foreach (var archivo in archivos) {
                var version = ExtraerVersion(archivo);
                var contenido = File.ReadAllText(archivo);
                var checksum = CalcularChecksum(contenido);

                //if (aplicadas.TryGetValue(version, out var checksumAplicado)) {
                //    if (checksumAplicado != checksum) {
                //        throw new InvalidOperationException(
                //            $"La migración {version} ya fue aplicada, pero el archivo cambió después. " +
                //            "No se ejecuta nada más hasta que se resuelva manualmente.");
                //    }
                //    continue; // ya aplicada, sin cambios, se salta
                //}

                EjecutarMigracion(version, archivo, contenido, checksum);
            }
        }

        private void EjecutarMigracion(string version, string archivo, string contenido, string checksum) {
            using var conexion = new MySqlConnection(ContextoBaseDatos.Configuracion.ToStringConexion());
            conexion.Open();
            using var transaccion = conexion.BeginTransaction();

            try {
                // Varias sentencias separadas por ; en el mismo archivo — se ejecutan todas en una sola transacción.
                foreach (var sentencia in DividirSentencias(contenido)) {
                    using var cmd = new MySqlCommand(sentencia, conexion, transaccion);
                    cmd.ExecuteNonQuery();
                }

                using var registrar = new MySqlCommand(
                    "INSERT INTO adv__migracion (version, descripcion, checksum) VALUES (@v, @d, @c);",
                    conexion, transaccion);
                registrar.Parameters.AddWithValue("@v", version);
                registrar.Parameters.AddWithValue("@d", Path.GetFileNameWithoutExtension(archivo));
                registrar.Parameters.AddWithValue("@c", checksum);
                registrar.ExecuteNonQuery();

                transaccion.Commit();
            } catch {
                transaccion.Rollback();
                throw; // que truene fuerte — mejor eso que una BD a medio migrar sin que nadie se entere
            }
        }

        private static string ExtraerVersion(string rutaArchivo) =>
            Path.GetFileName(rutaArchivo).Split("__")[0].TrimStart('V');

        private static string CalcularChecksum(string contenido) {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contenido)));
        }

        private static IEnumerable<string> DividirSentencias(string contenido) =>
            contenido.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                      .Where(s => !string.IsNullOrWhiteSpace(s));

        // AsegurarTablaControl() y ObtenerAplicadas() — CREATE TABLE IF NOT EXISTS + SELECT version, checksum, de siempre.
    }
}
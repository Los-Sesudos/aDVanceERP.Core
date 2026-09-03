using aDVanceERP.Core.Modelos.Comun.Interfaces;
using aDVanceERP.Core.Modelos.Modulos.Monedas;
using aDVanceERP.Core.Repositorios.BD;

using MySql.Data.MySqlClient;

using System.Globalization;

namespace aDVanceERP.Core.Repositorios.Modulos.Monedas {
    public class RepoDenominacionMoneda : RepoEntidadBaseDatos<DenominacionMoneda, FiltroBusquedaDenominacionMoneda> {
        public RepoDenominacionMoneda() : base("adv__denominacion_moneda", "id_denominacion_moneda") { }

        protected override string GenerarComandoAdicionar(DenominacionMoneda entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = $"""
                INSERT INTO adv__denominacion_moneda (
                    id_moneda,
                    valor
                ) VALUES (
                    @id_moneda,
                    @valor
                );                
                """;
            parametros = new Dictionary<string, object> {
                { "@id_moneda", entidad.IdMoneda },
                { "@valor" , entidad.Valor }
            };

            return consulta;
        }

        protected override string GenerarComandoEditar(DenominacionMoneda entidad, out Dictionary<string, object> parametros, params IEntidadBaseDatos[] entidadesExtra) {
            var consulta = $"""
                UPDATE adv__denominacion_moneda 
                SET 
                    id_moneda =  @id_moneda,
                    valor = @valor
                WHERE id_denominacion_moneda = @id;                
                """;
            parametros = new Dictionary<string, object> {
                { "@id_moneda", entidad.IdMoneda },
                { "@valor" , entidad.Valor },
                { "@id", entidad.Id }
            };

            return consulta;
        }

        protected override string GenerarComandoEliminar(long id, out Dictionary<string, object> parametros) {
            var consulta = $"""
                DELETE FROM adv__denominacion_moneda 
                WHERE id_denominacion_moneda = @id;                
                """;
            parametros = new Dictionary<string, object> {
                { "@id", id }
            };

            return consulta;
        }

        protected override string GenerarComandoObtener(FiltroBusquedaDenominacionMoneda filtroBusqueda, out Dictionary<string, object> parametros, params string[] criteriosBusqueda) {
            var criterio = criteriosBusqueda.Length > 0 ? criteriosBusqueda[0] : string.Empty;
            var consultaComun = $"""
                SELECT 
                    id_denominacion_moneda,
                    id_moneda,
                    valor
                FROM adv__denominacion_moneda
                """;
            var consulta = filtroBusqueda switch {
                FiltroBusquedaDenominacionMoneda.Id => $"""
                    {consultaComun}
                    WHERE id_denominacion_moneda = @id;
                    """,
                FiltroBusquedaDenominacionMoneda.IdMoneda => $"""
                    {consultaComun}
                    WHERE id_moneda = @id_moneda;
                    """,
                _ => $"""
                    {consultaComun};
                    """
            };
            parametros = filtroBusqueda switch {
                FiltroBusquedaDenominacionMoneda.Id => new Dictionary<string, object> {
                    { "@id", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) } ,
                },
                FiltroBusquedaDenominacionMoneda.IdMoneda => new Dictionary<string, object> {
                    { "@id_moneda", Convert.ToInt64(string.IsNullOrEmpty(criterio) ? "0" : criterio) } ,
                },
                _ => new Dictionary<string, object>()
            };

            return consulta;
        }

        protected override (DenominacionMoneda, List<IEntidadBaseDatos>) MapearEntidad(MySqlDataReader lector) {
            return (new DenominacionMoneda {
                Id = Convert.ToInt64(lector["id_denominacion_moneda"]),
                IdMoneda = Convert.ToInt64(lector["id_moneda"]),
                Valor = Convert.ToDecimal(lector["valor"], CultureInfo.InvariantCulture)
            }, new List<IEntidadBaseDatos>());
        }

        #region SINGLETON

        public static RepoDenominacionMoneda Instancia { get; } = new RepoDenominacionMoneda();

        #endregion
    }
}

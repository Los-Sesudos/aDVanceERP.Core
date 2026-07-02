namespace aDVanceERP.Core.Modelos.Modulos.Servicios.Estadisticas {
    public class OrdenActivaResumen {
        public string Codigo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public DateTime? InicioProgramado { get; set; }
        public DateTime? FinProgramado { get; set; }
        public int PorcentajeAvance { get; set; }
    }
}

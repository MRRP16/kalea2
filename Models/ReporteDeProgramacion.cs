using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class ReporteDeProgramacion
    {
        public string Evento { get; set; }
        public string Cliente { get; set; }
        public string DireccionEntrega { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string TiempoInicio { get; set; }
        public string TiempoFin { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Email { get; set; }
        public string TiempoArmado { get; set; }
        public string Peso { get; set; }
        public string Volumen { get; set; }
        public string Costo { get; set; }
        public string TipoVehiculo { get; set; }
        public string Radio { get; set; }
        public string Etiquetas { get; set; }
        public string Prioridad { get; set; }
    }

    public class RespuestaReporteDeEntrega
    {
        public List<ReporteDeProgramacion> ListadoReporteDeProgramacion { get; set; }
        public List<Vehiculos> Vehiculos { get; set; }
    }
}
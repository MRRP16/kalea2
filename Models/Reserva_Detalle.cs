using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Reserva_Detalle_Casos
    {
        public int Id { get; set; }
        public int NumEntrega { get; set; }
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }
        [DataType(DataType.MultilineText)]
        public string Acciones { get; set; }
        public string NumCaso { get; set; }
        public string Direccion { get; set; }
        public string Cliente { get; set; }
        public string Cel { get; set; }
        public string Tel { get; set; }
        
    }
}
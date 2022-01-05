using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Respuesta
    {
        public List<Models.Vehiculos> Vehiculos { get; set; }
        public List<Models.Reserva> Reservaciones { get; set; }
    }
}
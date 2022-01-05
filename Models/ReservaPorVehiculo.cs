using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class ReservaPorVehiculo
    {
        
        public string Id { get; set; }
        public string Fecha { get; set; }
        public List<Reserva> Listado { get; set; }
    }
}
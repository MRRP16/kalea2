using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class RespuestaReservaPorVehiculo
    {
        public String Id { get; set; }
        public Vehiculos Vehiculo { get; set; }

        public List<ReservaPorVehiculo> Listado { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class VehiculoBloqueoRuta
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Bloqueo { get; set; }
    }
}
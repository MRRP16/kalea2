using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class ListadoArticulosUpdate
    {
       public List<Models.Reserva_Detalle_Articulos> articulosdet { get; set; }

       public string Comentarios { get; set; }
    }
}
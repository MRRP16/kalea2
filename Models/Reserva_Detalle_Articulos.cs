using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Reserva_Detalle_Articulos
    {
        public int Id { get; set; }
        public int IdEntrega { get; set; }
        public string CodigoArticulo { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public string Remisionado { get; set; }
        public string EstadoArticulo { get; set; }
        public string NumEvento { get; set; }
        public string TiempoArmado { get; set; }

        public string Observaciones { get; set; }
        public string DireccionEntrega { get; set; }
        public string DireccionFiscal { get; set; }
        public string ZonaDireccion { get; set; }
        public string NombreCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string PersonaRecibe { get; set; }
        public string Telefono { get; set; }
    }
}
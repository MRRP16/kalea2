using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class CasosEntregas
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string NumTelCliente { get; set; }
        public string NitCliente { get; set; }
        public string FechaEntrega { get; set; }
        public string VehiculoId { get; set; }

        public List<CasosEntregasDetalle> DetalleCE { get; set; }

    }


    public class CasosEntregasDetalle
    { 
        public string Evento { get; set; }
        public string Articulo { get; set; }
        public int CantidadArticulo { get; set; }
        /// <summary>
        ///  Nos indica si el articulo fue dada de baja de inventario o no
        /// </summary>
        public string Remisionado { get; set; }
        /// <summary>
        /// El estado nos indica si fue remisionado por Caja rapida, Domicilio o tienda
        /// </summary>
        public string EstadoArticulo { get; set; }
        public decimal Total { get; set; }
    }
}
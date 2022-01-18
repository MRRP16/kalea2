 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class ReportesGuias
    {
        public string IdEntrega { get; set; } //ok
        public string FechaDeEntrega { get; set; } //ok
        public string Vehiculo{ get; set; } //ok
        public string EventoCaso { get; set; } //ok
        public string Armadores { get; set; } 
        public string HorarioRestriccion { get; set; } //ok
        public string VendedorNombre { get; set; } //ok
        public string SolucionesNombre { get; set; } 
        public string ClienteNombre { get; set; } //ok
        public string ClienteTelefono { get; set; } //ok telefono y ceular
        public string ClienteDireccionEntrega { get; set; } // ok
        public string ObservacionesEvento { get; set; } //ok
        public string ObservacionesTorre { get; set; } //ok
        public string NumeroCaso { get; set; } //ok
        public string ObservacionesCaso { get; set; } //ok
        public string AccionesCaso { get; set; } //ok
        public List<ReportesGuiasProductos> Productos { get; set; }
    }

    public class ReportesGuiasProductos { 
        public string Sku { get; set; }
        public string Descripcion { get; set; }
        public string Bodega { get; set; }
        public string Cantidad { get; set; }
    }

    public class ReportesEventosEntregas
    {
        public string Evento { get; set; }
        public string Vendedor { get; set; }
        public string Observaciones { get; set; }
        public string Bodega { get; set; }
       
        public List<ProductosEventosEntregas> Productos { get; set; }
    }
    public class ProductosEventosEntregas
    {
        public string Sku { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string Bodega { get; set; }
        public string Vehiculo { get; set; }
    }
}
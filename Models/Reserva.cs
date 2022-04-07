using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Models
{
    public class Reserva
    {
        public int Id { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage ="Debe seleccionar una fecha de entrega")]
        public DateTime FechaEntrega { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Debe seleccionar una fecha de entrega")]
        public string FechaEntrega2 { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Horario Inicio")]
        public string FechaInicio { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Horario Fin")]
        public string FechaFin { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Ingrese un numero valido")]
        public double TiempoArmado { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Horario R. Inicio")]
        public string FechaRestriccionInicio { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Horario R. Fin")]
        public string FechaRestriccionFin { get; set; }

        [Required(ErrorMessage = "Ingrese una direccion de entrega")]
        public string DireccionEntrega { get; set; }
        public string DireccionFiscal { get; set; }
        public string Departamento { get; set; }
        public string Municipio { get; set; }
        //[Required(ErrorMessage = "Ingrese una zona de entrega")]
        public string Zona { get; set; }
        public string Coordenadas { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Ingrese un nombre de cliente")]
        public string NombreCliente { get; set; }
        public string NitCliente { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Ingrese un numero de telefono")]
        public string Telefono { get; set; }

        [DataType(DataType.PhoneNumber)]        
        [Required(ErrorMessage = "Ingrese un numero de celular")]
        public string Celular { get; set; }
        
        public string PersonaRecepcion { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string ComentariosVenta { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string ComentariosTorre { get; set; }
        
        public string Estado { get; set; }
        
        public string UsrCreacion { get; set; }
        
        public string FechaCreacion { get; set; }
        
        public string NumeroEntregaDia { get; set; }
        
        public string Vehiculo { get; set; }
        public string NumVehiculo { get; set; }
        public string NumEvento { get; set; }

        public string ListadoEventosCasos { get; set; }

        public string ColorTipoEvento { get; set; }

        [Required(ErrorMessage = "Ingrese una zona de entrega")]
        public string ZonaDireccion { get; set; }

        public int TiempoRuta { get; set; }
        public string ReferenciaReserva { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Debe ingresar una Geolocalización para la entrega")]
        public string Geolocalizacion { get; set; }
        public string FechaArmado { get; set; }
        //[Required(ErrorMessage = "Seleccione un tipo de instalación")]
        public string TipoDeInstalacion { get; set; }
        public int TamanioTarjeta { get; set; }

        public int TiempoRestante { get; set; }
        public DateTime FechaArmadoT { get; set; }

        public DateTime FechaRestriccionI { get; set; }
        public DateTime FechaRestriccionF { get; set; }
        public DateTime FechaInicioOrdenar { get; set; }
        public List<Models.Reserva_Detalle_Casos> Reserva_Casos { get; set; }
        public List<Models.Reserva_Detalle_Articulos> Reserva_Articulos { get; set; }
        public List<SelectListItem> Eventos_Articulos;
        public List<SelectListItem> Casos_Pendientes;
        public List<SelectListItem> TiposDeInstalacion { get; set; }
        public int TamanioTarjetaTranspareante { get; set; }
    }
}
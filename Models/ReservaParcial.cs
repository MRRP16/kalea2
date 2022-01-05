using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class ReservaParcial
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Debe seleccionar una fecha de entrega")]
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

        public string TiempoArmado { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Horario R. Inicio")]
        public string FechaRestriccionInicio { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Referencia")]
        [Required(ErrorMessage = "Debe ingresar una referencia")]
        public string Referencia { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Horario R. Fin")]
        public string FechaRestriccionFin { get; set; }
        public string Vehiculo { get; set; }
        public string NumVehiculo { get; set; }
    }
}
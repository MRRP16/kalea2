using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Traslados
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Debe seleccionar una fecha de traslado")]
        public DateTime FechaEntrega { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Entrega")]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Debe seleccionar una fecha de traslado")]
        public string FechaEntrega2 { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Horario Inicio")]
        [Required(ErrorMessage = "Debe seleccionar una hora de incio")]
        public string FechaRestriccionInicio { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Horario Fin")]
        [Required(ErrorMessage = "Debe seleccionar una hora de finalización")]
        public string FechaRestriccionFin { get; set; }

        public string Tipo { get; set; }

        public string Vehiculo { get; set; }
    }
}
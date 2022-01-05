using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class ParametroGeneral
    {
        [Key]
        [Display(Name = "Codigo")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción para el parametro general")]
        [StringLength(254, ErrorMessage = "La descripción  no puede tener más de 254 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar un valor para el parametro general")]
        [StringLength(100, ErrorMessage = "El valor no puede tener más de 100 caracteres")]
        [Display(Name = "Valor")]
        public string Valor { get; set; }

        public string Estado { get; set; }
        public string UsrCreacion { get; set; }
    }
}
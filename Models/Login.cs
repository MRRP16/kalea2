using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Login
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un nombre de usuario")]
        [StringLength(100, ErrorMessage = "El usuario no puede tener más de 100 caracteres")]
        [Display(Name = "Nombre usuario")]
        //[RegularExpression(@"[^s]+")]
        public string NombreUsuario { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar una contraseña")]
        [StringLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
    }
}
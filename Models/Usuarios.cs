using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Models
{
    public class Usuarios
    {
        [Key]
        [Display(Name = "Codigo")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Debe ingresar nombre del usuario")]
        [StringLength(254, ErrorMessage = "La descripción no puede tener más de 254 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar apellido")]
        [StringLength(254, ErrorMessage = "El apellido no puede tener más de 254 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage = "Debe ingresar un nombre de usuario")]
        [StringLength(100, ErrorMessage = "El usuario no puede tener más de 100 caracteres")]
        [Display(Name = "Nombre usuario")]
        //[RegularExpression(@"[^s]+")]
        public string NombreUsuario { get; set; }

        //[Required(AllowEmptyStrings =false,ErrorMessage = "Debe ingresar una contraseña")]
        [StringLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contrasenia { get; set; }

        public string Estado { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe seleccionar un rol")]
        [Display(Name = "Rol")]
        public string Rol { get; set; }

        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> _Usuarios { get; set; }

    }
}
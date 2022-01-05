
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Roles
    {
        [Key]
        [Display(Name = "Codigo")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripcion para el nuevo Rol.")]
        [StringLength(100, ErrorMessage = "La descripcion no puede ser mayor a 100 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [StringLength(1, ErrorMessage = "El estado no puede ser mayor a 1 caracter.")]

        public string Estado { get; set; }

      
        public List<Models.Rol_Permiso> rol_Permiso { get; set; }

        public Roles()
        {
            rol_Permiso = new List<Rol_Permiso>();
        }
    }
}
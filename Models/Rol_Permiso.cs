using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class Rol_Permiso
    {
        public int Codigo_rol { get; set; }
        public string Descripcion_Permiso { get; set; }
        public int Codigo_Permiso { get; set; }
        public bool Crear { get; set; }
        public bool Editar { get; set; }
        public bool Visualizar { get; set; }
        public bool Eliminar { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Models
{
    public class Vehiculos
    {
        [Key]
        [Display(Name = "Codigo")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción para el vehiculo")]
        [StringLength(254, ErrorMessage = "La descripción  no puede tener más de 254 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar un numero de placa")]
        [StringLength(7, ErrorMessage = "El valor no puede tener más de 7 caracteres")]
        [Display(Name = "Placa")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "Debe ingresar un nombre de piloto")]
        [StringLength(100, ErrorMessage = "El piloto no puede tener más de 100 caracteres")]
        [Display(Name = "Piloto")]
        public string Piloto { get; set; }

        [Required(ErrorMessage = "Debe ingresar un volumen de carga")]
        [StringLength(50, ErrorMessage = "El volumen de carga no puede tener más de 50 caracteres")]
        [Display(Name = "Volumen de carga (m^3)")]
        [DataType(DataType.Currency)]
        public string VolumenCarga { get; set; }

        [Required(ErrorMessage = "Debe ingresar un peso de carga")]
        [StringLength(50, ErrorMessage = "El peso de carga no puede tener más de 50 caracteres")]
        [Display(Name = "Peso de carga (KG)")]
        [DataType(DataType.Currency)]
        public string PesoCarga { get; set; }

        [Required(ErrorMessage = "Debe ingresar un peso de carga")]
        [Display(Name = "Hora inicio de labores")]
        [DataType(DataType.Time)]
        public string HoraInicioLabores { get; set; }

        public bool Seleccionado { get; set; }


 
        public List<SelectListItem> TrasladosSiNo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar si el vehiculo es para traslado o no.")]
        [Display(Name = "Traslados")]
        public string TrasladoSiNo { get; set; }
        public string Estado { get; set; }
        public string UsrCreacion { get; set; }

        public Vehiculos()
        {
            TrasladosSiNo = new List<SelectListItem>();
            TrasladosSiNo.Add(new SelectListItem { Text = "No", Value = "No",Selected= true });
            TrasladosSiNo.Add(new SelectListItem { Text ="Si", Value = "Si" });
           

        }
    }
}
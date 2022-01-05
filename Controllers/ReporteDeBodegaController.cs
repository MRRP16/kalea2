using kalea2.Models;
using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class ReporteDeBodegaController : Controller
    {
       
        public List<Bodegas> ListadoBodegas()
        {
            List<Bodegas> listadoBodegas = new List<Bodegas>();
            listadoBodegas.Add(new Bodegas { Nombre = "Bodega Z10", Codigo = "1020" });
            listadoBodegas.Add(new Bodegas { Nombre = "Bodega Z11", Codigo = "1120" });
            listadoBodegas.Add(new Bodegas { Nombre = "Bodega DecoCity", Codigo = "DC20" });
            listadoBodegas.Add(new Bodegas { Nombre = "Alsersa Z17", Codigo = "1220" });

            return listadoBodegas;
        }

        [AuthorizeUser(idOperacion: 0, pantalla: 9)]
        public ActionResult Index()
        {
            return View(ListadoBodegas());
        }

        [AuthorizeUser(idOperacion: 0, pantalla: 9)]
        public ActionResult Details(string fecha, string bodegaId)
        {
            if (fecha == "")
            {
                TempData["texto"] = "Seleccione una fecha";
                TempData["color"] = "error";
                return RedirectToAction("Index");
            }

            DateTime date = Convert.ToDateTime(fecha, CultureInfo.InvariantCulture);

            PDF pdf = new PDF();
            Reportes reportes = new Reportes();
            List<ReportesEventosEntregas> listado = reportes.GetEventosParaEntregas(bodegaId: bodegaId, fecha: date.ToString("dd/MM/yyyy"));

            List<Bodegas> listadoBodegas = ListadoBodegas();
            string nombreBodega = "";
            foreach (var item in listadoBodegas)
            {
                if (item.Codigo == bodegaId)
                {
                    nombreBodega = item.Nombre;
                }
            }

            byte[] respuesta = pdf.CrearReporteDeEntrega(listado: listado, fechaDeEntrega: date.ToString("dd/MM/yyyy"), NombreBodega: nombreBodega);
            return File(respuesta, "application/pdf", "reporteDeEntregas.pdf");
        }


    }
}
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
    public class ReporteDeEntregasController : Controller
    {
        // GET: ReporteDeEntregas
        [AuthorizeUser(idOperacion: 0, pantalla: 8)]
        public ActionResult Index()
        {
            Utilidades.Reportes reportes = new Utilidades.Reportes();
            List<Models.Vehiculos> vehiculos = reportes.getVehiculos();
            return View(vehiculos);
        }


        // GET: ReporteDeEntregas
        [AuthorizeUser(idOperacion: 0, pantalla: 8)]
        public ActionResult Details(string fecha, string vehiculo)
        {
            if (fecha == "")
            {
                TempData["texto"] = "Seleccione una fecha";
                TempData["color"] = "error";
                return RedirectToAction("Index");
            }

            PDF pdf = new PDF();
            Reportes reportes = new Reportes();
            DateTime date = Convert.ToDateTime(fecha, CultureInfo.InvariantCulture);

            List<ReportesGuias> listado = reportes.GetEventosCasosParaGuiasDeTransporte(vehiculoId: vehiculo, fecha: date.ToString("dd/MM/yyyy"));

            Utilidades.Vehiculos vehiculos = new Utilidades.Vehiculos();
            Models.Vehiculos respuestaVehiculo = vehiculos.ObtenerVehiculo(int.Parse(vehiculo));

            byte[] respuesta =  pdf.CrearReporteDeTransporte(listado, "03/12/21", respuestaVehiculo.Descripcion);
            return File(respuesta, "application/pdf", "reporteDeEntregas.pdf");
        }
    }
}
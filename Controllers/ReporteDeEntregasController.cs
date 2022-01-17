using kalea2.Models;
using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            Utilidades.Vehiculos vehiculos = new Utilidades.Vehiculos();

            //if (vehiculo == "todos")
            //{
            //    List<Models.Vehiculos> listadoVehiculos = vehiculos.ListadoVehiculos();



            //    using (var memoryStream = new MemoryStream())
            //    {
            //        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            //        {
            //            var file1 = archive.CreateEntry("file1.txt");
            //            using (var streamWriter = new StreamWriter(file1.Open()))
            //            {
            //                streamWriter.Write("content1");
            //            }

            //            var file2 = archive.CreateEntry("file2.txt");
            //            using (var streamWriter = new StreamWriter(file2.Open()))
            //            {
            //                streamWriter.Write("content2");
            //            }
            //        }

            //        return File(memoryStream.ToArray(), "application/zip", "Images.zip");
            //    }


            //    foreach (var item in listadoVehiculos)
            //    {
            //        List<ReportesGuias> listado = reportes.GetEventosCasosParaGuiasDeTransporte(vehiculoId: item.Codigo.ToString(), fecha: date.ToString("dd/MM/yyyy"));
            //        Models.Vehiculos respuestaVehiculo = vehiculos.ObtenerVehiculo(int.Parse(item.Codigo.ToString()));
            //        byte[] respuesta = pdf.CrearReporteDeTransporte(listado, date.ToString("dd/MM/yyyy"), respuestaVehiculo.Descripcion);
            //        string nombreArchivo = respuestaVehiculo.Descripcion.ToString() + "_" + date.ToString("dd/MM/yyyy") + ".pdf";
            //    return File(respuesta, "application/pdf", nombreArchivo);
            //    }
            //    return View();
                
            //}
            //else
            //{
                List<ReportesGuias> listado = reportes.GetEventosCasosParaGuiasDeTransporte(vehiculoId: vehiculo, fecha: date.ToString("dd/MM/yyyy"));
                Models.Vehiculos respuestaVehiculo = vehiculos.ObtenerVehiculo(int.Parse(vehiculo));
                byte[] respuesta = pdf.CrearReporteDeTransporte(listado, date.ToString("dd/MM/yyyy"), respuestaVehiculo.Descripcion);
                string nombreArchivo = respuestaVehiculo.Descripcion.ToString() + "_" + date.ToString("dd/MM/yyyy") + ".pdf";
                return File(respuesta, "application/pdf", nombreArchivo);
            //}
        }
    }
}
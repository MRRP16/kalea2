using kalea2.Models;
using kalea2.Utilidades;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace kalea2.Controllers
{
    public class ReporteDeProgramacionController : Controller
    {
        // GET: ReportesDeProgramacion
        [AuthorizeUser(idOperacion: 0, pantalla: 10)]
        public ActionResult Index()
        {
            Utilidades.Reportes reportes = new Utilidades.Reportes();
            List<Models.Vehiculos> vehiculos = reportes.getVehiculos();
            return View("Details", vehiculos);
        }

        //GET: ReportesDeProgramacion/Details/5
        [AuthorizeUser(idOperacion: 0, pantalla: 10)]
        public ActionResult Details(string fecha, string vehiculo)
        {
            Utilidades.Reportes reportes = new Utilidades.Reportes();
            List<Models.Vehiculos> vehiculos = reportes.getVehiculos();

            DateTime.TryParse(fecha, out DateTime date);
            var fecha2 = date.ToString("yyyy-MM-dd");

            //Utilidades.Reportes reportes = new Utilidades.Reportes();
            List<ReporteDeProgramacion> listado = reportes.getEntregas(fecha2, int.Parse(vehiculo));

            if (listado == null)
            {
                return View(vehiculos);
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            List<Models.Reserva> ListadoInformacion = new List<Models.Reserva>();

            using (ExcelPackage excel = new ExcelPackage())
            {
                string nombreHoja = "Reporte de programación";
                excel.Workbook.Worksheets.Add(nombreHoja);

                var headerRow = new List<string[]>() { new string[] {
                    "Evento",
                    "Cliente",
                    "Direccion",
                    "Latitud",
                    "Longitud",
                    "TiempoInicio",
                    "TiempoFin",
                    "Telefono1",
                    "Telefono2",
                    "Email",
                    "TArmado",
                    "Peso",
                    "Volumen",
                    "Costo",
                    "TipoVehiculo",
                    "Radio",
                    "Etiquetas",
                    "Prioridad"
                } };

                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";
                var worksheet = excel.Workbook.Worksheets[nombreHoja];
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                string Panel = string.Empty;
                for (int i = 0; i < listado.Count(); i++)
                {
                    Panel = listado[i].TipoVehiculo;
                    var RowInformacion = new List<string[]>()
                    { new string[]{
                        listado[i].Evento,
                        listado[i].Cliente,
                        listado[i].DireccionEntrega,
                        listado[i].Latitud,
                        listado[i].Longitud,
                        listado[i].TiempoInicio,
                        listado[i].TiempoFin,
                        listado[i].Telefono1,
                        listado[i].Telefono2,
                        listado[i].Email,
                        listado[i].TiempoArmado, 
                        //listado[i].Peso, 
                        "null", 
                        //listado[i].Volumen, 
                        "null",
                        //listado[i].Costo, 
                        "null",
                        listado[i].TipoVehiculo, 
                        //listado[i].Radio, 
                        "1",
                        //listado[i].Etiquetas, 
                        "null",
                        listado[i].Prioridad,
                    } };
                    worksheet.Cells[i + 2, 1].LoadFromArrays(RowInformacion);
                }
                NumberFormatInfo nfi = new CultureInfo("es-GT", false).NumberFormat;

                using (MemoryStream stream = new MemoryStream())
                {
                    excel.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteGPS_"+ Panel + ".xlsx");

                    //archivo.SaveAs(stream);
                    //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
                //}
            }

            return View(vehiculos);
        }
    }
}

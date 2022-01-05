//using ClosedXML.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class ExcelController : Controller
    {
        // GET: Excel
        public ActionResult ReporteDeProgramacion()
        {
            //using(var workbook = new XLWorkbook())
            //{
            //    var worksheet = workbook.Worksheets.Add("Students");
            //    var currentRow = 
            //}




            return View();
        }

        public ExcelPackage Excel2()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            List<Models.Reserva> ListadoInformacion = new List<Models.Reserva>();

            using (ExcelPackage excel = new ExcelPackage())
            {
                string nombreHoja = "Listado de analisis";
                excel.Workbook.Worksheets.Add(nombreHoja);
                var headerRow = new List<string[]>()
                        {
                        new string[] { "Evento", "Cliente", "Direccion", "Latitud", "Longitud", "TiempoInicio", "TiempoFin", "Telefono1", "Telefono2", "Email" }
                        };

                var cellData = ListadoInformacion.Select(n => new object[] { n.Id , n.FechaRestriccionInicio, n.FechaRestriccionFin, n.Geolocalizacion});

                // Determine the header range (e.g. A1:D1)
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                // Target a worksheet
                var worksheet = excel.Workbook.Worksheets[nombreHoja];

                // Popular header row data
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                worksheet.Cells[2, 1].LoadFromArrays(cellData);
                NumberFormatInfo nfi = new CultureInfo("es-GT", false).NumberFormat;

                return excel;


                //excel.Workbook.Properties.Title = "Attempts";
                //this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //this.Response.AddHeader("content-disposition", string.Format("attachment;  filename={0}", "ExcellData.xlsx"));
                //this.Response.BinaryWrite(excel.GetAsByteArray());

                //return File(finalResult, "application/zip", fileName.Substring(6));
            }
        }
       
    }
}

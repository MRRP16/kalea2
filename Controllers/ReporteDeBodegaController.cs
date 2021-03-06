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
    public class ReporteDeBodegaController : Controller
    {

        public List<Bodegas> ListadoBodegas()
        {
            List<Bodegas> listadoBodegas = new List<Bodegas>();
            listadoBodegas.Add(new Bodegas { Nombre = "Todos", Codigo = "0000" });
            listadoBodegas.Add(new Bodegas { Nombre = "B.Z10", Codigo = "1020" });
            listadoBodegas.Add(new Bodegas { Nombre = "B.Z11", Codigo = "1120" });
            listadoBodegas.Add(new Bodegas { Nombre = "B. DecoCity", Codigo = "DC20" });
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

            List<Bodegas> listadoBodegas = reportes.listadoBodegas();
            string nombreBodega = "";
            foreach (var item in listadoBodegas)
            {
                if (item.Codigo == bodegaId)
                {
                    nombreBodega = item.Nombre;
                }
            }

            //byte[] respuesta = pdf.CrearReporteDeEntrega(listado: listado, fechaDeEntrega: date.ToString("dd/MM/yyyy"), NombreBodega: nombreBodega);
            //return File(respuesta, "application/pdf", "reporteDeEntregas.pdf");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                if (bodegaId.Equals("0000"))
                {
                    foreach (var item in listadoBodegas)
                    {
                        if (!item.Codigo.Equals("0000"))
                        {

                            List<ReportesEventosEntregas> listado2 = listado.Where(X => X.Bodega == item.Codigo).ToList();
                            if (listado2.Count > 0)
                            {
                                //string nombreHoja = "BODEGA " + item.Nombre;
                                string nombreHoja = item.Nombre;
                                excel.Workbook.Worksheets.Add(nombreHoja);

                                var headerRow = new List<string[]>() { new string[] {
                    "Pend: " + item.Nombre ,
                    null,
                     "Emisión: " + DateTime.Now.ToString("dd/MM/yy HH:MM:ss"),
                      null,
                        null,
                    "Entrega: " + date.ToString("dd/MM/yy"),
                     null,
                    null,
                    null,
                    null,
                } };

                                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";
                                var worksheet = excel.Workbook.Worksheets[nombreHoja];
                                worksheet.Cells[headerRange].Style.Font.Bold = true;
                                worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                                int SiguienteFila = 2;

                                var RowInformacion = new List<string[]>()
                    { new string[]{
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                    } };
                                worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                SiguienteFila++;

                                RowInformacion = new List<string[]>()
                    { new string[]{
                        "Preparó: ",
                        " ",
                        " ",
                        " ",
                        "Entrego: ",
                        " ",
                        " ",
                        " ",
                    } };
                                worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                SiguienteFila++;

                                RowInformacion = new List<string[]>()
                    { new string[]{
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                    } };
                                worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                SiguienteFila++;

                                RowInformacion = new List<string[]>()
                    { new string[]{
                        "Evento",
                        "Vendedor",
                        "Observaciones",
                        "Inm",
                        "Código",
                        "Descripción",
                        "Bod",
                        "Cant",
                        "Ruta",
                    } };
                                worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                SiguienteFila++;


                                for (int i = 0; i < listado2.Count(); i++)
                                {
                                    RowInformacion = new List<string[]>()
                            { new string[]{
                                listado2[i].Evento,
                                listado2[i].Vendedor,
                                listado2[i].Observaciones,
                            } };
                                    worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                    SiguienteFila++;

                                    if (listado2[i].Productos.Count>1)
                                    {
                                        for (int j = 0; j < listado2[i].Productos.Count; j++)
                                        {
                                            if (!string.IsNullOrEmpty(listado2[i].Productos[j].Descripcion))
                                            {
                                                RowInformacion = new List<string[]>()
                                            { new string[]{
                                                "",
                                                "",
                                                "",
                                                //inmediatas
                                                listado2[i].Productos[j].Inmediatas,
                                                listado2[i].Productos[j].Sku,
                                                listado2[i].Productos[j].Descripcion,
                                                listado2[i].Productos[j].Bodega,
                                                listado2[i].Productos[j].Cantidad,
                                                listado2[i].Productos[j].Vehiculo,
                                            } };
                                                worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                                SiguienteFila++;
                                            }
                                        }
                                    }
                                    else if(listado2[i].Productos.Count == 1)
                                    {
                                        if (!string.IsNullOrEmpty(listado2[i].Productos[0].Descripcion))
                                        {
                                            RowInformacion = new List<string[]>()
                                            { new string[]{
                                                "",
                                                "",
                                                "",
                                                //inmediatas
                                                listado2[i].Productos[0].Inmediatas,
                                                listado2[i].Productos[0].Sku,
                                                listado2[i].Productos[0].Descripcion,
                                                listado2[i].Productos[0].Bodega,
                                                listado2[i].Productos[0].Cantidad,
                                                listado2[i].Productos[0].Vehiculo,
                                            } };
                                            worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                            SiguienteFila++;
                                        }
                                        else
                                        {
                                            RowInformacion = new List<string[]>()
                                            { new string[]{
                                                "",
                                                "",
                                                "",
                                                //inmediatas
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                            } };
                                            worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                            SiguienteFila++;
                                        }
                                    }
                                    else
                                    {
                                        RowInformacion = new List<string[]>()
                                            { new string[]{
                                                "",
                                                "",
                                                "",
                                                //inmediatas
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                            } };
                                        worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                                        SiguienteFila++;
                                    }
                                    
                                   

                                }
                            }

                        }
                    }

                    NumberFormatInfo nfi = new CultureInfo("es-GT", false).NumberFormat;

                    string NombreReporte = "Pendientes_" + nombreBodega + "_" + date.Day.ToString().PadLeft(2, '0') + "_" + date.Month.ToString().PadLeft(2, '0') + "_" + date.ToString("yy").PadLeft(2, '0');


                    using (MemoryStream stream = new MemoryStream())
                    {
                        excel.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", NombreReporte + ".xlsx");

                        //archivo.SaveAs(stream);
                        //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                    }
                }
                else
                {
                    string nombreHoja = "BODEGA " + nombreBodega;
                    excel.Workbook.Worksheets.Add(nombreHoja);

                    var headerRow = new List<string[]>() { new string[] {
                    "Pendientes: " + nombreBodega ,
                    null,
                     "Emisión: " + DateTime.Now.ToString("dd/MM/yy HH:MM:ss"),
                      null,
                        null,
                    "Entrega: " + date.ToString("dd/MM/yy"),
                     null,
                    null,
                    null,
                    null,
                } };

                    string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";
                    var worksheet = excel.Workbook.Worksheets[nombreHoja];
                    worksheet.Cells[headerRange].Style.Font.Bold = true;
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                    int SiguienteFila = 2;

                    var RowInformacion = new List<string[]>()
                    { new string[]{
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                    } };
                    worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                    SiguienteFila++;

                    RowInformacion = new List<string[]>()
                    { new string[]{
                        "Preparó: ",
                        " ",
                        " ",
                        " ",
                        "Entrego: ",
                        " ",
                        " ",
                        " ",
                    } };
                    worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                    SiguienteFila++;

                    RowInformacion = new List<string[]>()
                    { new string[]{
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                    } };
                    worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                    SiguienteFila++;

                    RowInformacion = new List<string[]>()
                    { new string[]{
                        "Evento",
                        "Vendedor",
                        "Observaciones",
                        "Inm",
                        "Código",
                        "Descripción",
                        "Bod",
                        "Cant",
                        "Ruta",
                    } };
                    worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                    SiguienteFila++;


                    for (int i = 0; i < listado.Count(); i++)
                    {
                        RowInformacion = new List<string[]>()
                    { new string[]{
                        listado[i].Evento,
                        listado[i].Vendedor,
                        listado[i].Observaciones,
                    } };
                        worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                        SiguienteFila++;

                        for (int j = 0; j < listado[i].Productos.Count; j++)
                        {
                            RowInformacion = new List<string[]>()
                        { new string[]{
                            "",
                            "",
                            "",
                            //inmediatas
                            listado[i].Productos[j].Inmediatas,
                            listado[i].Productos[j].Sku,
                            listado[i].Productos[j].Descripcion,
                            listado[i].Productos[j].Bodega,
                            listado[i].Productos[j].Cantidad,
                            listado[i].Productos[j].Vehiculo,
                        } };
                            worksheet.Cells[SiguienteFila, 1].LoadFromArrays(RowInformacion);
                            SiguienteFila++;
                        }
                    }
                    NumberFormatInfo nfi = new CultureInfo("es-GT", false).NumberFormat;
                    string NombreReporte = "Pendientes_" + nombreBodega + "_" + date.Day.ToString().PadLeft(2, '0') + "_" + date.Month.ToString().PadLeft(2, '0') + "_" + date.ToString("yy").PadLeft(2, '0');
                    using (MemoryStream stream = new MemoryStream())
                    {
                        excel.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", NombreReporte + ".xlsx");

                        //archivo.SaveAs(stream);
                        //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                    }
                }

                //}
            }
        }


    }
}
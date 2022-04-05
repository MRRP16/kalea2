using iTextSharp.text;
using iTextSharp.text.pdf;
using kalea2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class PDF
    {
        public byte[] CrearReporteDeTransporte(List<ReportesGuias> listado, string fechaDeEntrega, string Vehiculo)
        {
            PdfCelda parametros;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                var fecha = DateTime.Now;
                Document doc = new Document(PageSize.A4, -50, -55, 25, 0);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();
                int numeroDePagina = 1;
                bool numeroDePaginaEncabezado = true;

                PdfPTable table = new PdfPTable(10);
                //table.AddCell(GetCell(Texto: "Pagina" + numeroDePagina, Rowspan: 1, Colspan: 10, HorizontalAlignment: 3, Size: 10));
                table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Border: 2));

                //**************************************************************** HEADER
                table.AddCell(GetCell(Texto: "Guias de transporte Impresión: ",     Rowspan: 1, Colspan: 3, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Para el: " + fechaDeEntrega,          Rowspan: 1, Colspan: 4, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Vehiculo: " + Vehiculo,               Rowspan: 1, Colspan: 3, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));

                table.AddCell(GetCell(Texto: "Fecha de impresión: "+fecha.ToString(),Rowspan: 1, Colspan: 10, HorizontalAlignment: 3, Border: 2, PaddingBottom: 10));

                //**************************************************************** FICHA

                foreach (var item in listado)
                {
                    //if (!numeroDePaginaEncabezado)
                    //{
                    //    numeroDePagina++;
                    //    table.AddCell(GetCell(Texto: "Pagina" + numeroDePagina, Rowspan: 1, Colspan: 10, HorizontalAlignment: 3, Size: 10));
                    //    table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Border: 2));
                    //}

                    //if (table.Rows > 29)
                    //{
                    //    doc.Add(table);
                    //    doc.NewPage();
                    //}

                    table.AddCell(GetCell(Texto: "Evento: " + item.EventoCaso,              Rowspan: 1, Colspan: 5, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                    table.AddCell(GetCell(Texto: "Armadores: ",                             Rowspan: 1, Colspan: 5, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));

                    table.AddCell(GetCell(Texto: "Restricción: " + item.HorarioRestriccion, Rowspan: 1, Colspan: 4, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                    table.AddCell(GetCell(Texto: "Vendedor: " + item.VendedorNombre,        Rowspan: 1, Colspan: 3, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                    table.AddCell(GetCell(Texto: "Soluciones: " ,                           Rowspan: 1, Colspan: 3, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));

                    table.AddCell(GetCell(Texto: " ",                                       Rowspan: 1, Colspan: 10, HorizontalAlignment: 3));


                    table.AddCell(GetCell(Texto: "Contacto: " + item.ClienteNombre,             Rowspan: 1, Colspan: 3, HorizontalAlignment: 0));
                    table.AddCell(GetCell(Texto: "Tel: " + item.ClienteTelefono,                Rowspan: 1, Colspan: 3, HorizontalAlignment: 0));
                    table.AddCell(GetCell(Texto: "Dirección: " + item.ClienteDireccionEntrega,  Rowspan: 1, Colspan: 4, HorizontalAlignment: 0));
                    table.AddCell(GetCell(Texto: "Tipo Inst.: " + item.TipoInstalacion, Rowspan: 1, Colspan: 4, HorizontalAlignment: 0));

                    table.AddCell(GetCell(Texto: "Obs. Evento: " + item.ObservacionesEvento,       Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5));
                    table.AddCell(GetCell(Texto: "Obs. Torre: " + item.ObservacionesTorre,      Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5));

                    table.AddCell(GetCell(Texto: " ",               Rowspan: 1, Colspan: 10, HorizontalAlignment: 2));

                    table.AddCell(GetCell(Texto: "CÓDIGO:",         Rowspan: 1, Colspan: 1, HorizontalAlignment: 0, PaddingBottom: 5));
                    table.AddCell(GetCell(Texto: "DESCRIPCIÓN:",    Rowspan: 1, Colspan: 7, HorizontalAlignment: 0, PaddingBottom: 5));
                    table.AddCell(GetCell(Texto: "BOD: ",           Rowspan: 1, Colspan: 1, HorizontalAlignment: 0, PaddingBottom: 5));
                    table.AddCell(GetCell(Texto: "CANT: " ,         Rowspan: 1, Colspan: 1, HorizontalAlignment: 1, PaddingBottom: 5));

                    int total = 0;
                    foreach (var item2 in item.Productos)
                    {
                        if (item2.Sku != "")
                        {
                            table.AddCell(GetCell(Texto: item2.Sku, Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                            table.AddCell(GetCell(Texto: item2.Descripcion, Rowspan: 1, Colspan: 7, HorizontalAlignment: 0));
                            table.AddCell(GetCell(Texto: item2.Bodega, Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                            table.AddCell(GetCell(Texto: item2.Cantidad, Rowspan: 1, Colspan: 1, HorizontalAlignment: 1));
                            total += int.Parse(item2.Cantidad);
                        }
                    }

                    table.AddCell(GetCell(Texto: " ",               Rowspan: 1, Colspan: 10, HorizontalAlignment: 2));

                    if (item.NumeroCaso.ToString() != "")
                    {
                        table.AddCell(GetCell(Texto: "TOTAL:",          Rowspan: 1, Colspan: 9, HorizontalAlignment: 2, PaddingBottom: 10));
                        table.AddCell(GetCell(Texto: total.ToString(),  Rowspan: 1, Colspan: 1, HorizontalAlignment: 1, PaddingBottom: 10));

                        table.AddCell(GetCell(Texto: "Caso: " + item.NumeroCaso,        Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5));
                        table.AddCell(GetCell(Texto: "Acciones: " + item.AccionesCaso,  Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5, Border: 2, PaddingBottom: 10));
                    }
                    else
                    {
                        table.AddCell(GetCell(Texto: "TOTAL:",          Rowspan: 1, Colspan: 9, HorizontalAlignment: 2, Border: 2, PaddingBottom: 10));
                        table.AddCell(GetCell(Texto: total.ToString(),  Rowspan: 1, Colspan: 1, HorizontalAlignment: 1, Border: 2, PaddingBottom: 10));
                    }
                    
                    numeroDePaginaEncabezado = false;
                }

                //**************************************************************** END FICHA
                numeroDePagina++;
                doc.Add(table);
                doc.Close();
                byte[] result = ms.ToArray();
                return AddPageNumber(result);
                #region
                //try
                //{
                //    #region encabezado
                //    PdfPTable TablaEncabezado = new PdfPTable(2);
                //    TablaEncabezado.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                //    Paragraph columnaIzquierda = new Paragraph
                //    {
                //        Font = FontFactory.GetFont(FontFactory.HELVETICA, 10f)
                //    };
                //    Paragraph columnaDerecha = new Paragraph
                //    {
                //        Font = FontFactory.GetFont(FontFactory.HELVETICA, 10f)
                //    };
                //    columnaIzquierda.Add("BIMAGUA, S.A \nReporte de Pendientes de despacho para \n ");
                //    columnaDerecha.Add("Fecha de impresión: " + fecha +" \n Fecha del reporte ");
                //    PdfPCell CeldaIzquieda = new PdfPCell(columnaIzquierda)
                //    {
                //        Border = Rectangle.NO_BORDER,
                //    };
                //    PdfPCell CeldaDerecha = new PdfPCell(columnaDerecha)
                //    {
                //        Border = Rectangle.NO_BORDER,
                //        HorizontalAlignment = Element.ALIGN_RIGHT,
                //    };
                //    TablaEncabezado.AddCell(CeldaIzquieda);
                //    TablaEncabezado.AddCell(CeldaDerecha);
                //    #endregion
                //    //************************************************************************************************************
                //    #region 2da fila
                //    PdfPTable TablaFila = new PdfPTable(1);
                //    Paragraph ParrafoFila = new Paragraph
                //    {
                //        Font = FontFactory.GetFont(FontFactory.HELVETICA, 10f)
                //    };
                //    ParrafoFila.Add("***** Listado de Factura en estado Liberado *****");
                //    PdfPCell CeldaFila = new PdfPCell(ParrafoFila)
                //    {
                //        Border = Rectangle.BOTTOM_BORDER,
                //        HorizontalAlignment = Element.ALIGN_CENTER,
                //    };
                //    TablaFila.AddCell(CeldaFila);
                //    #endregion
                //    //************************************************************************************************************
                //    tablaGeneral.AddCell(TablaEncabezado);
                //    tablaGeneral.AddCell(TablaFila);
                //}
                //catch
                //{

                //}

                #endregion


            }
        }

        public byte[] CrearReporteDeEntrega(List<ReportesEventosEntregas> listado, string fechaDeEntrega, string NombreBodega)
        {
            PdfCelda parametros;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                var fecha = DateTime.Now;
                Document doc = new Document(PageSize.A4, -50, -55, 25, 0);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();

                PdfPTable table = new PdfPTable(10);
                //**************************************************************** HEADER
                table.AddCell(GetCell(Texto: "Pendientes: " + NombreBodega,  Rowspan: 1, Colspan: 3, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Emisión: " + fecha.ToString(), Rowspan: 1, Colspan: 4, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Reporte: " + fechaDeEntrega,   Rowspan: 1, Colspan: 3, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));

                table.AddCell(GetCell(Texto: " ",         Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, PaddingBottom: 10));

                table.AddCell(GetCell(Texto: "Preparó: ", Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, Border: 0, PaddingBottom: 10));
                table.AddCell(GetCell(Texto: "",          Rowspan: 1, Colspan: 4, HorizontalAlignment: 3, Border: 2, PaddingBottom: 10));
                table.AddCell(GetCell(Texto: "Entregó: ", Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, Border: 0, PaddingBottom: 10));
                table.AddCell(GetCell(Texto: "",          Rowspan: 1, Colspan: 4, HorizontalAlignment: 3, Border: 2, PaddingBottom: 10));

                table.AddCell(GetCell(Texto: " ",               Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, PaddingBottom: 10));

                table.AddCell(GetCell(Texto: "Evento: ",        Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Vendedor: ",      Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Observaciones: ", Rowspan: 1, Colspan: 2, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Inm: ",           Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Código: ",        Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Descripción: ",   Rowspan: 1, Colspan: 2, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Bod: ",           Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, PaddingBottom: 5));
                table.AddCell(GetCell(Texto: "Cant: ",          Rowspan: 1, Colspan: 1, HorizontalAlignment: 3, PaddingBottom: 5));

                table.AddCell(GetCell(Texto: " ",               Rowspan: 1, Colspan: 10, Border: 2, HorizontalAlignment: 2, PaddingBottom: 10));

                //**************************************************************** FICHA

                foreach (var item in listado)
                {
                    table.AddCell(GetCell(Texto: item.Evento,   Rowspan: 1, Colspan: 1, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                    //vendedor
                    table.AddCell(GetCell(Texto: " ",           Rowspan: 1, Colspan: 1, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                    //observaciones
                    table.AddCell(GetCell(Texto: " ",           Rowspan: 1, Colspan: 8, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));

                    foreach (var item2 in item.Productos)
                    {
                        table.AddCell(GetCell(Texto: "",                Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                        table.AddCell(GetCell(Texto: "",                Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                        table.AddCell(GetCell(Texto: "",                Rowspan: 1, Colspan: 2, HorizontalAlignment: 0));
                        //inmediatas
                        table.AddCell(GetCell(Texto: "",                Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                        table.AddCell(GetCell(Texto: item2.Sku,         Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                        table.AddCell(GetCell(Texto: item2.Descripcion, Rowspan: 1, Colspan: 2, HorizontalAlignment: 0));
                        table.AddCell(GetCell(Texto: item2.Bodega,      Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                        table.AddCell(GetCell(Texto: item2.Cantidad,    Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                    }

                    table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, Border: 2, HorizontalAlignment: 2));
                }

                //**************************************************************** END FICHA

                doc.Add(table);
                doc.Close();
                byte[] result = ms.ToArray();
                return result;
            }
        }

        public byte[] CrearTodosLosReportesDeTransporte(List<Models.Vehiculos> listadoVehiculos, string fechaDeEntrega)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                var fecha = DateTime.Now;
                Document doc = new Document(PageSize.A4, -50, -55, 25, 0);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();
                int numeroDePagina = 0;
                bool numeroDePaginaEncabezado = false;
                PdfPTable table = new PdfPTable(10);
                foreach (var vehiculo in listadoVehiculos)
                {
                    numeroDePagina++;
                    Reportes reportes = new Reportes();
                    List<ReportesGuias> listado = reportes.GetEventosCasosParaGuiasDeTransporte(vehiculoId: vehiculo.Codigo.ToString(), fecha: fechaDeEntrega);
                    if (listado.Count>0)
                    {
                        //table.AddCell(GetCell(Texto: "Pagina " + numeroDePagina, Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Size: 10));
                        table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Border: 2));
                        numeroDePaginaEncabezado = true;

                        table.AddCell(GetCell(Texto: "Vehiculo: " + vehiculo.Descripcion, Rowspan: 1, Colspan: 10, HorizontalAlignment: 1, Size: 10));

                        table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Border: 2));
                        table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Border: 2));

                        table.AddCell(GetCell(Texto: "Guias de transporte Impresión: ", Rowspan: 1, Colspan: 3, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));
                        table.AddCell(GetCell(Texto: "Para el: " + fechaDeEntrega, Rowspan: 1, Colspan: 4, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));
                        table.AddCell(GetCell(Texto: "Vehiculo: " + vehiculo.Descripcion, Rowspan: 1, Colspan: 3, HorizontalAlignment: 1, Border: 0, PaddingBottom: 5));

                        table.AddCell(GetCell(Texto: "Fecha de impresión: " + fecha.ToString(), Rowspan: 1, Colspan: 10, HorizontalAlignment: 3, Border: 2, PaddingBottom: 10));

                        foreach (var item in listado)
                        {
                            //if (!numeroDePaginaEncabezado)
                            //{
                            //    numeroDePagina++;
                            //    table.AddCell(GetCell(Texto: "Pagina " + numeroDePagina, Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Size: 10));
                            //    table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2, Border: 2));
                            //}
                            if (table.TotalWidth > 420)
                            {
                                doc.Add(table);
                                doc.NewPage();
                            }

                            table.AddCell(GetCell(Texto: "Evento: " + item.EventoCaso, Rowspan: 1, Colspan: 5, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                            table.AddCell(GetCell(Texto: "Armadores: ", Rowspan: 1, Colspan: 5, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));

                            table.AddCell(GetCell(Texto: "Restricción: " + item.HorarioRestriccion, Rowspan: 1, Colspan: 4, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                            table.AddCell(GetCell(Texto: "Vendedor: " + item.VendedorNombre, Rowspan: 1, Colspan: 3, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));
                            table.AddCell(GetCell(Texto: "Soluciones: ", Rowspan: 1, Colspan: 3, HorizontalAlignment: 0, Border: 0, PaddingTop: 5));

                            table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 3));


                            table.AddCell(GetCell(Texto: "Contacto: " + item.ClienteNombre, Rowspan: 1, Colspan: 3, HorizontalAlignment: 0));
                            table.AddCell(GetCell(Texto: "Tel: " + item.ClienteTelefono, Rowspan: 1, Colspan: 3, HorizontalAlignment: 0));
                            table.AddCell(GetCell(Texto: "Dirección: " + item.ClienteDireccionEntrega, Rowspan: 1, Colspan: 4, HorizontalAlignment: 0));
                            table.AddCell(GetCell(Texto: "Tipo Inst.: " + item.TipoInstalacion, Rowspan: 1, Colspan: 4, HorizontalAlignment: 0));

                            table.AddCell(GetCell(Texto: "Obs. Evento: " + item.ObservacionesEvento, Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5));
                            table.AddCell(GetCell(Texto: "Obs. Torre: " + item.ObservacionesTorre, Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5));

                            table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2));

                            table.AddCell(GetCell(Texto: "CÓDIGO:", Rowspan: 1, Colspan: 1, HorizontalAlignment: 0, PaddingBottom: 5));
                            table.AddCell(GetCell(Texto: "DESCRIPCIÓN:", Rowspan: 1, Colspan: 7, HorizontalAlignment: 0, PaddingBottom: 5));
                            table.AddCell(GetCell(Texto: "BOD: ", Rowspan: 1, Colspan: 1, HorizontalAlignment: 0, PaddingBottom: 5));
                            table.AddCell(GetCell(Texto: "CANT: ", Rowspan: 1, Colspan: 1, HorizontalAlignment: 1, PaddingBottom: 5));

                            int total = 0;
                            foreach (var item2 in item.Productos)
                            {
                                if (item2.Sku != "")
                                {
                                    table.AddCell(GetCell(Texto: item2.Sku, Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                                    table.AddCell(GetCell(Texto: item2.Descripcion, Rowspan: 1, Colspan: 7, HorizontalAlignment: 0));
                                    table.AddCell(GetCell(Texto: item2.Bodega, Rowspan: 1, Colspan: 1, HorizontalAlignment: 0));
                                    table.AddCell(GetCell(Texto: item2.Cantidad, Rowspan: 1, Colspan: 1, HorizontalAlignment: 1));
                                    total += int.Parse(item2.Cantidad);
                                }
                            }

                            table.AddCell(GetCell(Texto: " ", Rowspan: 1, Colspan: 10, HorizontalAlignment: 2));

                            if (item.NumeroCaso.ToString() != "")
                            {
                                table.AddCell(GetCell(Texto: "TOTAL:", Rowspan: 1, Colspan: 9, HorizontalAlignment: 2, PaddingBottom: 10));
                                table.AddCell(GetCell(Texto: total.ToString(), Rowspan: 1, Colspan: 1, HorizontalAlignment: 1, PaddingBottom: 10));

                                table.AddCell(GetCell(Texto: "Caso: " + item.NumeroCaso, Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5));
                                table.AddCell(GetCell(Texto: "Acciones: " + item.AccionesCaso, Rowspan: 1, Colspan: 10, HorizontalAlignment: 0, PaddingTop: 5, Border: 2, PaddingBottom: 10));
                            }
                            else
                            {
                                table.AddCell(GetCell(Texto: "TOTAL:", Rowspan: 1, Colspan: 9, HorizontalAlignment: 2, Border: 2, PaddingBottom: 10));
                                table.AddCell(GetCell(Texto: total.ToString(), Rowspan: 1, Colspan: 1, HorizontalAlignment: 1, Border: 2, PaddingBottom: 10));
                            }


                            numeroDePaginaEncabezado = false;
                        }

                    }


                }

                doc.Add(table);
                doc.Close();
                byte[] result = ms.ToArray();
                return AddPageNumber(result); ;
            }
        }



        private PdfPCell GetCell(string Texto, int Rowspan = 0, int Colspan = 0, int HorizontalAlignment = 0, int VerticalAlignment = 0,  int Border = 0, int PaddingBottom = 0, int PaddingTop = 0, float Size = 7)
        {
            Phrase frase = new Phrase() {
                Font = FontFactory.GetFont(FontFactory.COURIER, size: Size)
            };
            frase.Add(Texto);


            PdfPCell cell = new PdfPCell(frase)
            {
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment   = VerticalAlignment,
                Rowspan             = Rowspan,
                Colspan             = Colspan,
                Border              = Border,
                PaddingBottom       = PaddingBottom,
                PaddingTop          = PaddingTop,
                
            };
            return cell;
        }



        byte[] AddPageNumber(byte[] byte1)
        {
            byte[] bytes = byte1;
            Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), blackFont), 568f, 15f, 0);
                    }
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }
    }
}
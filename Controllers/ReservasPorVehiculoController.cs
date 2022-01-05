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
    public class ReservasPorVehiculoController : Controller
    {
        // GET: ReservasPorVehiculo
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReservasPorVehiculo/Details/5
        [AuthorizeUser(idOperacion: 0, pantalla: 7)]
        public ActionResult Details(int id)
        {
            Utilidades.Reservas re = new Utilidades.Reservas();
            re.AnularEventosReservas();
            ReservasPorVehiculo reservas = new ReservasPorVehiculo();
            RespuestaReservaPorVehiculo respuesta = null;
            if (Request["daterange"] != null)
            {
                string [] fechas = Request["daterange"].ToString().Trim().Split('-');
                DateTime fechaf = DateTime.Parse(fechas[1].ToString().Trim(), CultureInfo.CreateSpecificCulture("en-US"));
                DateTime fechai = DateTime.Parse(fechas[0].ToString().Trim(), CultureInfo.CreateSpecificCulture("en-US"));
                respuesta = reservas.ObtenerReservas(id.ToString(), fechai, fechaf);
            }
            else
            {
                respuesta = reservas.ObtenerReservas(id.ToString(), DateTime.Now, DateTime.Now.AddDays(7));
            }
           
            return View("Details", respuesta);
        }

        // GET: ReservasPorVehiculo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservasPorVehiculo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: ReservasPorVehiculo/Create
        [HttpPost]
        [AuthorizeUser(idOperacion: 1, pantalla: 7)]
        public ActionResult ActualizarVehiculo(string id)
        {
            try
            {
                Utilidades.ReservasPorVehiculo rs = new Utilidades.ReservasPorVehiculo();
                char[] separadores = { ';', ',' };
                string[] split = id.Split(separadores);

                DateTime dtI = Convert.ToDateTime(split[0]);
                DateTime dtF = DateTime.Today;
                List<string> Temporal = new List<string>();
                string PanelId = "";
                for (int i = 1; i < split.Length; i++)
                {
                    if (split[i].Contains("card"))
                    {
                        PanelId = split[i].Split('-')[3];
                        Temporal.Add(split[i].Split('-')[1].ToString());
                    }
                    else if (DateTime.TryParse(split[i].ToString(), out dtF))
                    {
                        if (Temporal.Count>0)
                        {
                            string respuesta = rs.OrdenarPorMovimiento(Temporal, PanelId, dtI);

                            if (!respuesta.Contains("Exitoso"))
                            {
                                return Json("Error:" + respuesta);
                            }
                            
                            dtI = dtF;
                            Temporal = new List<string>();
                        }
                    }
                }
                return Json("true");
            }
            catch
            {
                return Json("false");
            }
        }

        // GET: ReservasPorVehiculo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservasPorVehiculo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservasPorVehiculo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservasPorVehiculo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Fecha()
        {
            DateTime startOfWeek = DateTime.Today.AddDays(
              (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek -
              (int)DateTime.Today.DayOfWeek);

            string result = string.Join("," + Environment.NewLine, Enumerable
              .Range(0, 7)
              .Select(i => startOfWeek
                 .AddDays(i)
                 .ToString("dd/MM/yyyy")));

            return View();
        }
    }
}

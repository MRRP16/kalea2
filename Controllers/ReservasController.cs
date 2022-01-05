﻿using kalea2.Utilidades;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class ReservasController : Controller
    {
        // GET: Reservas
        [AuthorizeUser(idOperacion: 0, pantalla: 7)]
        public ActionResult Index()
        {
            if (TempData["0"] !=null)
            {
                var t = TempData["0"].ToString();
                if (!string.IsNullOrEmpty(t))
                {
                    if (t.Contains("Error"))
                    {
                        ViewBag.Mensaje = t;
                    }
                    else if (t.Contains("Entrega"))
                    {
                        ViewBag.Exitoso = t;
                    }
                }
            }

            char[] delimitadores = { '=', '+' };
           
            string[] split = this.Request.RawUrl.Split(delimitadores);
            if (split.Length > 1)
            {
                Reservas reservas = new Reservas();
                reservas.AnularEventosReservas();
                if (string.IsNullOrEmpty(split[1]))
                {
                    var listado2 = reservas.ListadoReservas(DateTime.Today);
                    TempData["5"] = DateTime.Today.ToString("dd/MM/yyyy");
                    return View("Index", listado2);
                }
                DateTime.TryParse(split[1] + " " + split[2] + " " + split[3], out DateTime date);
                var listado = reservas.ListadoReservas(date);
                TempData["5"] = date.ToString("dd/MM/yyyy");
                return View("Index", listado);
            }
            else
            {
                Reservas reservas = new Reservas();
                reservas.AnularEventosReservas();
                var listado = reservas.ListadoReservas(DateTime.Today);
                TempData["5"] = DateTime.Today.ToString("dd/MM/yyyy");
                return View("Index", listado);
            }

        }

        // GET: Reservas/Details/5
        public ActionResult Details(int id)
        {
            //SELECT TO_CHAR(FechaInicio, 'yyyy/mm/dd hh24:mi:ss')FROM T_ENC_ENTREGAS
            Reservas reservas = new Reservas();
            var respuesta = reservas.ObtenerReservas(id);
            return View();
        }

        // GET: Reservas/Create
        [AuthorizeUser(idOperacion: 1, pantalla: 7)]
        public ActionResult Create(string Id,string fecha)
        {
            Reservas reservas = new Reservas();
            //ViewData["datos"] = datos;
            Models.Reserva datos = new Models.Reserva();
            datos.FechaEntrega = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            datos.FechaEntrega2 = datos.FechaEntrega.ToString("yyyy-MM-dd");
            datos.FechaRestriccionInicio = "08:00";
            datos.FechaRestriccionFin = "16:00";
            datos.Reserva_Articulos = new List<Models.Reserva_Detalle_Articulos>();
            datos.Reserva_Casos = new List<Models.Reserva_Detalle_Casos>();
            datos.Eventos_Articulos = reservas.ObtenerEventos();
            datos.Casos_Pendientes = reservas.ObtenerCasos();
            //datos.Vehiculo = Id;
            return View("Modal", datos);
        }

        [AuthorizeUser(idOperacion: 1, pantalla: 11)]
        public ActionResult CreateReserva(string Id, string fecha)
        {
            Reservas reservas = new Reservas();
            //ViewData["datos"] = datos;
            Models.ReservaParcial datos = new Models.ReservaParcial();
            datos.FechaEntrega = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            datos.FechaEntrega2 = datos.FechaEntrega.ToString("yyyy-MM-dd");

            //datos.Vehiculo = Id;
            return View("Reserva", datos);
        }

        [AuthorizeUser(idOperacion: 1, pantalla: 7)]
        public ActionResult CreateTraslado(string Id)
        {

            //ViewData["datos"] = datos;
            Models.Traslados datos = new Models.Traslados();
            datos.FechaEntrega = DateTime.Now;
            datos.FechaEntrega2 = DateTime.Now.ToString("yyyy-MM-dd");

            //datos.Vehiculo = Id;
            return View("Traslado", datos);
        }

        [HttpPost]
        public ActionResult CreateTraslado(string id, Models.Traslados collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (string.IsNullOrEmpty(id))
                {
                    TempData["0"] = "No ha sido seleccionado un vehiculo, por favor salga de la pantalla.";

                    return View("Traslado", collection);
                }
                else
                {
                    collection.Vehiculo = id;
                }
                if (ModelState.IsValid)
                {
                    Reservas reservas = new Reservas();
                    collection.FechaEntrega = Convert.ToDateTime(collection.FechaEntrega2);

                    string respuesta = reservas.CreateTraslado(collection, "0");
                }
                else
                {
                    var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                    TempData["0"] = "Error encontrado";
                    Reservas reservas = new Reservas();

                    return View("Traslado", collection);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateReserva(string id, Models.ReservaParcial collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (string.IsNullOrEmpty(id))
                {
                    TempData["0"] = "No ha sido seleccionado un vehiculo, por favor salga de la pantalla.";

                    return View("Reserva", collection);
                }
                else
                {
                    collection.Vehiculo = id;
                }
                if (ModelState.IsValid)
                {
                    Reservas reservas = new Reservas();
                    collection.FechaEntrega = Convert.ToDateTime(collection.FechaEntrega2);

                    string respuesta = reservas.CrearEntregaParcial(collection, "0");
                }
                else
                {
                    var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                    TempData["0"] = "Error encontrado";
                    Reservas reservas = new Reservas();

                    return View("Reserva", collection);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Reservas/Create
        [HttpPost]
        public ActionResult Create(string id, Models.Reserva collection)
        {
            try
            {
                string respuesta = string.Empty;
                // TODO: Add insert logic here
                if (string.IsNullOrEmpty(id))
                {
                    TempData["0"] = "No ha sido seleccionado un vehiculo, por favor salga de la pantalla.";
                    Reservas reservas = new Reservas();
                    if (collection.Reserva_Articulos == null)
                    {
                        collection.Reserva_Articulos = new List<Models.Reserva_Detalle_Articulos>();

                    }
                    if (collection.Reserva_Casos == null)
                    {
                        collection.Reserva_Casos = new List<Models.Reserva_Detalle_Casos>();
                    }
                    collection.FechaEntrega = Convert.ToDateTime(collection.FechaEntrega2);
                    collection.Eventos_Articulos = reservas.ObtenerEventos();
                    collection.Casos_Pendientes = reservas.ObtenerCasos();

                    return View("Modal", collection);
                }
                else
                {
                    collection.Vehiculo = id;
                }
                if (collection.Reserva_Articulos != null || collection.Reserva_Casos != null)
                {
                    if (ModelState.IsValid)
                    {
                        var r = Request[""];
                        Reservas reservas = new Reservas();
                        collection.FechaEntrega = Convert.ToDateTime(collection.FechaEntrega2);
                        respuesta = reservas.CrearEntregaDefinitiva(collection, "0");
                      
                        TempData["0"] = respuesta;
                    
                    }
                    else
                    {

                        Reservas reservas = new Reservas();
                        if (collection.Reserva_Articulos == null)
                        {
                            collection.Reserva_Articulos = new List<Models.Reserva_Detalle_Articulos>();

                        }
                        if (collection.Reserva_Casos == null)
                        {
                            collection.Reserva_Casos = new List<Models.Reserva_Detalle_Casos>();
                        }
                        collection.FechaEntrega = Convert.ToDateTime(collection.FechaEntrega2);

                        collection.Eventos_Articulos = reservas.ObtenerEventos();
                        collection.Casos_Pendientes = reservas.ObtenerCasos();

                        
                        var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Value.Errors })
                        .ToArray();
                        string Error = string.Empty;
                        foreach (var item in errors)
                        {
                            ViewBag.Mensaje += item.Errors[0].ErrorMessage + "; ";
                        }

                        return View("Modal", collection);
                    }
                }
                else
                {
                    Reservas reservas = new Reservas();
                    if (collection.Reserva_Articulos == null)
                    {
                        collection.Reserva_Articulos = new List<Models.Reserva_Detalle_Articulos>();

                    }
                    if (collection.Reserva_Casos == null)
                    {
                        collection.Reserva_Casos = new List<Models.Reserva_Detalle_Casos>();
                    }
                    collection.FechaEntrega = Convert.ToDateTime(collection.FechaEntrega2);

                    collection.Eventos_Articulos = reservas.ObtenerEventos();
                    collection.Casos_Pendientes = reservas.ObtenerCasos();
                    ViewBag.Mensaje = "Debe agregar eventos o casos a la entrega por progamar.";
                    return View("Modal", collection);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Modal", collection);
            }
        }

        // GET: Reservas/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reservas/Edit/5
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

        // GET: Reservas/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reservas/Delete/5
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

        public JsonResult GetListadoArticulos(string id)
        {
            Reservas reservas = new Reservas();
            List<Models.Reserva_Detalle_Articulos> articulos = reservas.Articulos(id);
            return Json(articulos);
        }

        public JsonResult ObtenerComentarios(string id)
        {
            Reservas reservas = new Reservas();
            string articulos = reservas.Comentarios(id);
            return Json(articulos);
        }

        public JsonResult GetCaso(string id)
        {
            Reservas reservas = new Reservas();
            List<Models.Reserva_Detalle_Casos> articulos = reservas.Caso(id);
            return Json(articulos);
        }

        [HttpPost]
        [AuthorizeUser(idOperacion: 2, pantalla: 7)]
        public ActionResult ActualizarEventos(string id)
        {
            try
            {
                // TODO: Add delete logic here
                Reservas reservas = new Reservas();
                string Respuesta = reservas.OrdenarEntregasMovimientos(id);
                return Json("true");
            }
            catch
            {
                return Json("false");
            }
        }


        public ActionResult AnteriorFecha(string tipo,string fecha)
        {
           
            if (fecha.Length > 1)
            {
                Reservas reservas = new Reservas();
                reservas.AnularEventosReservas();
                DateTime date = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (tipo.Equals("A"))
                {
                    date = date.AddDays(-1);
                }
                else
                {
                    date = date.AddDays(1);
                }
               
                var listado = reservas.ListadoReservas(date);
                TempData["5"] = date.ToString("dd/MM/yyyy");
                return View("Index", listado);
            }
            else
            {
                Reservas reservas = new Reservas();
                reservas.AnularEventosReservas();
                var listado = reservas.ListadoReservas(DateTime.Today);
                TempData["5"] = DateTime.Today.ToString("dd/MM/yyyy");
                return View("Index", listado);
            }
        }


        public JsonResult BloquerRuta(string id,string fecha)
        {
            Reservas reservas = new Reservas();
            string articulos = reservas.BloquearRuta(id,fecha);
            return Json(articulos);
        }
    }
}
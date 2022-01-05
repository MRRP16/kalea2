using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class VehiculosController : Controller
    {
        private Utilidades.Vehiculos vehiculos;
        // GET: Vehiculos
        [AuthorizeUser(idOperacion: 0, pantalla: 5)]
        public ActionResult Index()
        {
            if (TempData["0"] != null)
            {
                var t = TempData["0"].ToString();
                if (!string.IsNullOrEmpty(t))
                {
                    if (t.Contains("Error"))
                    {
                        ViewBag.Mensaje = t;
                    }
                    else if (t.Contains("Vehiculo"))
                    {
                        ViewBag.Exitoso = t;
                    }
                }
            }
            vehiculos = new Utilidades.Vehiculos();
            List<Models.Vehiculos> ListVehiculos = vehiculos.ListadoVehiculos();

            if (ListVehiculos != null)
            {
                return View(ListVehiculos);
            }
            return View();
        }

        // GET: Vehiculos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vehiculos/Create
        [AuthorizeUser(idOperacion: 1, pantalla: 5)]
        public ActionResult Create()
        {
            Models.Vehiculos vehiculo = new Models.Vehiculos();
            return View(vehiculo);
        }

        // POST: Vehiculos/Create
        [HttpPost]
        [AuthorizeUser(idOperacion: 1, pantalla: 5)]
        public ActionResult Create(Models.Vehiculos vehiculo)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                    vehiculos = new Utilidades.Vehiculos();
                    string respuesta = vehiculos.CrearVehiculo(vehiculo);
                    TempData["0"] = respuesta;
                    if (!respuesta.Contains("Error"))
                    {
                        List<Models.Vehiculos> ListVehiculos = vehiculos.ListadoVehiculos();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        
                        ViewBag.Mensaje = respuesta;
                    
                        return View(vehiculo);
                    }
                }
                else
                {
                    var errors = ModelState
                         .Where(x => x.Value.Errors.Count > 0)
                         .Select(x => new { x.Value.Errors })
                         .ToArray();
                    string Error = string.Empty;
                    foreach (var item in errors)
                    {
                        TempData["0"] += item.Errors[0].ErrorMessage + "; ";
                    }
                    return View(vehiculo);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Vehiculos/Edit/5
        [AuthorizeUser(idOperacion: 2, pantalla: 5)]
        public ActionResult Edit(int id)
        {
         
            vehiculos = new Utilidades.Vehiculos();
            Models.Vehiculos vehiculo = vehiculos.ObtenerVehiculo(id);
            return View(vehiculo);
        }

        // POST: Vehiculos/Edit/5
        [HttpPost]
        [AuthorizeUser(idOperacion: 2, pantalla: 5)]
        public ActionResult Edit(int id, Models.Vehiculos collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    vehiculos = new Utilidades.Vehiculos();
                    string respuesta = vehiculos.ActualizarVehiculo(id,collection);
                    TempData["0"] = respuesta;
                    if (!respuesta.Contains("Error"))
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    var errors = ModelState
                         .Where(x => x.Value.Errors.Count > 0)
                         .Select(x => new { x.Value.Errors })
                         .ToArray();
                    string Error = string.Empty;
                    foreach (var item in errors)
                    {
                        TempData["0"] += item.Errors[0].ErrorMessage + "; ";
                    }
                    return View(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vehiculos/Delete/5
        [AuthorizeUser(idOperacion: 3, pantalla: 5)]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here
              
                vehiculos = new Utilidades.Vehiculos();
                string respuesta = vehiculos.HabilitarInhabilitar(id);
                TempData["0"] = respuesta;
                if (!respuesta.Contains("Error"))
                {
                    return RedirectToAction("Index");
                }
               
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Vehiculos/Delete/5
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
    }
}

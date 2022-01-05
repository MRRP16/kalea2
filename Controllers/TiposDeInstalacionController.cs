using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class TiposDeInstalacionController : Controller
    {
        private Utilidades.TiposDeInstalacion tiposCaso;
        [AuthorizeUser(idOperacion: 0, pantalla: 3)]
        // GET: TiposCaso
        public ActionResult Index()
        {
            tiposCaso = new Utilidades.TiposDeInstalacion();
            List<Models.TiposDeCaso> tiposcaso = tiposCaso.ListadoTiposCaso();

            if (tiposcaso != null)
            {
                return View(tiposcaso);
            }
            return View();

        }

        // GET: TiposCaso/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TiposCaso/Create
        [AuthorizeUser(idOperacion: 1, pantalla: 3)]
        public ActionResult Crear()
        {
            Models.TiposDeCaso tipocaso = new Models.TiposDeCaso();
            return PartialView("Modal",tipocaso);
        }

        // POST: TiposCaso/Create
        [HttpPost]
        [AuthorizeUser(idOperacion: 1, pantalla: 3)]
        public ActionResult Crear(Models.TiposDeCaso collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    tiposCaso = new Utilidades.TiposDeInstalacion();
                    string respuesta = tiposCaso.CrearTipoCaso(collection);

                    if (!respuesta.Contains("Error"))
                    {
                        //List<Models.Vehiculos> ListVehiculos = tiposCaso.ListadoTiposCaso();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(collection);
                    }
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TiposCaso/Edit/5
        [AuthorizeUser(idOperacion: 2, pantalla: 3)]
        public ActionResult Edit(int id)
        {
            tiposCaso = new Utilidades.TiposDeInstalacion();
            Models.TiposDeCaso parametroGenerals = tiposCaso.ObtenerTipoCaso(id);

            return PartialView("Modal", parametroGenerals);
        }

        // POST: TiposCaso/Edit/5
        [HttpPost]
        [AuthorizeUser(idOperacion: 2, pantalla: 3)]
        public ActionResult Edit(int id, Models.TiposDeCaso collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    tiposCaso = new Utilidades.TiposDeInstalacion();
                    string respuesta = tiposCaso.ActualizarTipoCaso(id, collection);

                    if (!respuesta.Contains("Error"))
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TiposCaso/Delete/5
        [AuthorizeUser(idOperacion: 3, pantalla: 3)]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here

                tiposCaso = new Utilidades.TiposDeInstalacion();
                string respuesta = tiposCaso.HabilitarInhabilitar(id);

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

        // POST: TiposCaso/Delete/5
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

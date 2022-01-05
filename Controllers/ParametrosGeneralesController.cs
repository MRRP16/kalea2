using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class ParametrosGeneralesController : Controller
    {

        private Utilidades.ParametrosGenerales parametros;
        // GET: ParametrosGenerales
        //[Autorizaciones(Permissions = "3", Accion = "0")]
        [AuthorizeUser(idOperacion: 0, pantalla:1)]
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
                    else if (t.Contains("exitosamente"))
                    {
                        ViewBag.Exitoso = t;
                    }
                }
            }

            parametros = new Utilidades.ParametrosGenerales();
            List<Models.ParametroGeneral> parametroGenerals = parametros.ListadoParametros();

            if (parametroGenerals != null)
            {
                return View(parametroGenerals);
            }
            return View();


        }
        // GET: Clase/Details/5
        
        public ActionResult Details(int id)
        {

            return View();
        }

       
        public ActionResult Crear()
        {
            return PartialView("Modal");
        }

        // POST: ParametrosGenerales/Create
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

        // GET: ParametrosGenerales/Edit/5
        [AuthorizeUser(idOperacion: 2, 1)]
        public ActionResult Edit(int id)
        {

            parametros = new Utilidades.ParametrosGenerales();
            Models.ParametroGeneral parametroGenerals = parametros.ObtenerParametroGeneral(id.ToString());

            return PartialView("Modal", parametroGenerals);
            //return View();
        }

        // POST: ParametrosGenerales/Edit/5
        [HttpPost]
        [AuthorizeUser(idOperacion: 2, 1)]
        public ActionResult Edit( Models.ParametroGeneral collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    parametros = new Utilidades.ParametrosGenerales();
                    string respuesta = parametros.ActualizarParametroGeneral(collection);

                    if (!respuesta.Contains("Error"))
                    {
                        TempData["0"] = respuesta;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["0"] = respuesta;
                        return PartialView("Modal", collection);
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
                        ViewBag.Mensaje += item.Errors[0].ErrorMessage + "; ";
                    }
                    TempData["0"] = ViewBag.Mensaje;
                    return PartialView("Modal", collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: ParametrosGenerales/Delete/5
        public ActionResult Delete()
        {
            //string u = Request.Form[""].ToString();
            return View();
        }

        // POST: ParametrosGenerales/Delete/5
        [HttpPost]
        public ActionResult Delete( FormCollection collection)
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

using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class RolesController : Controller
    {

        private Utilidades.Roles roles;
        // GET: Roles
          [AuthorizeUser(idOperacion: 0, pantalla: 2)]
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
                    else if (t.Contains("Rol"))
                    {
                        ViewBag.Exitoso = t;
                    }
                }
            }

            roles = new Utilidades.Roles();
            List<Models.Roles> parametroGenerals = roles.ListadoRoles();

            if (parametroGenerals != null)
            {
                return View(parametroGenerals);
            }
            return View();
        }

        // GET: Roles/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Roles/Create
        [AuthorizeUser(idOperacion: 1, pantalla: 2)]
        public ActionResult Create()
        {
            roles = new Utilidades.Roles();
            Models.Roles rol = roles.ObtenerModeloRolCrear();
            return View(rol);
        }

        // POST: Roles/Create
        [HttpPost]
        [AuthorizeUser(idOperacion: 1, pantalla: 2)]
        public ActionResult Create(Models.Roles collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    roles = new Utilidades.Roles();
                    string respuesta = roles.CrearRol(collection);
                    TempData["0"] = respuesta;
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

        // GET: Roles/Edit/5
        [AuthorizeUser(idOperacion: 2, pantalla: 2)]
        public ActionResult Edit(int id)
        {
            roles = new Utilidades.Roles();
            Models.Roles rol = roles.ObtenerModeloRolEditar(id);

            return View(rol);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [AuthorizeUser(idOperacion: 2, pantalla: 2)]
        public ActionResult Edit(int id, Models.Roles collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    roles = new Utilidades.Roles();
                    string respuesta = roles.ActualizarRol(id,collection);
                    if (respuesta.Contains("Error"))
                    {
                        TempData["0"] = respuesta;
                        return View(collection);
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
                return View(collection);
            }
        }

        // GET: Roles/Delete/5
        [AuthorizeUser(idOperacion: 3, pantalla: 2)]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here

                roles = new Utilidades.Roles();
                string respuesta = roles.HabilitarInhabilitar(id);

                if (!respuesta.Contains("Error"))
                {
                    TempData["0"] = respuesta;
                    return RedirectToAction("Index");
                }
                TempData["0"] = respuesta;

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Roles/Delete/5
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

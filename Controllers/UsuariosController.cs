using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        private Utilidades.Usuarios usuarios;
        [AuthorizeUser(idOperacion: 0, pantalla: 4)]
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
                    else if (t.Contains("Usuario"))
                    {
                        ViewBag.Exitoso = t;
                    }
                }
            }
            usuarios = new Utilidades.Usuarios();
            List<Models.Usuarios> LisUsuarios = usuarios.ListadoDeUsuarios();

            if (LisUsuarios != null)
            {
                return View(LisUsuarios);
            }
            return View();
        }

        // GET: Usuarios/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Usuarios/Create
        [AuthorizeUser(idOperacion: 1, pantalla: 4)]
        public ActionResult Create()
        {
            usuarios = new Utilidades.Usuarios();
            Models.Usuarios usuario = new Models.Usuarios();
            usuario.Roles = usuarios.GetRolesCreate();
            usuario._Usuarios = usuarios.GetUsuarios();
            return View(usuario);
        }

        // POST: Usuarios/Create
        [HttpPost]
        [AuthorizeUser(idOperacion: 1, pantalla: 4)]
        public ActionResult Create(Models.Usuarios collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                  
                    usuarios = new Utilidades.Usuarios();
                    string respuesta = usuarios.CrearUsuario(collection);

                    TempData["0"] = respuesta;
                    if (!respuesta.Contains("Error"))
                    {
                       // List<Models.Usuarios> ListVehiculos = usuarios.ListadoDeUsuarios();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        collection.Roles = usuarios.GetRolesCreate();
                        collection._Usuarios = usuarios.GetUsuarios();
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
                    usuarios = new Utilidades.Usuarios();
                    collection.Roles = usuarios.GetRolesCreate();
                    collection._Usuarios = usuarios.GetUsuarios();
                    return View(collection);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuarios/Edit/5
        [AuthorizeUser(idOperacion: 2, pantalla: 4)]
        public ActionResult Edit(int id)
        {

            usuarios = new Utilidades.Usuarios();
            Models.Usuarios vehiculo = usuarios.ObtenerUsuario(id);
            return View(vehiculo);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [AuthorizeUser(idOperacion: 2, pantalla: 4)]
        public ActionResult Edit(int id, Models.Usuarios collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    //if (string.IsNullOrEmpty(collection.Contrasenia))
                    //{
                    //    TempData["0"] = "Debe ingresar una contraseña para el usuario";
                    //    collection.Roles = usuarios.GetRolesCreate();
                    //    return View(collection);
                    //}
                    usuarios = new Utilidades.Usuarios();
                    if (collection.NombreUsuario.Contains(" "))
                    {
                        TempData["0"] = "El nombre de usuario no puede contener espacios en blanco";
                        collection.Roles = usuarios.GetRolesCreate();
                        return View(collection);
                    }
                    string respuesta = usuarios.ActualizarUsuario(id, collection);
                    TempData["0"] = respuesta;
                    if (!respuesta.Contains("Error"))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        collection.Roles = usuarios.GetRolesCreate();
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
                    usuarios = new Utilidades.Usuarios();
                    collection.Roles = usuarios.GetRolesCreate();
                    return View(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuarios/Delete/5
        [AuthorizeUser(idOperacion: 3, pantalla: 4)]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here

                usuarios = new Utilidades.Usuarios();
                string respuesta = usuarios.HabilitarInhabilitar(id);
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

        // POST: Usuarios/Delete/5
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

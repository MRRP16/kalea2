using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            Models.Login login = new Models.Login();
            return View(login);
        }

        [HttpPost]
        public ActionResult Index(Models.Login collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Utilidades.Usuarios usuarios = new Utilidades.Usuarios();
                    Models.Usuarios usr = usuarios.VerificaLogin(collection.NombreUsuario);
                    //Models.Usuarios usr = usuarios.VerificarLoginkalea2(collection.NombreUsuario);
                    if (collection.NombreUsuario == usr.NombreUsuario)
                    {
                        if (usr.Estado.Equals("A"))
                        {
                            OdbcConnection conexion = new OdbcConnection($"DSN={ConfigurationManager.AppSettings["DSN"]};Uid={collection.NombreUsuario};Pwd={collection.Contraseña};");
                            conexion.Open();
                            conexion.Close();
                            Session["User"] = usr;
                            Session["UserName"] = usr.Nombre + " " + usr.Apellido;
                            return RedirectToAction("Index", "Reservas");
                        }
                        else
                        {
                            if (collection.NombreUsuario.Equals(ConfigurationManager.AppSettings["user"]) && collection.Contraseña.Equals(ConfigurationManager.AppSettings["password"]))
                            {
                                Models.Usuarios usr2 = new Models.Usuarios();
                                usr2.Codigo = -1;
                                usr2.Nombre = collection.NombreUsuario;
                                usr2.Apellido = collection.NombreUsuario;
                                Session["User"] = usr2;
                                Session["UserName"] = collection.NombreUsuario + " " + collection.NombreUsuario;
                                return RedirectToAction("Index", "Reservas");
                            }
                            else
                            {
                                ViewBag.Mensaje = "Error: Usuario inactivo.";
                                return View(collection);
                            }
                           
                        }
                        
                    }
                    else
                    {
                        if (collection.NombreUsuario.Equals(ConfigurationManager.AppSettings["user"]) && collection.Contraseña.Equals(ConfigurationManager.AppSettings["password"]))
                        {
                            Models.Usuarios usr2 = new Models.Usuarios();
                            usr2.Codigo = -1;
                            usr2.Nombre = collection.NombreUsuario;
                            usr2.Apellido = collection.NombreUsuario;
                            Session["User"] = usr2;
                            Session["UserName"] = collection.NombreUsuario + " " + collection.NombreUsuario;
                            return RedirectToAction("Index", "Reservas");
                        }
                        else
                        {
                            ViewBag.Mensaje = "Error: Usuario no registrado.";
                            return View(collection);
                        }
                        
                    }
                }
                else
                {
                    ViewBag.Mensaje = "Error: Usuario o contraseña invalida.";
                    return View(collection);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("invalid"))
                {
                    ViewBag.Mensaje = "Error: Usuario o contraseña invalida. ";
                }
                else
                {
                    ViewBag.Mensaje = "Error: " + e.Message;
                }

                ViewBag.Error = e.Message;
                return View();
            }

        }


        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}
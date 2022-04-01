using kalea2.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class CasosEntregasController : Controller
    {
        Utilidades.CasosEntregas CasosEntregas;
        // GET: CasosEntregas
        [AuthorizeUser(idOperacion: 0, pantalla: 6)]
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
                    else if (t.Contains("Entrega"))
                    {
                        ViewBag.Exitoso = t;
                    }
                }
            }

            Utilidades.Reservas re = new Utilidades.Reservas();
            re.AnularEventosReservas();

            CasosEntregas = new Utilidades.CasosEntregas();

            string[] split = this.Request.RawUrl.Split('=');
            if (split.Length>1)
            {
                if (!string.IsNullOrEmpty(split[1]))
                {
                    Models.Reserva reserva = CasosEntregas.GetEntrega(split[1].ToString());
                   
                    if (!string.IsNullOrEmpty(reserva.Id.ToString()))
                    {
                        List<Models.Reserva> listado = new List<Models.Reserva>();
                        listado.Add(reserva);
                        return View(listado);
                    }
                    else
                    {
                        List<Models.Reserva> listado = new List<Models.Reserva>();
                        return View( listado);
                    }
                }
            }
            return View(CasosEntregas.Entregas());
        }

        public ActionResult Create()
        {
            Models.CasosEntregas casoEntrega = new Models.CasosEntregas();
            return View(casoEntrega);
        }

        [AuthorizeUser(idOperacion: 2, pantalla: 6)]
        public ActionResult Edit(int id)
        {

            CasosEntregas = new Utilidades.CasosEntregas();

            Models.Reserva reserva = CasosEntregas.GetEntrega(id.ToString());

            if (!string.IsNullOrEmpty(reserva.Id.ToString()))
            {
                return View("Modal", reserva);
            }
            return RedirectToAction("Index");
           
        }

        [HttpPost]
        public ActionResult Edit(int id, Models.Reserva reserva)
        {
            try
            {
                CasosEntregas = new Utilidades.CasosEntregas();
                if (ModelState.IsValid)
                {
                    if (reserva.Reserva_Articulos == null && reserva.Reserva_Casos == null)
                    {
                        if (reserva.Reserva_Articulos.Count == 0)
                        {
                            ViewBag.Alerta = "Debe seleccionar al menos 1 evento o 1 caso para la entrega";
                            Models.Reserva reservatemp = CasosEntregas.GetEntrega(id.ToString());
                            reserva.FechaEntrega = Convert.ToDateTime(reserva.FechaEntrega2);
                            reserva.Casos_Pendientes = reservatemp.Casos_Pendientes;
                            reserva.Eventos_Articulos = reservatemp.Eventos_Articulos;
                            reserva.Reserva_Articulos = reservatemp.Reserva_Articulos;
                            reserva.Reserva_Casos = reservatemp.Reserva_Casos;

                            return View("Modal", reserva);
                        }
                    }
                    //else
                    //{
                    //    ViewBag.Alerta = "Debe seleccionar al menos 1 evento o 1 caso para la entrega";
                    //    Models.Reserva reservatemp = CasosEntregas.GetEntrega(id.ToString());
                    //    reserva.FechaEntrega = Convert.ToDateTime(reserva.FechaEntrega2);
                    //    reserva.Casos_Pendientes = reservatemp.Casos_Pendientes;
                    //    reserva.Eventos_Articulos = reservatemp.Eventos_Articulos;
                    //    reserva.Reserva_Articulos = reservatemp.Reserva_Articulos;
                    //    reserva.Reserva_Casos = reservatemp.Reserva_Casos;
                    //    return View("Modal", reserva);
                    //}
                    
                    Utilidades.Reservas reservas = new Utilidades.Reservas();
                    reserva.FechaEntrega = Convert.ToDateTime(reserva.FechaEntrega2);
                    TempData["0"] =  reservas.EditarEntregaDefinitiva(reserva, "0", id.ToString());
                }
                else
                {
                    var errors = ModelState
                       .Where(x => x.Value.Errors.Count > 0)
                       .Select(x => new { x.Key, x.Value.Errors })
                       .ToArray();
                    Utilidades.Reservas reservas = new Utilidades.Reservas();
                    if (!string.IsNullOrEmpty(reserva.Id.ToString()))
                    {
                        Models.Reserva reservatemp = CasosEntregas.GetEntrega(id.ToString());
                        reserva.FechaEntrega = Convert.ToDateTime(reserva.FechaEntrega2);
                        reserva.Casos_Pendientes = reservatemp.Casos_Pendientes;
                        reserva.Eventos_Articulos = reservatemp.Eventos_Articulos;
                       
                        string Error = string.Empty;
                        foreach (var item in errors)
                        {
                            ViewBag.Mensaje += item.Errors[0].ErrorMessage + "; ";
                        }

                        return View("Modal", reserva);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                ViewBag.Alerta = "Ocurrio un error inesperado en la aplicacion.";
                Models.Reserva reservatemp = CasosEntregas.GetEntrega(id.ToString());
                reserva.FechaEntrega = Convert.ToDateTime(reserva.FechaEntrega2);
                reserva.Casos_Pendientes = reservatemp.Casos_Pendientes;
                reserva.Eventos_Articulos = reservatemp.Eventos_Articulos;
                reserva.Reserva_Articulos = reservatemp.Reserva_Articulos;
                reserva.Reserva_Casos = reservatemp.Reserva_Casos;
                return View("Modal", reserva);
            }
           
        }


        [HttpPost]
        public ActionResult Filtro(int id)
        {
            CasosEntregas = new Utilidades.CasosEntregas();

            Models.Reserva reserva = CasosEntregas.GetEntrega(id.ToString());

            if (!string.IsNullOrEmpty(reserva.Id.ToString()))
            {
                List<Models.Reserva> listado = new List<Models.Reserva>();
                listado.Add(reserva);
                return PartialView("Tabla", reserva);
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        [AuthorizeUser(idOperacion: 3, pantalla: 6)]
        public ActionResult Delete(int id)
        {
            try
            {
                CasosEntregas = new Utilidades.CasosEntregas();

                string respuesta = CasosEntregas.AnularEntrega(id.ToString());
                CasosEntregas.RordenarTrasAnulacion(id.ToString());
                return Json(respuesta);
            }
            catch
            {
                return Json("false");
            }
        }
    }
}
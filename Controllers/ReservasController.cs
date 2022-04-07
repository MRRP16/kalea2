using kalea2.Utilidades;

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
            if (TempData["7"] != null)
            {
                Reservas reservas = new Reservas();
                DateTime dt = Convert.ToDateTime(TempData["7"].ToString());
                var listado = reservas.ListadoReservas(dt);
                TempData["5"] = dt.ToString("dd/MM/yyyy");
                return View("Index", listado);
            }
            else
            {
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
            datos.TiposDeInstalacion = reservas.TiposDeInstalaciones();
            //datos.Eventos_Articulos = new List<SelectListItem>();
            //datos.Casos_Pendientes = new List<SelectListItem>();

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
                if (collection.TipoDeInstalacion ==null)
                {
                    collection.TipoDeInstalacion = "0";
                }
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
                    collection.TiposDeInstalacion = reservas.TiposDeInstalaciones();
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
                        TempData["7"] = collection.FechaEntrega2;
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
                        collection.TiposDeInstalacion = reservas.TiposDeInstalaciones();

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
                    collection.TiposDeInstalacion = reservas.TiposDeInstalaciones();
                    ViewBag.Mensaje = "Debe agregar eventos o casos a la entrega por programar.";
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



        //public ActionResult GetListadoArticulos(string id)
        //{
        //    Reservas reservas = new Reservas();
        //    List<Models.Reserva_Detalle_Articulos> articulos = reservas.Articulos(id);
        //    Models.Reserva r = new Models.Reserva();
        //    r.Reserva_Articulos = articulos;
        //    return View("GetListadoArticulos", r);
        //}

        public JsonResult ObtenerCasosEventosLoad()
        {
            dynamic Listado = new System.Dynamic.ExpandoObject();
            Reservas reservas = new Reservas();
            //datos.Casos_Pendientes = 
            Listado.Casos = reservas.ObtenerCasos();
            Listado.Eventos = reservas.ObtenerEventos();

            return Json(Listado);
        }

        public JsonResult ObtenerComentarios(string id)
        {
            Reservas reservas = new Reservas();
            string[] sp = id.Split('|');

            Models.ListadoArticulosUpdate la = new Models.ListadoArticulosUpdate();
            List<Models.Reserva_Detalle_Articulos> articulosdet = new List<Models.Reserva_Detalle_Articulos>();
            if (!id.Equals("||||||"))
            {
                for (int i = 0; i < sp.Length; i++)
                {
                    string[] spGeneral = sp[i].TrimEnd(',').Split(',');
                    for (int j = 0; j < spGeneral.Length; j++)
                    {
                        switch (i)
                        {
                            case 0:

                                Models.Reserva_Detalle_Articulos articulo = new Models.Reserva_Detalle_Articulos();
                                articulo.Id = j;
                                string[] numNumEvento = spGeneral[j].ToString().Split((char)39);
                                articulo.NumEvento = numNumEvento[1];
                                articulosdet.Add(articulo);


                                break;
                            case 1:

                                articulosdet[j].CodigoArticulo = spGeneral[j].ToString();

                                break;
                            case 2:

                                articulosdet[j].Descripcion = spGeneral[j].ToString();

                                break;
                            case 3:

                                articulosdet[j].Cantidad = Convert.ToInt32(spGeneral[j].ToString());

                                break;
                            case 4:

                                articulosdet[j].TiempoArmado = spGeneral[j].ToString();
                                break;
                            case 5:


                                break;
                            case 6:
                                articulosdet[j].EstadoArticulo = spGeneral[j].ToString();

                                break;

                        }
                    }

                }

                la.articulosdet = articulosdet;
                la.Comentarios = reservas.Comentarios(sp[0]);
            }
            else
            {
                la.articulosdet = articulosdet;
                la.Comentarios = "";
            }
          
           
            return Json(la);
        }

        public JsonResult OrdenarCasos(string id)
        {
            List<Models.ListadoCasos> CasosDet = new List<Models.ListadoCasos>();
            string[] sp = id.Split('|');

            for (int i = 0; i < sp.Length; i++)
            {
                string[] spGeneral = sp[i].TrimEnd(',').Split('\\');
                for (int j = 0; j < spGeneral.Length; j++)
                {
                    switch (i)
                    {
                        case 0:
                            if (spGeneral[j].ToString()!="")
                            {
                                Models.ListadoCasos articulo = new Models.ListadoCasos();
                                articulo.Caso = spGeneral[j].ToString();
                                CasosDet.Add(articulo);
                            }
                            break;
                        case 1:
                            if (spGeneral[j].ToString() != "")
                            {
                                CasosDet[j].Acciones = spGeneral[j].ToString();
                            }
                                
                            break;
                    }
                }
            }
            return Json(CasosDet);
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

        [AuthorizeUser(idOperacion: 1, pantalla: 12)]
        public JsonResult BloquerRuta(string id,string fecha)
        {
            Reservas reservas = new Reservas();
            string articulos = reservas.BloquearRuta(id,fecha);
            return Json(articulos);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Controllers
{
    public class BloqueoRutasController : Controller
    {
        // GET: BloqueoRutas
        public ActionResult Index()
        {
            Utilidades.BloqueRutas bl = new Utilidades.BloqueRutas();

            List<Models.BloqueoRutas> LIST = bl.Listado();

            return View(LIST);
        }


        public JsonResult DesbloquearRuta(string id, string fecha)
        {
            Utilidades.BloqueRutas bl = new Utilidades.BloqueRutas();
            string articulos = bl.BloquearRuta(id, fecha);
            return Json(articulos);
        }


       
    }
}

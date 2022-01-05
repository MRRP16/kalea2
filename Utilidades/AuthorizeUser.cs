using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kalea2.Models;
namespace kalea2.Utilidades
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeUser : AuthorizeAttribute
    {

        private kalea2.Models.Usuarios oUsuario = null;
        private int idOperacion;
        private int Pantalla;
        
        
        public AuthorizeUser(int idOperacion = 0, int pantalla=0)
        {
            this.idOperacion = idOperacion;
            this.Pantalla = pantalla;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            String nombreOperacion = "";
            String nombreModulo = "";
            try
            {
                oUsuario = (kalea2.Models.Usuarios)HttpContext.Current.Session["User"];
                if (oUsuario!=null)
                {
                    //if (idOperacion != 0 && Pantalla !=0)
                    //{
                        if (oUsuario.Codigo!=-1)
                        {
                            Utilidades.conexionDB conexion = new conexionDB();
                            string query = string.Empty;
                            switch (idOperacion)
                            {
                                case 0:
                                    query = string.Format(@"SELECT COUNT(*) AS CANTIDAD FROM T_USUARIOS T0
                                                INNER JOIN T_ROLES T1 ON T0.IDROL = T1.ID
                                                INNER JOIN T_ROLES_PERMISOS T2 ON T1.ID = T2.CODIGOROL
                                                WHERE T0.ID = {0}
                                                AND T1.ID = {1}
                                                AND T2.VISUALIZAR = {2}
                                                AND T2.CODIGOPERMISO = {3}", oUsuario.Codigo, oUsuario.Rol, "1", Pantalla);
                                    break;
                                case 1:
                                    query = string.Format(@"SELECT COUNT(*) AS CANTIDAD FROM T_USUARIOS T0
                                                INNER JOIN T_ROLES T1 ON T0.IDROL = T1.ID
                                                INNER JOIN T_ROLES_PERMISOS T2 ON T1.ID = T2.CODIGOROL
                                                WHERE T0.ID = {0}
                                                AND T1.ID = {1}
                                                AND T2.CREAR = {2}
                                                AND T2.CODIGOPERMISO = {3}", oUsuario.Codigo, oUsuario.Rol, "1", Pantalla);
                                    break;
                                case 2:
                                    query = string.Format(@"SELECT COUNT(*) AS CANTIDAD FROM T_USUARIOS T0
                                                INNER JOIN T_ROLES T1 ON T0.IDROL = T1.ID
                                                INNER JOIN T_ROLES_PERMISOS T2 ON T1.ID = T2.CODIGOROL
                                                WHERE T0.ID = {0}
                                                AND T1.ID = {1}
                                                AND T2.EDITAR = {2}
                                                AND T2.CODIGOPERMISO = {3}", oUsuario.Codigo, oUsuario.Rol, "1", Pantalla);
                                    break;
                                case 3:
                                    query = string.Format(@"SELECT COUNT(*) AS CANTIDAD FROM T_USUARIOS T0
                                                INNER JOIN T_ROLES T1 ON T0.IDROL = T1.ID
                                                INNER JOIN T_ROLES_PERMISOS T2 ON T1.ID = T2.CODIGOROL
                                                WHERE T0.ID = {0}
                                                AND T1.ID = {1}
                                                AND T2.ELIMINAR = {2}
                                                AND T2.CODIGOPERMISO = {3}", oUsuario.Codigo, oUsuario.Rol, "1", Pantalla);
                                    break;

                            }

                            var resultado = conexion.ConsultarDB(query, "T_PARAMETROS_GENERALES");

                            string Respuesta = string.Empty;
                            if (resultado.Tables[0].Rows.Count >= 0)
                            {
                                foreach (DataRow item in resultado.Tables[0].Rows)
                                {
                                    Respuesta = item["CANTIDAD"].ToString();
                                }
                                switch (Respuesta)
                                {
                                    case "1":
                                        break;
                                    default:
                                        filterContext.Result = new RedirectResult("/Error/UnauthorizedOperation?operacion=" + nombreOperacion + "&modulo=" + nombreModulo + "&msjeErrorExcepcion=");
                                        break;
                                }

                            }
                        }
                        
                    //}
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Login");
                }
               
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("/Error/UnauthorizedOperation?operacion=" + nombreOperacion + "&modulo=" + nombreModulo + "&msjeErrorExcepcion=" + ex.Message);
            }
        }

    }
}
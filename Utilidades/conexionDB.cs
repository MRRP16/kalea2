using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Configuration;

namespace kalea2.Utilidades
{
    public class conexionDB
    {

        public DataSet ConsultarDB(string Query, string Tabla)
        {
            OdbcConnection myConnection = new OdbcConnection();
            try
            {
                
                DataSet ds = new DataSet();
                
                myConnection.ConnectionString = strgConexion();
                myConnection.Open();

                OdbcDataAdapter adapter = new OdbcDataAdapter(Query, myConnection);
                adapter.SelectCommand.CommandTimeout = 180;
                adapter.Fill(ds, Tabla);
                myConnection.Close();
                myConnection.Dispose();
                return ds;
            }
            catch (Exception ex)
            {

                return null;
            }
            finally {
                myConnection.Close();
            }

        }


            public string strgConexion()
        {
            Seguridad seguridad = new Seguridad();

            //string myConnectionString = @"DSN=" + seguridad.Desencripta(_env.GetSection("Basedatos:DSN").Value) +
            //    ";Uid=" + seguridad.Desencripta(_env.GetSection("Basedatos:Usuario").Value) + ";Pwd=" + seguridad.Desencripta(_env.GetSection("Basedatos:Contrasenia").Value) + ";";


            string myConnectionString = string.Format("DSN={0};Uid={1};Pwd={2};",ConfigurationManager.AppSettings["DSN"], ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            return myConnectionString;

        }


        public void cerrarConexionDB()
        {
            //this.con.Close();
        }

    }
}
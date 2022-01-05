using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using kalea2.Utilidades;
namespace kalea2.Utilidades
{
    public class ParametrosGenerales :conexionDB
    {
        conexionDB dB;
        public List<Models.ParametroGeneral> ListadoParametros()
        {
            dB = new conexionDB();

            List<Models.ParametroGeneral> ListadoParametrosGenerales = null;
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_PARAMETROS_GENERALES;", "T_PARAMETROS_GENERALES");

                ListadoParametrosGenerales = new List<Models.ParametroGeneral>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.ParametroGeneral parametroGeneral = new Models.ParametroGeneral();
                    parametroGeneral.Codigo = Convert.ToInt32(item["Id"].ToString());
                    parametroGeneral.Descripcion = item["Descripcion"].ToString();
                    parametroGeneral.Valor = item["Valor"].ToString();
                    ListadoParametrosGenerales.Add(parametroGeneral);
                }


                return ListadoParametrosGenerales;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Models.ParametroGeneral ObtenerParametroGeneral(string Id)
        {
            try
            {
                string Query = string.Format("SELECT * FROM T_PARAMETROS_GENERALES WHERE Id = '{0}';", Id);
                dB = new conexionDB();
                var resultado = dB.ConsultarDB(Query, "T_PARAMETROS_GENERALES");

                Models.ParametroGeneral parametroGeneral = new Models.ParametroGeneral();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    parametroGeneral.Codigo = Convert.ToInt32(item["Id"].ToString());
                    parametroGeneral.Descripcion = item["Descripcion"].ToString();
                    parametroGeneral.Valor = item["Valor"].ToString();
                }
                return parametroGeneral;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public string ActualizarParametroGeneral(Models.ParametroGeneral model)
        {
            using (OdbcConnection myConnection = new OdbcConnection(strgConexion()))
            {

                OdbcCommand commandInsertar = myConnection.CreateCommand();
                OdbcTransaction transaction = null;
                // Set the Connection to the new OdbcConnection.
                commandInsertar.Connection = myConnection;
                try
                {
                    myConnection.Open();

                    // Start a local transaction
                    transaction = myConnection.BeginTransaction();

                    commandInsertar.Connection = myConnection;
                    commandInsertar.Transaction = transaction;

                    string Query = string.Format("UPDATE T_PARAMETROS_GENERALES SET Descripcion = '{0}', Valor='{1}' WHERE Id = '{2}'",model.Descripcion,model.Valor,model.Codigo);


                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();
                    transaction.Commit();
                    return "Parametro general actualizado exitosamente";
                }
                catch (Exception e)
                {
                    try
                    {
                        // Attempt to roll back the transaction.
                        transaction.Rollback();
                        return "Error:" + e.Message;
                    }
                    catch
                    {
                        return "Error:" + e.Message;
                        // Do nothing here; transaction is not active.
                    }
                }
            }

        }


    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class BloqueRutas : conexionDB
    {
        conexionDB dB;

        public List<Models.BloqueoRutas> Listado()
        {
            List<Models.BloqueoRutas> list = new List<Models.BloqueoRutas>();
            try
            {
                dB = new conexionDB();
                var resultado = dB.ConsultarDB(@"SELECT T0.*,T1.DESCRIPCION  FROM T_BLOQUEO_RUTAS T0
                                                 LEFT JOIN T_VEHICULOS T1 ON T0.IDVEHICULO = T1.ID; ", "T_VEHICULOS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.BloqueoRutas vehiculo = new Models.BloqueoRutas();
                    vehiculo.Descripcion = item["DESCRIPCION"].ToString();
                    vehiculo.Vehiculo = item["IdVehiculo"].ToString();
                    vehiculo.Fecha =Convert.ToDateTime(item["Fecha"].ToString());
                    vehiculo._Fecha = Convert.ToDateTime(item["Fecha"].ToString()).ToString("dd/MM/yyyy");
                    list.Add(vehiculo);
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        
        }

        public string BloquearRuta(string id, string fecha)
        {
            try
            {
                dB = new conexionDB();
                DateTime dt = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string query = string.Format(@"DELETE FROM T_BLOQUEO_RUTAS WHERE IDVEHICULO = '{0}'
                                              AND FECHA >= to_timestamp('{1} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') 
                                              AND FECHA <= to_timestamp('{1} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')", id, dt.ToString("yyyy-MM-dd"));

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

                        commandInsertar.CommandText = query;
                        commandInsertar.ExecuteNonQuery();

                        transaction.Commit();
                        return "Exitoso";
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
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }


        }
    }
}
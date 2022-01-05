using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class TiposDeInstalacion :conexionDB
    {
        conexionDB dB;
        public List<Models.TiposDeCaso> ListadoTiposCaso()
        {
            dB = new conexionDB();

            List<Models.TiposDeCaso> ListadoTiposCaso = null;
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_TIPOS_CASO;", "T_VEHICULOS");

                ListadoTiposCaso = new List<Models.TiposDeCaso>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.TiposDeCaso vehiculo = new Models.TiposDeCaso();
                    vehiculo.Codigo = Convert.ToInt32(item["Id"].ToString());
                    vehiculo.Descripcion = item["Descripcion"].ToString();
                    vehiculo.Estado = item["Estado"].ToString();
                    ListadoTiposCaso.Add(vehiculo);
                }
                return ListadoTiposCaso;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string CrearTipoCaso(Models.TiposDeCaso vehiculo)
        {
            dB = new conexionDB();


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

                    string Query = "INSERT INTO T_TIPOS_CASO(Descripcion,FechaCreacion,Estado) VALUES (?,?,?)";

                    commandInsertar.Parameters.AddWithValue("@Descripcion ", SqlDbType.VarChar).Value = vehiculo.Descripcion;
                    commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Today;
                    commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";
  

                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Vehiculo creado exitosamente";
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

        public Models.TiposDeCaso ObtenerTipoCaso(int id)
        {
            dB = new conexionDB();

            Models.TiposDeCaso tipocaso = new Models.TiposDeCaso();
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_TIPOS_CASO WHERE Id = '" + id + "';", "T_VEHICULOS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {

                    tipocaso.Codigo = Convert.ToInt32(item["Id"].ToString());
                    tipocaso.Descripcion = item["Descripcion"].ToString();
                    tipocaso.Estado = item["Estado"].ToString();
                }

                return tipocaso;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string ActualizarTipoCaso(int id, Models.TiposDeCaso vehiculos)
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

                    string Query = string.Format("UPDATE T_TIPOS_CASO SET Descripcion ='{0}' WHERE Id = '{1}'",
                        vehiculos.Descripcion,id);
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Tipo de caso actualizado exitosamente";
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

        public string HabilitarInhabilitar(int id)
        {
            dB = new conexionDB();
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


                    var resultado = dB.ConsultarDB("SELECT Estado FROM T_TIPOS_CASO WHERE Id = '" + id + "';", "T_VEHICULOS");

                    string estado = "A";
                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {
                        estado = item["Estado"].ToString();
                    }
                    string Query = string.Empty;
                    switch (estado)
                    {
                        case "A":
                            Query = string.Format("UPDATE T_TIPOS_CASO SET Estado = 'I' WHERE Id = '{0}'", id);
                            break;
                        default:
                            Query = string.Format("UPDATE T_TIPOS_CASO SET Estado = 'A' WHERE Id = '{0}'", id);
                            break;
                    }
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Tipo de caso actualizado exitosamente";
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class Vehiculos :conexionDB
    {

        conexionDB dB;
        public List<Models.Vehiculos> ListadoVehiculos()
        {
            dB = new conexionDB();

            List<Models.Vehiculos> ListadoVehiculos = null;
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_VEHICULOS;", "T_VEHICULOS");

                ListadoVehiculos = new List<Models.Vehiculos>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Vehiculos vehiculo = new Models.Vehiculos();
                    vehiculo.Codigo = Convert.ToInt32(item["Id"].ToString());
                    vehiculo.Descripcion = item["Descripcion"].ToString();
                    vehiculo.Placa = item["Placa"].ToString();
                    vehiculo.PesoCarga = item["PesoCarga"].ToString();
                    vehiculo.VolumenCarga = item["VolumenCarga"].ToString();
                    vehiculo.Piloto = item["Piloto"].ToString();
                    vehiculo.Placa = item["Placa"].ToString();
                    vehiculo.Estado = item["Estado"].ToString();
                    vehiculo.TrasladoSiNo = item["Traslado"].ToString();
                    ListadoVehiculos.Add(vehiculo);
                }


                return ListadoVehiculos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Models.Vehiculos> ListadoVehiculosNobloqueados(DateTime fecha)
        {
            dB = new conexionDB();

            List<Models.Vehiculos> ListadoVehiculos = null;
            try
            {

                string Query = string.Format(@"SELECT T0.* FROM T_VEHICULOS T0
                                WHERE T0.ID NOT IN (
                                SELECT IDVEHICULO FROM T_BLOQUEO_RUTAS 
                                WHERE Fecha >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') 
                                AND Fecha <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')
                                );", fecha.ToString("yyyy-MM-dd"));
                var resultado = dB.ConsultarDB(Query, "T_VEHICULOS");

                ListadoVehiculos = new List<Models.Vehiculos>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Vehiculos vehiculo = new Models.Vehiculos();
                    vehiculo.Codigo = Convert.ToInt32(item["Id"].ToString());
                    vehiculo.Descripcion = item["Descripcion"].ToString();
                    vehiculo.Placa = item["Placa"].ToString();
                    vehiculo.PesoCarga = item["PesoCarga"].ToString();
                    vehiculo.VolumenCarga = item["VolumenCarga"].ToString();
                    vehiculo.Piloto = item["Piloto"].ToString();
                    vehiculo.Placa = item["Placa"].ToString();
                    vehiculo.Estado = item["Estado"].ToString();
                    vehiculo.TrasladoSiNo = item["Traslado"].ToString();
                    ListadoVehiculos.Add(vehiculo);
                }


                return ListadoVehiculos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string CrearVehiculo( Models.Vehiculos vehiculo)
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

                    string Query = "INSERT INTO T_VEHICULOS(Descripcion,Placa,Piloto,VolumenCarga,PesoCarga,HoraInicioLabores,FechaCreacion,Estado,Traslado) VALUES (?,?,?,?,?,?,?,?,?)";

                    string Temp = DateTime.Today.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(vehiculo.HoraInicioLabores) ? "00:00" : vehiculo.HoraInicioLabores);

                    DateTime HoraLabores = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                    commandInsertar.Parameters.AddWithValue("@Descripcion ", SqlDbType.VarChar).Value = vehiculo.Descripcion;
                    commandInsertar.Parameters.AddWithValue("@Placa", SqlDbType.VarChar).Value = vehiculo.Placa;
                    commandInsertar.Parameters.AddWithValue("@Piloto", SqlDbType.VarChar).Value = vehiculo.Piloto;
                    commandInsertar.Parameters.AddWithValue("@VolumenCarga", SqlDbType.VarChar).Value = vehiculo.VolumenCarga;
                    commandInsertar.Parameters.AddWithValue("@PesoCarga", SqlDbType.VarChar).Value = vehiculo.PesoCarga;
                    commandInsertar.Parameters.AddWithValue("@HoraInicioLabores", SqlDbType.Date).Value = HoraLabores;
                    commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Today;
                    //commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.Int).Value = Convert.ToInt32(UsrCreacion);
                    commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";
                    commandInsertar.Parameters.AddWithValue("@Traslado", SqlDbType.VarChar).Value = vehiculo.TrasladoSiNo;
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

        public Models.Vehiculos ObtenerVehiculo(int id)
        {
            dB = new conexionDB();

            Models.Vehiculos vehiculo = new Models.Vehiculos();
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_VEHICULOS WHERE Id = '"+id+"';", "T_VEHICULOS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    
                    vehiculo.Codigo = Convert.ToInt32(item["Id"].ToString());
                    vehiculo.Descripcion = item["Descripcion"].ToString();
                    vehiculo.Placa = item["Placa"].ToString();
                    vehiculo.PesoCarga = item["PesoCarga"].ToString();
                    vehiculo.VolumenCarga = item["VolumenCarga"].ToString();
                    vehiculo.Piloto = item["Piloto"].ToString();
                    vehiculo.Placa = item["Placa"].ToString();
                    vehiculo.Estado = item["Estado"].ToString();
                    vehiculo.HoraInicioLabores = Convert.ToDateTime(item["HoraInicioLabores"].ToString()).ToString("HH:mm");
                    vehiculo.TrasladoSiNo = item["Traslado"].ToString();
                }

                return vehiculo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string ActualizarVehiculo(int id, Models.Vehiculos vehiculos) 
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

                    string Temp = DateTime.Today.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(vehiculos.HoraInicioLabores) ? "00:00" : vehiculos.HoraInicioLabores);

                    DateTime HoraLabores = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                    string Query = string.Format("UPDATE T_VEHICULOS SET Descripcion = ? ,Placa= ? ,Piloto = ?,VolumenCarga = ?,PesoCarga = ?, HoraInicioLabores = ?, Traslado = ? WHERE Id = '{0}'",id);

                    commandInsertar.Parameters.AddWithValue("@Descripcion ", SqlDbType.VarChar).Value = vehiculos.Descripcion;
                    commandInsertar.Parameters.AddWithValue("@Placa", SqlDbType.VarChar).Value = vehiculos.Placa;
                    commandInsertar.Parameters.AddWithValue("@Piloto", SqlDbType.VarChar).Value = vehiculos.Piloto;
                    commandInsertar.Parameters.AddWithValue("@VolumenCarga", SqlDbType.VarChar).Value = vehiculos.VolumenCarga;
                    commandInsertar.Parameters.AddWithValue("@PesoCarga", SqlDbType.VarChar).Value = vehiculos.PesoCarga;
                    commandInsertar.Parameters.AddWithValue("@HoraInicioLabores", SqlDbType.Date).Value = HoraLabores;
                    commandInsertar.Parameters.AddWithValue("@Traslado", SqlDbType.NVarChar).Value = vehiculos.TrasladoSiNo;
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Usuario actualizado exitosamente";
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


                    var resultado = dB.ConsultarDB("SELECT Estado FROM T_VEHICULOS WHERE Id = '"+id+"';", "T_VEHICULOS");

                    string estado = "A";
                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {
                       estado = item["Estado"].ToString();
                    }
                    string Query = string.Empty;
                    switch (estado)
                    {
                        case "A":
                            Query = string.Format("UPDATE T_VEHICULOS SET Estado = 'I' WHERE Id = '{0}'", id);
                            break;
                        default:
                            Query = string.Format("UPDATE T_VEHICULOS SET Estado = 'A' WHERE Id = '{0}'", id);
                            break;
                    }
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Vehiculo actualizado exitosamente";
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
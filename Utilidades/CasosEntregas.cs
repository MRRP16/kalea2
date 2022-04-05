using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class CasosEntregas : conexionDB
    {
        conexionDB dB;
        public List<Models.Reserva> Entregas()
        {
            dB = new conexionDB();

            List<Models.Reserva> ListadoReservas = null;
            try
            {
                var resultado = dB.ConsultarDB(String.Format("SELECT T0.*,T1.DESCRIPCION AS \"NombreVehiculo\" FROM T_ENC_ENTREGAS T0 LEFT JOIN T_VEHICULOS T1 ON T0.VEHICULO = T1.ID WHERE T0.Estado <> 'NA' ORDER BY T0.Id Desc"), "T_ENC_ENTREGAS");
                ListadoReservas = new List<Models.Reserva>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva reserva = new Models.Reserva();
                    reserva.Id = Convert.ToInt32(item["Id"].ToString());
                    reserva.FechaEntrega = Convert.ToDateTime(item["FechaInicio"].ToString());
                    reserva.FechaInicio = item["FechaInicio"].ToString();
                    reserva.FechaFin = item["FechaFin"].ToString();
                    reserva.TiempoArmado = item["TiempoArmado"].ToString();
                    reserva.FechaRestriccionInicio = item["FechaRestriccionInicio"].ToString();
                    reserva.FechaRestriccionFin = item["FECHARECTRICCIONFIN"].ToString();
                    reserva.DireccionEntrega = item["DireccionEntrega"].ToString();
                    reserva.Departamento = item["Departamento"].ToString();
                    reserva.Municipio = item["Municipio"].ToString();
                    reserva.Zona = item["Zona"].ToString();
                    reserva.Coordenadas = item["Coordenadas"].ToString();
                    reserva.NombreCliente = item["NombreCliente"].ToString();
                    reserva.NitCliente = item["NitCliente"].ToString();
                    reserva.Telefono = item["Telefono"].ToString();
                    reserva.Celular = item["Celular"].ToString();
                    reserva.PersonaRecepcion = item["PersonaRecepcion"].ToString();
                    reserva.ComentariosVenta = item["ComentariosVentas"].ToString();
                    reserva.ComentariosTorre = item["ComentariosTorre"].ToString();
                    reserva.Estado = item["Estado"].ToString();
                    reserva.UsrCreacion = item["UsrCreacion"].ToString();
                    reserva.FechaCreacion = item["FechaCreacion"].ToString();
                    reserva.NumeroEntregaDia = item["NumeroEntregaDia"].ToString();
                    reserva.Vehiculo = item["NombreVehiculo"].ToString();
                    reserva.FechaEntrega2 = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("dd-MM-yyyy");
                    reserva.ReferenciaReserva = item["Referencia_Reserva"].ToString();
                    reserva.ColorTipoEvento = item["TipoEvento"].ToString();
                    ListadoReservas.Add(reserva);
                }

                return ListadoReservas;
            }
            catch (Exception ex)
            {
               return ListadoReservas = new List<Models.Reserva>();
            }
        }

        public Models.Reserva GetEntrega(string Id)
        {
            try
            {
                Models.Reserva reserva = new Models.Reserva();
                dB = new conexionDB();
                Utilidades.Reservas reservas = new Reservas();
                var resultado = dB.ConsultarDB(string.Format( "SELECT T0.*,T1.DESCRIPCION AS \"NombreVehiculo\" FROM T_ENC_ENTREGAS T0 LEFT JOIN T_VEHICULOS T1 ON T0.VEHICULO = T1.ID WHERE T0.Estado <> 'NA' AND T0.Id = '{0}';",Id), "T_ENC_ENTREGAS");
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    reserva.Id = Convert.ToInt32(item["Id"].ToString());
                    reserva.FechaEntrega = Convert.ToDateTime(item["FechaInicio"].ToString());
                    reserva.FechaInicio = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("HH:mm");
                    reserva.FechaFin = Convert.ToDateTime(item["FechaFin"].ToString()).ToString("HH:mm");
                    reserva.TiempoArmado = item["TiempoArmado"].ToString();
                    reserva.FechaRestriccionInicio = Convert.ToDateTime(item["FechaRestriccionInicio"].ToString()).ToString("HH:mm");
                    reserva.FechaRestriccionFin = Convert.ToDateTime(item["FECHARECTRICCIONFIN"].ToString()).ToString("HH:mm");
                    reserva.DireccionEntrega = item["DireccionEntrega"].ToString();
                    reserva.Departamento = item["Departamento"].ToString();
                    reserva.Municipio = item["Municipio"].ToString();
                    reserva.Zona = item["Zona"].ToString();
                    reserva.Coordenadas = item["Coordenadas"].ToString();
                    reserva.NombreCliente = item["NombreCliente"].ToString();
                    reserva.NitCliente = item["NitCliente"].ToString();
                    reserva.Telefono = item["Telefono"].ToString();
                    reserva.Celular = item["Celular"].ToString();
                    reserva.PersonaRecepcion = item["PersonaRecepcion"].ToString();
                    reserva.ComentariosVenta = item["ComentariosVentas"].ToString();
                    reserva.ComentariosTorre = item["ComentariosTorre"].ToString();
                    reserva.Estado = item["Estado"].ToString();
                    reserva.UsrCreacion = item["UsrCreacion"].ToString();
                    reserva.FechaCreacion = item["FechaCreacion"].ToString();
                    reserva.NumeroEntregaDia = item["NumeroEntregaDia"].ToString();
                    reserva.Vehiculo = item["NombreVehiculo"].ToString();
                    reserva.NumVehiculo = item["Vehiculo"].ToString();
                    reserva.Geolocalizacion = item["Geolocalizacion"].ToString();
                    reserva.DireccionFiscal = item["DireccionFiscal"].ToString();
                    reserva.FechaEntrega2 = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("yyyy-MM-dd");
                    reserva.TipoDeInstalacion = item["TIPOINSTALACION"].ToString();
                }
                reserva.Casos_Pendientes = reservas.ObtenerCasos();
                reserva.Eventos_Articulos = reservas.ObtenerEventos();
                reserva.Reserva_Articulos = reservas.ArticulosEntrega(Id);
                reserva.Reserva_Casos = reservas.CasossEntrega(Id);
                reserva.TiposDeInstalacion = reservas.TiposDeInstalaciones();
                return reserva;
            }
            catch (Exception)
            {
                return null;
            }
        
        }

        public string RevisarEspaciosReserva()
        {
            try
            {



                return "Exitoso";
            }
            catch (Exception e)
            {

                return "Error:"+e.Message;
            }
        
        }

        public string AnularEntrega(string ID)
        {
            try
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

                        string Query = string.Format("UPDATE T_ENC_ENTREGAS SET ESTADO = 'AN' where ID = '{0}'", ID);


                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();
                        transaction.Commit();
                        return "Entrega anulada exitosamente!!";
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
                return "Error: " + e.Message;
            }
        }

        public string RordenarTrasAnulacion(string id)
        {
            try
            {
                Utilidades.Reservas re = new Utilidades.Reservas();
                dB = new conexionDB();
                var temp = dB.ConsultarDB(string.Format("SELECT ID,FECHACREACION,FECHAINICIO,VEHICULO FROM T_ENC_ENTREGAS WHERE ID = '{0}';",id), "NUM_ENTREGA");
                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    DateTime dtf = Convert.ToDateTime(item["FECHAINICIO"].ToString());

                    dtf = Convert.ToDateTime(dtf.ToString("yyyy-MM-dd"));

                    re.ReOrdenarEntregas(dtf, item["VEHICULO"].ToString(), new Models.Reserva());
                }
                return "Exitoso";
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
           

        }
    }
}
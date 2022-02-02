using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Utilidades
{
    public class Reservas : conexionDB
    {
        conexionDB dB;

        public Models.Respuesta ListadoReservas(DateTime Fecha)
        {
            dB = new conexionDB();

            List<Models.Reserva> ListadoReservas = null;
            try
            {
                Models.Respuesta respuesta = new Models.Respuesta();

                Vehiculos vehiculos = new Vehiculos();
                var listado = vehiculos.ListadoVehiculosNobloqueados(Fecha).OrderBy(x => x.TrasladoSiNo).ToList();
                respuesta.Vehiculos = listado;

                var resultado = dB.ConsultarDB(@"SELECT ID,Valor FROM T_PARAMETROS_GENERALES WHERE Id = '2' OR ID = '3';", "T_ENC_ENTREGAS");
                int HolguraInicio = 0;
                int HolguraFinal = 0;
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    switch (item["Id"].ToString())
                    {
                        case "2":
                            HolguraInicio = int.Parse(item["Valor"].ToString());
                            break;
                        case "3":
                            HolguraFinal = int.Parse(item["Valor"].ToString());
                            break;
                    }
                }


                string Query = string.Format(@"SELECT T0.*,T1.CODIGOEVENTO,T2.NUMCASO FROM T_ENC_ENTREGAS T0 
                                                LEFT JOIN T_DET_ENTREGAS T1 ON T0.ID = T1.IDENTREGA
                                                LEFT JOIN T_DET_CASOS_ENTREGAS T2 ON T0.ID = T2.IDENTREGA
                                                WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') 
                                                and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')
                                                AND T0.Estado = 'A'
                                                ORDER BY T0.ID ASC;", Fecha.ToString("yyyy-MM-dd"));

                resultado = dB.ConsultarDB(Query, "T_ENC_ENTREGAS");
                ListadoReservas = new List<Models.Reserva>();
                double tArmado = 0;

                string inicial = string.Empty;
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    if (inicial != item["Id"].ToString())
                    {
                        Models.Reserva reserva = new Models.Reserva();

                        DateTime dt = Convert.ToDateTime(item["FECHACREACION"].ToString());
                        reserva.TiempoRestante = (DateTime.Now - dt).Minutes + (DateTime.Now - dt).Hours * 60;
                        reserva.Id = Convert.ToInt32(item["Id"].ToString());
                        reserva.FechaInicio = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("HH:mm");
                        reserva.FechaFin = Convert.ToDateTime(item["FechaFin"].ToString()).ToString("HH:mm");
                        reserva.FechaArmado = Convert.ToDateTime(item["FechaArmado"].ToString()).ToString("HH:mm");
                        tArmado = string.IsNullOrEmpty(item["TiempoArmado"].ToString()) ? 0 : Convert.ToDouble(item["TiempoArmado"].ToString());
                        reserva.TiempoArmado = (tArmado + HolguraFinal).ToString();
                        reserva.FechaRestriccionInicio = Convert.ToDateTime(item["FechaRestriccionInicio"]).ToString("HH:mm");
                        reserva.FechaRestriccionFin = Convert.ToDateTime(item["FECHARECTRICCIONFIN"]).ToString("HH:mm");
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
                        reserva.Vehiculo = item["Vehiculo"].ToString();
                        reserva.ColorTipoEvento = item["TipoEvento"].ToString();
                        reserva.ReferenciaReserva = item["Referencia_Reserva"].ToString();
                        reserva.Geolocalizacion = item["GeoloCalizacion"].ToString();
                        reserva.DireccionFiscal = item["DireccionFiscal"].ToString();
                        int NumMinutos = (Convert.ToDateTime(item["FechaInicio"].ToString()) - Convert.ToDateTime(item["FechaFin"].ToString())).Minutes + (Convert.ToDateTime(item["FechaInicio"].ToString()) - Convert.ToDateTime(item["FechaFin"].ToString())).Hours * 60;
                        int SumEspacios = (NumMinutos / 60) * 40 * -1;
                        if (SumEspacios == 0)
                        {
                            SumEspacios = 40;
                        }

                        reserva.TamanioTarjeta = (NumMinutos * 4 * -1) + SumEspacios;
                        reserva.TiempoRuta = string.IsNullOrEmpty(item["TiempoRuta"].ToString()) ? 0 : (Convert.ToInt32(item["TiempoRuta"].ToString()) + HolguraInicio);
                        reserva.TamanioTarjeta = (NumMinutos * 4*-1)+ SumEspacios;
                        reserva.TiempoRuta = string.IsNullOrEmpty(item["TiempoRuta"].ToString()) ? 0 : (Convert.ToInt32(item["TiempoRuta"].ToString() ) + HolguraInicio);
                        DataRow[] drow = resultado.Tables[0].Select("ID = '" + reserva.Id + "'");
                        foreach (DataRow item2 in drow)
                        {
                            if (!string.IsNullOrEmpty(item2[30].ToString()))
                            {
                                if (!string.IsNullOrEmpty(reserva.ListadoEventosCasos))
                                {
                                    if (!reserva.ListadoEventosCasos.Contains(item2[30].ToString()))
                                    {
                                        reserva.ListadoEventosCasos += item2[30].ToString() + ";";
                                    }
                                }
                                else
                                {
                                    reserva.ListadoEventosCasos += item2[30].ToString() + ";";
                                } 
                            }
                            if (!string.IsNullOrEmpty(item2[31].ToString()))
                            {
                                if (!string.IsNullOrEmpty(reserva.ListadoEventosCasos))
                                {
                                    if (!reserva.ListadoEventosCasos.Contains(item2[31].ToString()))
                                    {
                                        reserva.ListadoEventosCasos += item2[31].ToString() + ";";
                                    }
                                }
                                else
                                {
                                    reserva.ListadoEventosCasos += item2[31].ToString() + ";";
                                }

                            }

                        }
                        ListadoReservas.Add(reserva);
                        inicial = item["Id"].ToString();
                    }

                }

                respuesta.Reservaciones = ListadoReservas;
                return respuesta;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Models.Reserva ObtenerReservas(int id)
        {
            dB = new conexionDB();
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_ENC_ENTREGAS WHERE Id = '" + id + "';", "T_PARAMETROS_GENERALES");
                Models.Reserva reserva = new Models.Reserva();


                return reserva;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<SelectListItem> ObtenerCasos()
        {
            dB = new conexionDB();
            List<SelectListItem> Casos;
            try
            {
                //string query = @"SELECT T0.Caso FROM Naf47.rec_detalle_caso T0 inner join Naf47.rec_caso T1 ON T0.Caso = T1.Caso
                //                 WHERE T0.Caso NOT IN (SELECT NumCaso FROM T_DET_CASOS_ENTREGAS) AND T1.Estado <> 'F' GROUP BY T0.Caso";

                string query = @"SELECT T0.Caso FROM Naf47.rec_detalle_caso T0 inner join Naf47.rec_caso T1 ON T0.Caso = T1.Caso
                                 WHERE T1.Estado <> 'F' GROUP BY T0.Caso";
                var resultado = dB.ConsultarDB(query, "T_EVENTOS");


                Casos = new List<SelectListItem>();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Casos.Add(new SelectListItem { Text = item["Caso"].ToString(), Value = item["Caso"].ToString() });
                }

                return Casos;
            }
            catch (Exception)
            {
                return Casos = new List<SelectListItem>();
            }

        }
        public List<SelectListItem> ObtenerEventos()
        {
            dB = new conexionDB();
            List<SelectListItem> Eventos;
            try
            {
                //string query = @"SELECT * FROM Naf47.V_EVENTOS_PENDIENTES T5 WHERE EVENTO NOT IN (SELECT CodigoEvento FROM T_DET_ENTREGAS) AND FECHA >= TO_DATE('01-OCT-2021')
                //                AND (SELECT count(*)
                //                FROM Naf47.Pvlineas_movimiento T0
                //                LEFT JOIN Naf47.Arinda T1 ON T0.NO_ARTI = T1.NO_ARTI
                //                LEFT JOIN Naf47.Pvencabezado_movimientos T2 ON T2.NO_TRANSA_MOV = T0.NO_TRANSA_MOV
                //                LEFT JOIN Naf47.pvclientes T3 ON T3.COD_CLIENTE = T2.COD_CLIENTE
                //                WHERE T0.NO_TRANSA_MOV = T5.EVENTO AND T0.ENTREGADOMICILIO = 'D') >=1;";

                string query = @"SELECT * FROM Naf47.V_EVENTOS_PENDIENTES T5 WHERE FECHA >= TO_DATE('01-NOV-2021')
                                AND (SELECT count(*)
                                FROM Naf47.Pvlineas_movimiento T0
                                LEFT JOIN Naf47.Arinda T1 ON T0.NO_ARTI = T1.NO_ARTI
                                LEFT JOIN Naf47.Pvencabezado_movimientos T2 ON T2.NO_TRANSA_MOV = T0.NO_TRANSA_MOV
                                LEFT JOIN Naf47.pvclientes T3 ON T3.COD_CLIENTE = T2.COD_CLIENTE
                                WHERE T0.NO_TRANSA_MOV = T5.EVENTO AND T0.ENTREGADOMICILIO = 'D') >=1;";

                var resultado = dB.ConsultarDB(query, "T_EVENTOS");


                Eventos = new List<SelectListItem>();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Eventos.Add(new SelectListItem { Text = item["EVENTO"].ToString(), Value = item["EVENTO"].ToString() });                    
                }

                return Eventos;
            }
            catch (Exception e)
            {
                return Eventos = new List<SelectListItem>();
            }

        }
        public List<Models.Reserva_Detalle_Articulos> Articulos(string id)
        {
            dB = new conexionDB();
            List<Models.Reserva_Detalle_Articulos> articulos;
            try
            {
                articulos = new List<Models.Reserva_Detalle_Articulos>();


                string query = string.Format(@"SELECT T0.NO_ARTI,T1.Descripcion,T1.Tiempo_armado,T0.Cantidad,T0.CantidadDomicilio,T0.EntregaDomicilio,T2.Nombre_Cliente,T2.COD_CLIENTE,T2.Observaciones, T2.DIRECCION_ENTREGA,T2.DIRECCION_FISCAL,T3.Telefono,T1.CANTIDAD_ARMADORES
                                FROM Naf47.Pvlineas_movimiento T0
                                LEFT JOIN Naf47.Arinda T1 ON T0.NO_ARTI = T1.NO_ARTI
                                LEFT JOIN Naf47.Pvencabezado_movimientos T2 ON T2.NO_TRANSA_MOV = T0.NO_TRANSA_MOV
                                LEFT JOIN Naf47.pvclientes T3 ON T3.COD_CLIENTE = T2.COD_CLIENTE
                                WHERE T0.NO_TRANSA_MOV = '{0}' AND T0.ENTREGADOMICILIO = 'D'
                                GROUP BY T0.NO_ARTI,T1.Descripcion,T1.Tiempo_armado,T0.Cantidad,T0.CantidadDomicilio,T0.EntregaDomicilio,T2.Nombre_Cliente,T2.COD_CLIENTE,T2.Observaciones, T2.DIRECCION_ENTREGA,T2.DIRECCION_FISCAL,T3.Telefono,T1.CANTIDAD_ARMADORES;", id);

                var resultado = dB.ConsultarDB(query, "T_EVENTOS");

                articulos = new List<Models.Reserva_Detalle_Articulos>();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva_Detalle_Articulos articulo = new Models.Reserva_Detalle_Articulos();
                    //articulo.Id = Convert.ToInt32(item["Id"].ToString());
                    articulo.CodigoArticulo = item["NO_ARTI"].ToString();
                    articulo.Descripcion = item["Descripcion"].ToString();
                    articulo.Cantidad = Convert.ToInt32(item["Cantidad"].ToString());
                    articulo.EstadoArticulo = item["EntregaDomicilio"].ToString();
                    articulo.NumEvento = id;
                    articulo.Observaciones = item["Observaciones"].ToString();
                    articulo.DireccionEntrega = item["DIRECCION_ENTREGA"].ToString();
                    articulo.DireccionFiscal = item["DIRECCION_FISCAL"].ToString();
                    articulo.CodigoCliente = item["COD_CLIENTE"].ToString();
                    articulo.NombreCliente = item["Nombre_Cliente"].ToString();
                    articulo.Telefono = item["Telefono"].ToString();

                    double totalT = 0;
                    try
                    {
                        if (!string.IsNullOrEmpty(item["Tiempo_armado"].ToString()))
                        {
                            totalT = (Convert.ToDouble(item["Tiempo_armado"].ToString()) * articulo.Cantidad * Convert.ToInt32(item["CANTIDAD_ARMADORES"].ToString())) / 2;
                            articulo.TiempoArmado = totalT.ToString();
                        }
                        else
                        {
                            articulo.TiempoArmado = "0";
                        }
                    }
                    catch (Exception)
                    {

                        articulo.TiempoArmado = totalT.ToString() ;
                    }

                    articulos.Add(articulo);
                }

                return articulos;
            }
            catch (Exception ex)
            {
                articulos = new List<Models.Reserva_Detalle_Articulos>();
                return articulos;
            }
        }

        public string Comentarios(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                dB = new conexionDB();
                List<Models.Reserva_Detalle_Articulos> articulos;
                try
                {
                    id = Regex.Replace(id, @"\r\n?|\n|\t", String.Empty);
                    id = id.Replace(" ", "");
                    
                    id = id.TrimEnd(new char[] { ',','\n' });

                    articulos = new List<Models.Reserva_Detalle_Articulos>();


                    string query = string.Format(@"select NO_TRANSA_MOV,OBSERVACIONES from  Naf47.Pvencabezado_movimientos
                                                where NO_TRANSA_MOV IN ({0});", id);

                    var resultado = dB.ConsultarDB(query, "T_EVENTOS");
                    string Observaciones = string.Empty;
                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {

                        Observaciones += "Evento " + item["NO_TRANSA_MOV"].ToString() + ": " + item["Observaciones"].ToString() + System.Environment.NewLine + System.Environment.NewLine;

                    }

                    return Observaciones;
                }
                catch (Exception ex)
                {

                    return ex.Message;
                }
            }
            else
            {
                return "";
            }
           
        }
        public List<Models.Reserva_Detalle_Casos> Caso(string id)
        {
            dB = new conexionDB();
            List<Models.Reserva_Detalle_Casos> articulos;
            try
            {
                articulos = new List<Models.Reserva_Detalle_Casos>();


                string query = string.Format(@"SELECT T0.CASO,T0.DIR_ENTREGA,T0.TELEFONO_CASA,T0.TELEFONO_CEL,NOMBRE_CLIENTE,T1.Observaciones, T1.Acciones FROM Naf47.rec_caso T0
                                                LEFT JOIN Naf47.rec_detalle_caso T1 ON T0.CASO = T1.CASO
                                                WHERE T0.CASO = '{0}' AND T1.TORRE = 'S';", id);

                var resultado = dB.ConsultarDB(query, "T_EVENTOS");

                articulos = new List<Models.Reserva_Detalle_Casos>();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva_Detalle_Casos articulo = new Models.Reserva_Detalle_Casos();
                    articulo.NumCaso = item["CASO"].ToString();
                    articulo.Observaciones = item["Observaciones"].ToString();
                    articulo.Acciones = item["Acciones"].ToString();
                    articulo.Tel = item["TELEFONO_CASA"].ToString();
                    articulo.Cel = item["TELEFONO_CEL"].ToString();
                    articulo.Cliente = item["NOMBRE_CLIENTE"].ToString();
                    articulo.Direccion = item["DIR_ENTREGA"].ToString();
                    articulo.Cliente = item["NOMBRE_CLIENTE"].ToString();
                    articulos.Add(articulo);
                }

                return articulos;
            }
            catch (Exception ex)
            {
                articulos = new List<Models.Reserva_Detalle_Casos>();
                return articulos;
            }
        }

        public string BloquearRuta(string id,string fecha)
        {
            try
            {
                dB = new conexionDB();
                DateTime dt = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string query = string.Format(@" SELECT COUNT(*) AS TOTAL FROM T_ENC_ENTREGAS T0 
                                                WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') 
                                                and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')
                                                AND T0.Estado = 'A'
                                                AND T0.VEHICULO = '{1}'
                                                ORDER BY T0.ID ASC;", dt.ToString("yyyy-MM-dd"),id);
                var resultado = dB.ConsultarDB(query, "T_EVENTOS");

                int total = 0;
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    total = Convert.ToInt32(item["TOTAL"].ToString());
                }
                if (total.Equals(0))
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

                            string Query = "INSERT INTO T_BLOQUEO_RUTAS(IDVehiculo,Fecha) VALUES (?,?)";

                            commandInsertar.Parameters.AddWithValue("@IDVehiculo", SqlDbType.Int).Value = Convert.ToInt32(id);
                            commandInsertar.Parameters.AddWithValue("@Fecha", SqlDbType.DateTime).Value = dt;
                            commandInsertar.CommandText = Query;
                            commandInsertar.ExecuteNonQuery();

                            transaction.Commit();
                            return "Ruta bloqueada exitosamente.";
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
                else
                {
                    return "Error: No se puede bloquear la ruta, debido a que el vehiculo ya contiene entregas asignadas.";
                }
                return "Exitoso";
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
        
        
        }

        public string CrearEntregaDefinitiva(Models.Reserva reserva, string UsrCreacion)
        {
            string Respuesta = string.Empty;
            try
            {
                dB = new conexionDB();

                var temp = dB.ConsultarDB("SELECT IDTT_ENC_ENTREGAS.nextval from dual;", "NUM_ENTREGA");
                string NumEntrega = string.Empty;

                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    NumEntrega = item["nextval"].ToString();
                }

                temp = dB.ConsultarDB(string.Format("SELECT MAX(NUMEROENTREGADIA) AS EntregaMax FROM T_ENC_ENTREGAS WHERE VEHICULO = '{0}';",reserva.Vehiculo), "NUM_ENTREGA");
                int  NumEntregaDia = 0;

                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    if (!string.IsNullOrEmpty(item["EntregaMax"].ToString()))
                    {
                        NumEntregaDia = Convert.ToInt32(item["EntregaMax"].ToString()) + 1;
                    }
                    else
                    {
                        NumEntregaDia = 1;
                    }
                }

                double TiempoArmado = 0;
                if (reserva.Reserva_Articulos != null)
                {
                    foreach (var item in reserva.Reserva_Articulos)
                    {
                        TiempoArmado += Convert.ToDouble(item.TiempoArmado);
                    }
                }
                DateTime FechaInicio = reserva.FechaEntrega;
                DateTime FechaArmado = reserva.FechaEntrega;
                DateTime FechaFin = reserva.FechaEntrega;

                string Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionInicio) ? "00:00" : reserva.FechaRestriccionInicio);

                DateTime FechaRectriccionInicio = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionFin) ? "00:00" : reserva.FechaRestriccionFin); ;

                DateTime FechaRectriccionFin = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

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
                        //transaction = myConnection.BeginTransaction();

                        commandInsertar.Connection = myConnection;
                        //commandInsertar.Transaction = transaction;

                        string Query = @"INSERT INTO T_ENC_ENTREGAS(Id,FechaInicio,FechaArmado,FechaFin,TiempoArmado,FechaRestriccionInicio,FechaRectriccionFin,DireccionEntrega,
                                        Departamento,Municipio,Zona,Coordenadas,NombreCliente,NitCliente,Telefono,Celular,PersonaRecepcion,
                                        ComentariosVentas,ComentariosTorre,Estado,UsrCreacion,FechaCreacion,NumeroEntregaDia,Vehiculo,TipoEvento,GeoLocalizacion,DireccionFiscal) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                        commandInsertar.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Convert.ToInt32(NumEntrega);
                        commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaArmado;
                        commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaFin;
                        commandInsertar.Parameters.AddWithValue("@TiempoArmado", SqlDbType.Int).Value = TiempoArmado;
                        commandInsertar.Parameters.AddWithValue("@FechaRestriccionInicio ", SqlDbType.Date).Value = FechaRectriccionInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaRectriccionFin ", SqlDbType.Date).Value = FechaRectriccionFin;
                        commandInsertar.Parameters.AddWithValue("@DireccionEntrega", SqlDbType.VarChar).Value = reserva.DireccionEntrega;
                        commandInsertar.Parameters.AddWithValue("@Departamento", SqlDbType.VarChar).Value = "Guatemala";
                        commandInsertar.Parameters.AddWithValue("@Municipio", SqlDbType.VarChar).Value = "Ciudad de Guatemala";
                        commandInsertar.Parameters.AddWithValue("@Zona", SqlDbType.VarChar).Value = "3";
                        commandInsertar.Parameters.AddWithValue("@Coordenadas", SqlDbType.VarChar).Value = reserva.Geolocalizacion;
                        commandInsertar.Parameters.AddWithValue("@NombreCliente", SqlDbType.VarChar).Value = reserva.NombreCliente;
                        commandInsertar.Parameters.AddWithValue("@NitCliente", SqlDbType.VarChar).Value = "95853820";
                        commandInsertar.Parameters.AddWithValue("@Telefono", SqlDbType.VarChar).Value = reserva.Telefono;
                        commandInsertar.Parameters.AddWithValue("@Celular", SqlDbType.VarChar).Value = reserva.Celular;
                        commandInsertar.Parameters.AddWithValue("@PersonaRecepcion", SqlDbType.VarChar).Value = "Cliente que recibe";
                        commandInsertar.Parameters.AddWithValue("@ComentariosVentas", SqlDbType.VarChar).Value = reserva.ComentariosVenta;
                        commandInsertar.Parameters.AddWithValue("@ComentariosTorre", SqlDbType.VarChar).Value = string.IsNullOrEmpty(reserva.ComentariosTorre) ? "" : reserva.ComentariosTorre;
                        commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";
                        commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = "1";
                        commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Now;
                        commandInsertar.Parameters.AddWithValue("@NumeroEntregaDia", SqlDbType.Int).Value = NumEntregaDia;
                        commandInsertar.Parameters.AddWithValue("@Vehiculo", SqlDbType.Int).Value = Convert.ToInt32(reserva.Vehiculo);
                        commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = "#00b050";
                        commandInsertar.Parameters.AddWithValue("@GeoLocalizacion", SqlDbType.VarChar).Value = reserva.Geolocalizacion;
                        commandInsertar.Parameters.AddWithValue("@DireccionFiscal", SqlDbType.VarChar).Value = reserva.DireccionFiscal;

                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();

                        reserva.ColorTipoEvento = "#00b050";
                        Query = "INSERT INTO T_DET_ENTREGAS(IdEntrega,CodigoEvento,CodigoArticulo,Cantidad,Remisionado,EstadoArticulo,UsrCreacion) VALUES (?,?,?,?,?,?,?)";
                        if (reserva.Reserva_Articulos != null)
                        {
                            foreach (var item in reserva.Reserva_Articulos)
                            {
                                commandInsertar.Dispose();
                                commandInsertar = myConnection.CreateCommand();
                                commandInsertar.Parameters.AddWithValue("@IdEntrega", SqlDbType.Int).Value = Convert.ToInt32(NumEntrega);
                                commandInsertar.Parameters.AddWithValue("@CodigoEvento", SqlDbType.VarChar).Value = item.NumEvento;
                                commandInsertar.Parameters.AddWithValue("@CodigoArticulo", SqlDbType.VarChar).Value = item.CodigoArticulo;
                                commandInsertar.Parameters.AddWithValue("@Cantidad", SqlDbType.Int).Value = item.Cantidad;
                                commandInsertar.Parameters.AddWithValue("@Remisionado", SqlDbType.VarChar).Value = "F";
                                commandInsertar.Parameters.AddWithValue("@EstadoArticulo", SqlDbType.VarChar).Value = "N";
                                commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = "1";
                                commandInsertar.CommandText = Query;
                                commandInsertar.ExecuteNonQuery();
                            }
                        }

                        Query = "INSERT INTO T_DET_CASOS_ENTREGAS(IdEntrega,NumCaso,Acciones,UsrCreacion) VALUES (?,?,?,?)";
                        if (reserva.Reserva_Casos != null)
                        {
                            foreach (var item in reserva.Reserva_Casos)
                            {
                                commandInsertar.Dispose();
                                commandInsertar = myConnection.CreateCommand();
                                commandInsertar.Parameters.AddWithValue("@IdEntrega ", SqlDbType.Int).Value = Convert.ToInt32(NumEntrega);
                                commandInsertar.Parameters.AddWithValue("@NumCaso", SqlDbType.VarChar).Value = item.NumCaso;
                                //commandInsertar.Parameters.AddWithValue("@Observaciones", SqlDbType.VarChar).Value = string.IsNullOrEmpty(item.Observaciones) ? "" : item.Observaciones;
                                commandInsertar.Parameters.AddWithValue("@Acciones", SqlDbType.VarChar).Value = string.IsNullOrEmpty(item.Acciones) ? "" : item.Acciones;
                                commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = UsrCreacion;

                                commandInsertar.CommandText = Query;
                                commandInsertar.ExecuteNonQuery();
                            }
                        }


                        //transaction.Commit();

                        reserva.Id = int.Parse(NumEntrega);
                        string resultado = ReOrdenarEntregas(FechaInicio, reserva.Vehiculo, reserva);

                        if (resultado.Contains("Error"))
                        {
                            return resultado;
                        }

                        return "Entrega #" + NumEntrega + " creada exitosamente.";
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // Attempt to roll back the transaction.
                            //transaction.Rollback();
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

                throw;
            }

            return Respuesta;
        }
        public string EditarEntregaDefinitiva(Models.Reserva reserva, string UsrCreacion, string id)
        {
            string Respuesta = string.Empty;
            try
            {
                dB = new conexionDB();

                var temp = dB.ConsultarDB(string.Format("DELETE FROM T_DET_ENTREGAS WHERE IDENTREGA = '{0}';", id), "NUM_ENTREGA");
                temp = dB.ConsultarDB(string.Format("DELETE FROM T_DET_CASOS_ENTREGAS WHERE IDENTREGA = '{0}';", id), "NUM_ENTREGA");

                int TiempoArmado = 0;
                if (reserva.Reserva_Articulos != null)
                {
                    foreach (var item in reserva.Reserva_Articulos)
                    {
                        TiempoArmado += TiempoArmado + Convert.ToInt32(item.TiempoArmado);
                    }
                }


                var resultado = dB.ConsultarDB(@"SELECT ID,Valor FROM T_PARAMETROS_GENERALES WHERE Id = '2' OR ID = '3';", "T_ENC_ENTREGAS");
                int HolguraInicio = 0;
                int HolguraFinal = 0;

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    switch (item["Id"].ToString())
                    {
                        case "2":
                            HolguraInicio = int.Parse(item["Valor"].ToString());
                            break;
                        case "3":
                            HolguraFinal = int.Parse(item["Valor"].ToString());
                            break;
                    }
                }

                string Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaInicio) ? "00:00" : reserva.FechaInicio);

                DateTime FechaInicio = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                DateTime FechaArmado = FechaInicio;
                FechaArmado = FechaArmado.AddMinutes(HolguraInicio);
                FechaArmado = FechaArmado.AddMinutes(Convert.ToInt32(TiempoArmado));


                DateTime FechaFin = reserva.FechaEntrega;
                FechaFin = FechaArmado;
                FechaFin = FechaFin.AddMinutes(HolguraFinal);

                Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionInicio) ? "00:00" : reserva.FechaRestriccionInicio);

                DateTime FechaRectriccionInicio = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionFin) ? "00:00" : reserva.FechaRestriccionFin); ;

                DateTime FechaRectriccionFin = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

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
                        //transaction = myConnection.BeginTransaction();

                        commandInsertar.Connection = myConnection;
                        //commandInsertar.Transaction = transaction;

                        string Query = string.Format("UPDATE T_ENC_ENTREGAS SET FechaInicio = ? ,FechaArmado = ?,FechaFin = ? ,TiempoArmado = ?,FechaRestriccionInicio = ?,FechaRectriccionFin = ?,DireccionEntrega = ?," +
                                        "Departamento = ?,Municipio = ?,Zona = ?,Coordenadas = ?,NombreCliente = ?,NitCliente = ?,Telefono = ?,Celular = ?,PersonaRecepcion = ?," +
                                        "ComentariosVentas = ?,ComentariosTorre = ?,Estado = ?,UsrCreacion = ?,NumeroEntregaDia = ?,Vehiculo = ?,GeoLocalizacion = ?,TipoEvento = ? WHERE Id = '{0}';", id);

                        //commandInsertar.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Convert.ToInt32(id);
                        commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaArmado;
                        commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaFin;
                        commandInsertar.Parameters.AddWithValue("@TiempoArmado", SqlDbType.Int).Value = TiempoArmado;
                        commandInsertar.Parameters.AddWithValue("@FechaRestriccionInicio ", SqlDbType.Date).Value = FechaRectriccionInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaRectriccionFin ", SqlDbType.Date).Value = FechaRectriccionFin;
                        commandInsertar.Parameters.AddWithValue("@DireccionEntrega", SqlDbType.VarChar).Value = reserva.DireccionEntrega;
                        commandInsertar.Parameters.AddWithValue("@Departamento", SqlDbType.VarChar).Value = "Guatemala";
                        commandInsertar.Parameters.AddWithValue("@Municipio", SqlDbType.VarChar).Value = "Ciudad de Guatemala";
                        commandInsertar.Parameters.AddWithValue("@Zona", SqlDbType.VarChar).Value = "3";
                        commandInsertar.Parameters.AddWithValue("@Coordenadas", SqlDbType.VarChar).Value = "14.640428851152517, -90.52079554577591";
                        commandInsertar.Parameters.AddWithValue("@NombreCliente", SqlDbType.VarChar).Value = reserva.NombreCliente;
                        commandInsertar.Parameters.AddWithValue("@NitCliente", SqlDbType.VarChar).Value = "95853820";
                        commandInsertar.Parameters.AddWithValue("@Telefono", SqlDbType.VarChar).Value = reserva.Telefono;
                        commandInsertar.Parameters.AddWithValue("@Celular", SqlDbType.VarChar).Value = reserva.Celular;
                        commandInsertar.Parameters.AddWithValue("@PersonaRecepcion", SqlDbType.VarChar).Value = "Cliente que recibe";
                        commandInsertar.Parameters.AddWithValue("@ComentariosVentas", SqlDbType.VarChar).Value = reserva.ComentariosVenta;
                        commandInsertar.Parameters.AddWithValue("@ComentariosTorre", SqlDbType.VarChar).Value = string.IsNullOrEmpty(reserva.ComentariosTorre) ? "" : reserva.ComentariosTorre;
                        commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";
                        commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = "1";
                        commandInsertar.Parameters.AddWithValue("@NumeroEntregaDia", SqlDbType.Int).Value = 0;
                        commandInsertar.Parameters.AddWithValue("@Vehiculo", SqlDbType.Int).Value = Convert.ToInt32(reserva.NumVehiculo);
                        commandInsertar.Parameters.AddWithValue("@GeoLocalizacion", SqlDbType.VarChar).Value = reserva.Geolocalizacion;
                        commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = "#00b050";
                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();

                        reserva.ColorTipoEvento = "#00b050";

                        Query = "INSERT INTO T_DET_ENTREGAS(IdEntrega,CodigoEvento,CodigoArticulo,Cantidad,Remisionado,EstadoArticulo,UsrCreacion) VALUES (?,?,?,?,?,?,?)";
                        if (reserva.Reserva_Articulos != null)
                        {
                            foreach (var item in reserva.Reserva_Articulos)
                            {
                                commandInsertar.Dispose();
                                commandInsertar = myConnection.CreateCommand();
                                commandInsertar.Parameters.AddWithValue("@IdEntrega", SqlDbType.Int).Value = Convert.ToInt32(id);
                                commandInsertar.Parameters.AddWithValue("@CodigoEvento", SqlDbType.VarChar).Value = item.NumEvento;
                                commandInsertar.Parameters.AddWithValue("@CodigoArticulo", SqlDbType.VarChar).Value = item.CodigoArticulo;
                                commandInsertar.Parameters.AddWithValue("@Cantidad", SqlDbType.Int).Value = item.Cantidad;
                                commandInsertar.Parameters.AddWithValue("@Remisionado", SqlDbType.VarChar).Value = "F";
                                commandInsertar.Parameters.AddWithValue("@EstadoArticulo", SqlDbType.VarChar).Value = "N";
                                commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = "1";
                                commandInsertar.CommandText = Query;
                                commandInsertar.ExecuteNonQuery();
                            }
                        }

                        Query = "INSERT INTO T_DET_CASOS_ENTREGAS(IdEntrega,NumCaso,Observaciones,Acciones,UsrCreacion) VALUES (?,?,?,?,?)";
                        if (reserva.Reserva_Casos != null)
                        {
                            foreach (var item in reserva.Reserva_Casos)
                            {
                                commandInsertar.Dispose();
                                commandInsertar = myConnection.CreateCommand();
                                commandInsertar.Parameters.AddWithValue("@IdEntrega ", SqlDbType.Int).Value = Convert.ToInt32(id);
                                commandInsertar.Parameters.AddWithValue("@NumCaso", SqlDbType.VarChar).Value = item.NumCaso;
                                commandInsertar.Parameters.AddWithValue("@Observaciones", SqlDbType.VarChar).Value = string.IsNullOrEmpty(item.Observaciones) ? "" : item.Observaciones;
                                commandInsertar.Parameters.AddWithValue("@Acciones", SqlDbType.VarChar).Value = string.IsNullOrEmpty(item.Acciones) ? "" : item.Acciones;
                                commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = UsrCreacion;

                                commandInsertar.CommandText = Query;
                                commandInsertar.ExecuteNonQuery();
                            }
                        }

                        Reservas reser = new Reservas();
                        reserva.Vehiculo = reserva.NumVehiculo;
                        string rresultado = reser.ReOrdenarEntregas(FechaInicio, reserva.NumVehiculo, reserva);
                        //transaction.Commit();

                        

                        return string.Format("Entrega #{0} editada exitosamente",id);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // Attempt to roll back the transaction.
                            //transaction.Rollback();
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

                throw;
            }

            return Respuesta;
        }
        public string ReOrdenarEntregas(DateTime FechaEntrega, string Vehiculo, Models.Reserva reservaP)
        {
            try
            {
                dB = new conexionDB();
                var resultado = dB.ConsultarDB(String.Format("SELECT * FROM T_ENC_ENTREGAS T0 WHERE Vehiculo = '{1}' AND FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')  AND Estado <> 'AN';", FechaEntrega.ToString("yyyy-MM-dd"), Vehiculo), "T_ENC_ENTREGAS");
                List<Models.Reserva> ListadoReservas = null;
                ListadoReservas = new List<Models.Reserva>();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva reserva = new Models.Reserva();
                    reserva.Id = Convert.ToInt32(item["Id"].ToString());
                    reserva.FechaInicio = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("HH:mm");
                    reserva.FechaFin = Convert.ToDateTime(item["FechaFin"].ToString()).ToString("HH:mm");
                    reserva.FechaArmadoT = Convert.ToDateTime(item["FechaArmado"].ToString());
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
                    reserva.Vehiculo = item["Vehiculo"].ToString();
                    reserva.Geolocalizacion = item["GeoLocalizacion"].ToString();
                    reserva.ColorTipoEvento = item["TipoEvento"].ToString();
                    reserva.TiempoRuta = string.IsNullOrEmpty(item["TiempoRuta"].ToString()) ? 0 : Convert.ToInt32(item["TiempoRuta"].ToString());
                    ListadoReservas.Add(reserva);
                }

                var ListaTemporal = ListadoReservas.Where(x => x.ColorTipoEvento.Equals("#00b050") || x.ColorTipoEvento.Equals("#FF0000")).OrderBy(x => x.NumeroEntregaDia).ToList();

                string query = string.Empty;
                DateTime horainicioLabores = DateTime.Today;
                DateTime FechaInicio = FechaEntrega.AddHours(FechaEntrega.Hour*-1);
                FechaInicio = FechaInicio.AddMinutes(FechaInicio.Minute * -1);
                FechaInicio = FechaInicio.AddSeconds(FechaInicio.Second * -1);
                DateTime FechaArmado = FechaEntrega;
                DateTime FechaFin = FechaEntrega;

                resultado = dB.ConsultarDB(@"SELECT ID,Valor FROM T_PARAMETROS_GENERALES WHERE Id = '2' OR ID = '3';", "T_ENC_ENTREGAS");
                int HolguraInicio = 0;
                int HolguraFinal = 0;
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    switch (item["Id"].ToString())
                    {
                        case "2":
                            HolguraInicio = int.Parse(item["Valor"].ToString());
                            break;
                        case "3":
                            HolguraFinal = int.Parse(item["Valor"].ToString());
                            break;
                    }
                }
                resultado = dB.ConsultarDB(string.Format(@"SELECT HORAINICIOLABORES FROM T_VEHICULOS WHERE ID = '{0}';", ListadoReservas[0].Vehiculo), "T_ENC_ENTREGAS");
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    horainicioLabores = Convert.ToDateTime(item["HORAINICIOLABORES"].ToString());
                }
                int NUMEROENTREGADIA = 0;
                string geoprincipal = "14.644836805197727, -90.47603107394566";
                FechaInicio = FechaInicio.AddHours(horainicioLabores.Hour);
                FechaInicio = FechaInicio.AddMinutes(horainicioLabores.Minute);
                for (int i = 0; i < ListaTemporal.Count(); i++)
                {
                    int temp = 0;
                    temp = Gettiempo(geoprincipal, ListaTemporal[i].Geolocalizacion) / 60;
                    FechaArmado = FechaInicio;
                    FechaArmado = FechaArmado.AddMinutes(HolguraInicio);
                    FechaArmado = FechaArmado.AddMinutes(temp);
                    

                    FechaFin = FechaArmado;
                    FechaFin = FechaFin.AddMinutes(Convert.ToDouble(ListaTemporal[i].TiempoArmado));
                    FechaFin = FechaFin.AddMinutes(HolguraFinal);

                    string Color = "#00b050";
                    if (!ListaTemporal[i].ColorTipoEvento.Equals("#376092"))
                    {
                        if (!ListaTemporal[i].FechaRestriccionInicio.Contains("12:00:00 AM"))
                        {
                            DateTime FechaRestriccionfin = Convert.ToDateTime(ListaTemporal[i].FechaRestriccionFin);

                            switch (FechaArmado > FechaRestriccionfin)
                            {
                                case true:
                                    Color = "#FF0000";
                                    break;
                                case false:
                                    Color = "#00b050";
                                    break;
                            }
                        }

                        //if (!ListaTemporal[i].FechaRestriccionInicio.Contains("12:00:00 a"))
                        //{
                        //    DateTime FechaRestriccionfin = Convert.ToDateTime(ListaTemporal[i].FechaRestriccionFin);

                        //    switch (FechaArmado > FechaRestriccionfin)
                        //    {
                        //        case true:
                        //            Color = "#FF0000";
                        //            break;
                        //        case false:
                        //            Color = "#00b050";
                        //            break;
                        //    }
                        //}

                        //if (!ListaTemporal[i].FechaRestriccionInicio.Contains("12:00:00 AM"))
                        //{
                        //    DateTime FechaRestriccionfin = Convert.ToDateTime(ListaTemporal[i].FechaRestriccionFin);

                        //    switch (FechaArmado > FechaRestriccionfin)
                        //    {
                        //        case true:
                        //            Color = "#FF0000";
                        //            break;
                        //        case false:
                        //            Color = "#00b050";
                        //            break;
                        //    }
                        //}
                    }
                    else
                    {
                        Color = ListaTemporal[i].ColorTipoEvento;
                    }

                    NUMEROENTREGADIA = i + 1;
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
                            //transaction = myConnection.BeginTransaction();

                            commandInsertar.Connection = myConnection;
                            //commandInsertar.Transaction = transaction;

                            string Query = string.Format(@"UPDATE T_ENC_ENTREGAS SET NUMEROENTREGADIA = ? ,FechaInicio = ? ,FechaArmado = ? ,FechaFin = ?, TiempoRuta = ?, TipoEvento = ?  WHERE ID = {0}", ListaTemporal[i].Id);

                            commandInsertar.Parameters.AddWithValue("@NUMEROENTREGADIA", SqlDbType.Int).Value = NUMEROENTREGADIA;
                            commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                            commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaArmado;
                            commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaFin;
                            commandInsertar.Parameters.AddWithValue("@TiempoRuta", SqlDbType.Int).Value = temp;
                            commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = Color;
                            commandInsertar.CommandText = Query;
                            commandInsertar.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                // Attempt to roll back the transaction.
                                //transaction.Rollback();
                                return "Error:" + e.Message;
                            }
                            catch
                            {
                                return "Error:" + e.Message;
                                // Do nothing here; transaction is not active.
                            }
                        }
                    }

                    geoprincipal = ListaTemporal[i].Geolocalizacion;
                    FechaInicio = FechaFin;
                }

                ListaTemporal = ListadoReservas.Where(x => x.ColorTipoEvento.Equals("#376092")).OrderBy(x => x.NumeroEntregaDia).ToList();
                
                for (int i = 0; i < ListaTemporal.Count(); i++)
                {
                  
                    NUMEROENTREGADIA++;
                    FechaArmado = FechaInicio;
                    FechaArmado = FechaArmado.AddMinutes(HolguraInicio);
                    FechaFin = FechaArmado;
                    FechaFin = FechaFin.AddMinutes(HolguraFinal);


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
                            //transaction = myConnection.BeginTransaction();

                            commandInsertar.Connection = myConnection;
                            //commandInsertar.Transaction = transaction;

                            string Query = string.Format(@"UPDATE T_ENC_ENTREGAS SET NUMEROENTREGADIA = ? ,FechaInicio = ? ,FechaArmado = ? ,FechaFin = ?, TiempoRuta = ?, TipoEvento = ?  WHERE ID = {0}", ListaTemporal[i].Id);

                            commandInsertar.Parameters.AddWithValue("@NUMEROENTREGADIA", SqlDbType.Int).Value = NUMEROENTREGADIA;
                            commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                            commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaArmado;
                            commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaFin;
                            commandInsertar.Parameters.AddWithValue("@TiempoRuta", SqlDbType.Int).Value = 0;
                            commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = "#376092";
                            commandInsertar.CommandText = Query;
                            commandInsertar.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                // Attempt to roll back the transaction.
                                //transaction.Rollback();
                                return "Error:" + e.Message;
                            }
                            catch
                            {
                                return "Error:" + e.Message;
                                // Do nothing here; transaction is not active.
                            }
                        }
                    }
                    FechaInicio = FechaFin;
                }

                return "Entregas ordendas exitosamente";
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
            return "";
        }
        public List<Models.Reserva_Detalle_Articulos> ArticulosEntrega(string IdEntrega)
        {
            try
            {
                List<Models.Reserva_Detalle_Articulos> Eventos_Articulos = new List<Models.Reserva_Detalle_Articulos>();

                string query = string.Format(@"SELECT T0.Id,T0.IDEntrega,T0.CodigoEvento,T0.CodigoArticulo,T0.Cantidad,T0.Remisionado,T0.EstadoArticulo,T0.UsrCreacion,T1.Descripcion, T1.Tiempo_Armado 
                                                FROM T_DET_ENTREGAS T0 LEFT JOIN Naf47.Arinda T1 ON T0.CodigoArticulo = T1.NO_ARTI
                                                WHERE IdEntrega = '{0}'
                                                group by T0.Id,T0.IDEntrega,T0.CodigoEvento,T0.CodigoArticulo,T0.Cantidad,T0.Remisionado,T0.EstadoArticulo,T0.UsrCreacion,
                                                T1.Descripcion, T1.Tiempo_Armado", IdEntrega);

                var resultado = dB.ConsultarDB(query, "T_EVENTOS");


                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva_Detalle_Articulos articulo = new Models.Reserva_Detalle_Articulos();
                    //articulo.Id = Convert.ToInt32(item["Id"].ToString());
                    articulo.Id = Convert.ToInt32(item["Id"].ToString());
                    articulo.IdEntrega = Convert.ToInt32(item["IdEntrega"].ToString());
                    articulo.NumEvento = item["CodigoEvento"].ToString();
                    articulo.CodigoArticulo = item["CodigoArticulo"].ToString();
                    articulo.Descripcion = item["Descripcion"].ToString();
                    articulo.TiempoArmado = string.IsNullOrEmpty(item["Tiempo_Armado"].ToString()) ? "0" : item["Tiempo_Armado"].ToString();
                    articulo.Cantidad = Convert.ToInt32(item["Cantidad"].ToString());
                    articulo.Remisionado = item["Remisionado"].ToString();
                    articulo.EstadoArticulo = item["EstadoArticulo"].ToString();
                    Eventos_Articulos.Add(articulo);
                }

                return Eventos_Articulos;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Models.Reserva_Detalle_Casos> CasossEntrega(string IdEntrega)
        {
            try
            {
                List<Models.Reserva_Detalle_Casos> Eventos_Articulos = new List<Models.Reserva_Detalle_Casos>();

                string query = string.Format(@"SELECT * FROM T_DET_CASOS_ENTREGAS WHERE IdEntrega = '{0}'", IdEntrega);

                var resultado = dB.ConsultarDB(query, "T_EVENTOS");
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva_Detalle_Casos articulo = new Models.Reserva_Detalle_Casos();
                    //articulo.Id = Convert.ToInt32(item["Id"].ToString());
                    articulo.Id = Convert.ToInt32(item["Id"].ToString());
                    articulo.NumEntrega = Convert.ToInt32(item["IdEntrega"].ToString());
                    articulo.NumCaso = item["NumCaso"].ToString();
                    articulo.Observaciones = item["Observaciones"].ToString();
                    articulo.Acciones = item["Acciones"].ToString();
                    Eventos_Articulos.Add(articulo);
                }

                return Eventos_Articulos;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string OrdenarEntregasMovimientos(string Cadena)
        {
            try
            {
                char[] demilitadores = { ';', ',' };
                string[] split = Cadena.Split(demilitadores);

                string PanelInicial = split[0];
                List<string> Temporal = new List<string>();
                Vehiculos vehiculos = new Vehiculos();
                var listado = vehiculos.ListadoVehiculos().OrderBy(x => x.TrasladoSiNo).ToList();
                for (int i = 1; i < split.Length; i++)
                {
                    var temp = listado.Find(x => x.Descripcion.Equals(split[i]));
                    if (temp==null)
                    {
                        Temporal.Add(split[i]);
                    }
                    else
                    {
                        OrdenarPorMovimiento(Temporal, PanelInicial);
                        PanelInicial = split[i];
                        Temporal = new List<string>();
                    }
                }
                if (Temporal.Count > 0)
                {
                    OrdenarPorMovimiento(Temporal, PanelInicial);
                }
                return "";
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }
        public string OrdenarPorMovimiento(List<string> Temporal, string PanelInicial)
        {
            dB = new conexionDB();
            try
            {
                string split = Temporal[0].Split('-')[2];

                if (Temporal.Count > 1)
                {
                    for (int j = 1; j < Temporal.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(Temporal[j]))
                        {
                            split += "," + Temporal[j].Split('-')[2];
                        }
                    }
                }

                string VehiculoFinal = string.Empty;
                var resultado = dB.ConsultarDB(string.Format("SELECT ID FROM T_VEHICULOS WHERE DESCRIPCION = '{0}'", PanelInicial), "T_ENC_ENTREGAS");
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    VehiculoFinal = item["ID"].ToString();
                }

                resultado = dB.ConsultarDB(string.Format("SELECT * FROM T_ENC_ENTREGAS WHERE ID IN ({0})", split), "T_ENC_ENTREGAS");

                List<Models.Reserva> ListadoReservas = null;
                ListadoReservas = new List<Models.Reserva>();
                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Reserva reserva = new Models.Reserva();
                    reserva.Id = Convert.ToInt32(item["Id"].ToString());
                    reserva.FechaInicio = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("HH:mm");
                    reserva.FechaArmadoT = Convert.ToDateTime(item["FechaInicio"].ToString());
                    reserva.FechaFin = Convert.ToDateTime(item["FechaFin"].ToString()).ToString("HH:mm");
                    reserva.FechaInicioOrdenar = Convert.ToDateTime(item["FechaFin"].ToString());
                    reserva.TiempoArmado = item["TiempoArmado"].ToString();
                    reserva.FechaRestriccionInicio = item["FechaRestriccionInicio"].ToString();
                    reserva.FechaRestriccionFin = item["FECHARECTRICCIONFIN"].ToString();
                    reserva.FechaRestriccionI = Convert.ToDateTime(item["FechaRestriccionInicio"].ToString());
                    reserva.FechaRestriccionF = Convert.ToDateTime(item["FECHARECTRICCIONFIN"].ToString());
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
                    reserva.Vehiculo = item["Vehiculo"].ToString();
                    reserva.Geolocalizacion = item["GeoloCalizacion"].ToString();
                    reserva.ColorTipoEvento = item["TipoEvento"].ToString();
                    ListadoReservas.Add(reserva);
                }

                string query = string.Empty;


                resultado = dB.ConsultarDB(@"SELECT ID,Valor FROM T_PARAMETROS_GENERALES WHERE Id = '2' OR ID = '3';", "T_ENC_ENTREGAS");
                int HolguraInicio = 0;
                int HolguraFinal = 0;

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    switch (item["Id"].ToString())
                    {
                        case "2":
                            HolguraInicio = int.Parse(item["Valor"].ToString());
                            break;
                        case "3":
                            HolguraFinal = int.Parse(item["Valor"].ToString());
                            break;
                    }
                }

                resultado = dB.ConsultarDB(string.Format(@"SELECT HORAINICIOLABORES FROM T_VEHICULOS WHERE ID = '{0}';", VehiculoFinal), "T_ENC_ENTREGAS");

                DateTime horainicioLabores = DateTime.Today;

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    horainicioLabores = Convert.ToDateTime(item["HORAINICIOLABORES"].ToString());
                }

                string[] arreglo = split.Split(',');

                Models.Reserva listaTemp = ListadoReservas.Where(x => x.Id.Equals(Convert.ToInt32(arreglo[0]))).First();

                DateTime FechaInicio = listaTemp.FechaInicioOrdenar.Date;
                DateTime FechaArmado = listaTemp.FechaInicioOrdenar.Date;
                DateTime FechaFin = listaTemp.FechaInicioOrdenar.Date;

                FechaInicio = FechaInicio.AddHours(horainicioLabores.Hour);
                FechaInicio = FechaInicio.AddMinutes(horainicioLabores.Minute);
                string geoprincipal = "14.644836805197727, -90.47603107394566";
                for (int i = 0; i < arreglo.Length; i++)
                {
                    listaTemp = ListadoReservas.Where(x => x.Id.Equals(Convert.ToInt32(arreglo[i]))).First();

                    FechaArmado = FechaInicio;

                    int temp = Gettiempo(listaTemp.Geolocalizacion, geoprincipal) / 60;

                    FechaArmado = FechaArmado.AddMinutes(HolguraInicio);
                    FechaArmado = FechaArmado.AddMinutes(temp);
                    if (string.IsNullOrEmpty(listaTemp.TiempoArmado))
                    {
                        listaTemp.TiempoArmado = "0";
                    }

                    TimeSpan Tamano = (FechaArmado - Convert.ToDateTime(listaTemp.FechaArmadoT));

                    FechaFin = FechaArmado;
                    FechaFin = FechaFin.AddMinutes(Convert.ToDouble(listaTemp.TiempoArmado));
                    FechaFin = FechaFin.AddMinutes(HolguraFinal);
                    string Color = "#00b050";
                    string hora = listaTemp.FechaRestriccionF.ToString("hh:mm:ss t");

                    /// Cambiar por el siguieten codigo

                    //if (!hora.Contains("12:00:00 A"))
                    //{
                    //    switch (FechaArmado > listaTemp.FechaRestriccionF)
                    //    {
                    //        case true:
                    //            Color = "#FF0000";
                    //            break;
                    //        case false:
                    //            Color = "#00b050";
                    //            break;

                    //    }
                    //}


                    // Parte de codigo para productivo
                    if (!hora.Contains("12:00:00 a"))
                    {
                        switch (FechaArmado > listaTemp.FechaRestriccionF)
                        {
                            case true:
                                Color = "#FF0000";
                                break;
                            case false:
                                Color = "#00b050";
                                break;

                        }
                    }


                    if (listaTemp.ColorTipoEvento.SequenceEqual("#376092"))
                    {
                        Color = "#376092";
                    }

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

                            string Query = string.Format(@"UPDATE T_ENC_ENTREGAS SET NUMEROENTREGADIA = ?,FechaInicio = ? ,FechaArmado = ? ,FechaFin = ? ,Vehiculo = ?,TipoEvento = ?, TiempoRuta= ?   WHERE ID = {0}", arreglo[i]);
                            int NumEntregaDia = (i + 1);
                            commandInsertar.Parameters.AddWithValue("@NUMEROENTREGADIA", SqlDbType.Int).Value = NumEntregaDia;
                            commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                            commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaArmado;
                            commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaFin;
                            commandInsertar.Parameters.AddWithValue("@Vehiculo", SqlDbType.Int).Value = Convert.ToInt32(VehiculoFinal);
                            commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = Color;
                            commandInsertar.Parameters.AddWithValue("@TiempoRuta", SqlDbType.Int).Value = temp;
                            commandInsertar.CommandText = Query;
                            commandInsertar.ExecuteNonQuery();
                            transaction.Commit();
                            //"Vehiculo creado exitosamente";
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
                                transaction.Rollback();
                                return "Error:" + e.Message;
                                // Do nothing here; transaction is not active.
                            }
                        }

                    }
                    FechaInicio = FechaFin;
                    geoprincipal = listaTemp.Geolocalizacion;
                }

                return "Exitoso";
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }

        public string CreateTraslado(Models.Traslados reserva, string UsrCreacion)
        {
            kalea2.Models.Usuarios oUsuario = (kalea2.Models.Usuarios)HttpContext.Current.Session["User"];
            string Respuesta = string.Empty;
            try
            {
                dB = new conexionDB();

                var temp = dB.ConsultarDB("SELECT IDTT_ENC_ENTREGAS.nextval from dual;", "NUM_ENTREGA");
                string NumEntrega = string.Empty;

                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    NumEntrega = item["nextval"].ToString();
                }

                DateTime FechaInicio = reserva.FechaEntrega;
                DateTime FechaArmado = reserva.FechaEntrega;
                DateTime FechaFin = reserva.FechaEntrega;

                string Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionInicio) ? "00:00" : reserva.FechaRestriccionInicio);

                DateTime FechaRectriccionInicio = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionFin) ? "00:00" : reserva.FechaRestriccionFin); ;

                DateTime FechaRectriccionFin = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

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
                        //transaction = myConnection.BeginTransaction();

                        commandInsertar.Connection = myConnection;
                        //commandInsertar.Transaction = transaction;

                        string Query = @"INSERT INTO T_ENC_ENTREGAS(Id,FechaInicio,FechaArmado,FechaFin,FechaRestriccionInicio,FechaRectriccionFin,
                                         Estado,UsrCreacion,FechaCreacion,NumeroEntregaDia,Vehiculo,TipoEvento) VALUES (?,?,?,?,?,?,?,?,?,?,?,?)";

                        commandInsertar.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Convert.ToInt32(NumEntrega);
                        commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaRectriccionInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaRectriccionInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaRectriccionFin;
                        commandInsertar.Parameters.AddWithValue("@FechaRestriccionInicio ", SqlDbType.Date).Value = FechaRectriccionInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaRectriccionFin ", SqlDbType.Date).Value = FechaRectriccionFin;
                        commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";
                        commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = oUsuario.Codigo;
                        commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Now;
                        commandInsertar.Parameters.AddWithValue("@NumeroEntregaDia", SqlDbType.Int).Value = 0;
                        commandInsertar.Parameters.AddWithValue("@Vehiculo", SqlDbType.Int).Value = Convert.ToInt32(reserva.Vehiculo);
                        commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = "#FF5733";
                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();

                        return "Vehiculo creado exitosamente";
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // Attempt to roll back the transaction.
                            //transaction.Rollback();
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

                throw;
            }
        }


        public string CrearEntregaParcial(Models.ReservaParcial reserva, string UsrCreacion)
        {
            kalea2.Models.Usuarios oUsuario = (kalea2.Models.Usuarios)HttpContext.Current.Session["User"];
            string Respuesta = string.Empty;
            try
            {
                dB = new conexionDB();

                var temp = dB.ConsultarDB("SELECT IDTT_ENC_ENTREGAS.nextval from dual;", "NUM_ENTREGA");
                string NumEntrega = string.Empty;

                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    NumEntrega = item["nextval"].ToString();
                }

                int TiempoArmado = 0;

                DateTime FechaInicio = reserva.FechaEntrega;
                DateTime FechaArmado = reserva.FechaEntrega;
                DateTime FechaFin = reserva.FechaEntrega;

                string Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionInicio) ? "00:00" : reserva.FechaRestriccionInicio);

                DateTime FechaRectriccionInicio = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                Temp = reserva.FechaEntrega.ToString("yyyy-MM-dd") + " " + (string.IsNullOrEmpty(reserva.FechaRestriccionFin) ? "00:00" : reserva.FechaRestriccionFin); ;

                DateTime FechaRectriccionFin = DateTime.ParseExact(Temp, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

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
                        //transaction = myConnection.BeginTransaction();

                        commandInsertar.Connection = myConnection;
                        //commandInsertar.Transaction = transaction;

                        string Query = @"INSERT INTO T_ENC_ENTREGAS(Id,FechaInicio,FechaArmado,FechaFin,FechaRestriccionInicio,FechaRectriccionFin,
                                         Estado,UsrCreacion,FechaCreacion,NumeroEntregaDia,Vehiculo,TipoEvento,Referencia_Reserva) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)";

                        commandInsertar.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Convert.ToInt32(NumEntrega);
                        commandInsertar.Parameters.AddWithValue("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaArmado", SqlDbType.Date).Value = FechaArmado;
                        commandInsertar.Parameters.AddWithValue("@FechaFin", SqlDbType.Date).Value = FechaFin;
                        commandInsertar.Parameters.AddWithValue("@FechaRestriccionInicio ", SqlDbType.Date).Value = FechaRectriccionInicio;
                        commandInsertar.Parameters.AddWithValue("@FechaRectriccionFin ", SqlDbType.Date).Value = FechaRectriccionFin;
                        commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";
                        commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.VarChar).Value = oUsuario.Codigo;
                        commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Now;
                        commandInsertar.Parameters.AddWithValue("@NumeroEntregaDia", SqlDbType.Int).Value = 0;
                        commandInsertar.Parameters.AddWithValue("@Vehiculo", SqlDbType.Int).Value = Convert.ToInt32(reserva.Vehiculo);
                        commandInsertar.Parameters.AddWithValue("@TipoEvento", SqlDbType.VarChar).Value = "#376092";
                        commandInsertar.Parameters.AddWithValue("@Referencia_Reserva", SqlDbType.VarChar).Value = reserva.Referencia;
                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();


                        //transaction.Commit();

                        Models.Reserva reserva2 = new Models.Reserva();
                        reserva2.Id = int.Parse(NumEntrega);
                        reserva2.FechaEntrega = FechaInicio;
                        reserva2.FechaInicio = reserva.FechaInicio;
                        reserva2.FechaRestriccionInicio = reserva.FechaRestriccionInicio;
                        reserva2.FechaRestriccionFin = reserva.FechaRestriccionFin;
                        reserva2.Vehiculo = reserva.Vehiculo;
                        reserva2.ColorTipoEvento = "#376092";

                        string resultado = ReOrdenarEntregas(FechaInicio, reserva.Vehiculo, reserva2);

                        if (resultado.Contains("Error"))
                        {
                            return resultado;
                        }

                        return "Vehiculo creado exitosamente";
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // Attempt to roll back the transaction.
                            //transaction.Rollback();
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

                throw;
            }

            return Respuesta;
        }

        public int Gettiempo(string Inicio, string Fin)
        {
            try
            {

                int distance = 0;
                string from = Inicio.Replace('(', ' ');
                from = from.Replace(')', ' ');

                string to = Fin.Replace('(', ' ');
                to = to.Replace(')', ' ');

                string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + from.Trim() + "&destinations=" + to.Trim() + "&key=AIzaSyBjmB6paM6r62Rh6baf5O2ulK7SY7GNUOU";
                string requesturl = url.Trim();
                //string requesturl = @"http://maps.googleapis.com/maps/api/directions/json?origin=" + from + "&alternatives=false&units=imperial&destination=" + to + "&sensor=false";
                string content = fileGetContents(requesturl);
                JObject o = JObject.Parse(content);
                try
                {
                    distance = (int)o.SelectToken("rows[0].elements[0].duration.value");
                    return distance;
                }
                catch
                {
                    return distance;
                }
                return distance;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected string fileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("https:") > -1)
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    //wc.Headers.Add(HttpRequestHeader.Authorization,"key " + "AIzaSyBjmB6paM6r62Rh6baf5O2ulK7SY7GNUOU");
                    //wc.QueryString.Add("key", "AIzaSyBjmB6paM6r62Rh6baf5O2ulK7SY7GNUOU");
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { sContents = "unable to connect to server "; }
            return sContents;
        }


        public string AnularEventosReservas()
        {
            try
            {
                dB = new conexionDB();

                var temp = dB.ConsultarDB("SELECT VALOR FROM T_PARAMETROS_GENERALES WHERE ID ='1'", "NUM_ENTREGA");
                int Tiempo = 0;
                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    Tiempo = Convert.ToInt32(item["VALOR"].ToString());
                }

                temp = dB.ConsultarDB("SELECT ID,FECHACREACION,FECHAINICIO,VEHICULO FROM T_ENC_ENTREGAS WHERE TIPOEVENTO = '#376092' AND ESTADO<>'AN';", "NUM_ENTREGA");
                DateTime dt = new DateTime();
                
                foreach (DataRow item in temp.Tables[0].Rows)
                {
                    dt = Convert.ToDateTime(item["FECHACREACION"].ToString());

                    int NumMinutos = (DateTime.Now -dt).Minutes + (DateTime.Now - dt).Hours*60;
                    if (NumMinutos>Tiempo)
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
                                string Query = string.Format("UPDATE T_ENC_ENTREGAS SET Estado = '{0}' WHERE Id = '{1}'", "AN", item["ID"].ToString());

                                commandInsertar.CommandText = Query;
                                commandInsertar.ExecuteNonQuery();
                                transaction.Commit();

                                DateTime dtf = Convert.ToDateTime(item["FECHAINICIO"].ToString());

                                dtf = Convert.ToDateTime(dtf.ToString("yyyy-MM-dd"));

                                ReOrdenarEntregas(dtf, item["VEHICULO"].ToString(),new Models.Reserva());
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

                return "Exitoso";

            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }


        }


        
        
    }
}
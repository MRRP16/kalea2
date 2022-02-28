using kalea2.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class ReservasPorVehiculo : conexionDB
    {
        conexionDB dB;
        public RespuestaReservaPorVehiculo ObtenerReservas(string id, DateTime FechaInicio , DateTime FechaFin)
        {
            dB = new conexionDB();
            RespuestaReservaPorVehiculo respuesta = new RespuestaReservaPorVehiculo();

            try
            {
                int NumDias = (FechaFin - FechaInicio).Days+1;

                List<ReservaPorVehiculo> listadoFinal = new List<ReservaPorVehiculo>();

                var resultadoV = dB.ConsultarDB("SELECT * FROM T_VEHICULOS WHERE ID ='" + id + "'", "T_VEHICULOS");
                foreach (DataRow item in resultadoV.Tables[0].Rows)
                {
                    Models.Vehiculos vehiculo = new Models.Vehiculos()
                    {
                        Codigo = Convert.ToInt32(item["Id"].ToString()),
                        Descripcion = item["Descripcion"].ToString(),
                        Placa = item["Placa"].ToString(),
                        PesoCarga = item["PesoCarga"].ToString(),
                        VolumenCarga = item["VolumenCarga"].ToString(),
                        Piloto = item["Piloto"].ToString(),
                        Estado = item["Estado"].ToString()

                    };

                    respuesta.Vehiculo = vehiculo;
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


                string inicial = string.Empty;
                DateTime dtmInicial = DateTime.Today;
                DateTime dtmFinal = DateTime.Today;
                DateTime dtmInicialC = DateTime.Today;
                DataRow dt = null;
                foreach (var fecha2 in Enumerable.Range(0, NumDias).Select(i => FechaInicio.AddDays(i).ToString("yyyy-MM-dd")))
                {
                    string Query = string.Format(@"SELECT T0.*,T1.CODIGOEVENTO,T2.NUMCASO FROM T_ENC_ENTREGAS T0 
                                                LEFT JOIN T_DET_ENTREGAS T1 ON T0.ID = T1.IDENTREGA
                                                LEFT JOIN T_DET_CASOS_ENTREGAS T2 ON T0.ID = T2.IDENTREGA
                                                WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') 
                                                and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')
                                                AND vehiculo ='{1}'
                                                AND T0.Estado = 'A'
                                                ORDER BY T0.ID ASC;", fecha2,id);


                    resultado = dB.ConsultarDB(Query, "T_ENC_ENTREGAS");
                    List<Reserva> listadoreservas = new List<Reserva>();
                    
                    if (resultado != null)
                    {
                        foreach (DataRow item in resultado.Tables[0].Rows)
                        {
                            if (inicial != item["Id"].ToString())
                            {
                                Models.Reserva reserva = new Models.Reserva();
                                reserva.Id = Convert.ToInt32(item["Id"].ToString());
                                reserva.FechaInicio = Convert.ToDateTime(item["FechaInicio"].ToString()).ToString("HH:mm");
                                reserva.FechaFin = Convert.ToDateTime(item["FechaFin"].ToString()).ToString("HH:mm");
                                reserva.FechaArmado = Convert.ToDateTime(item["FechaArmado"].ToString()).ToString("HH:mm");
                                reserva.TiempoArmado = item["TiempoArmado"].ToString();
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
                                reserva.TamanioTarjeta = (NumMinutos * 4 * -1) + SumEspacios;
                                reserva.TiempoRuta = string.IsNullOrEmpty(item["TiempoRuta"].ToString()) ? 0 : Convert.ToInt32(item["TiempoRuta"].ToString());

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
                                listadoreservas.Add(reserva);
                                inicial = item["Id"].ToString();
                            }
                        }
                    }

                    listadoreservas = listadoreservas.OrderBy(x => x.NumeroEntregaDia).ToList();

                    if (listadoreservas.Count > 0)
                    {
                        List<Models.Reserva> someVariable2 = (from s in listadoreservas
                                                              where s.NumeroEntregaDia == "1"
                                                              select s).ToList();

                        int contador = 0;
                       
                        foreach (var item2 in someVariable2)
                        {
                            dt = resultado.Tables[0].Select("ID = '" + item2.Id + "'").First();
                            if (dtmInicial.Equals(DateTime.Today))
                            {
                                dtmInicial = Convert.ToDateTime(dt["FechaInicio"].ToString());
                                dtmInicial = dtmInicial.AddHours(-1);
                                dtmInicialC = dtmInicial;
                            }
                            if (dtmInicial > Convert.ToDateTime(dt["FechaInicio"].ToString()))
                            {
                                dtmInicial = Convert.ToDateTime(dt["FechaInicio"].ToString());
                                dtmInicial = dtmInicial.AddHours(-1);
                                dtmInicialC = dtmInicial;
                            }
                        }

                        int NumMinutos = 0;
                        int SumEspacios = 0;
                        List<Models.Reserva> Iniciales = new List<Models.Reserva>();
                        foreach (var item2 in someVariable2)
                        {
                            Models.Reserva reserva = new Models.Reserva();
                            reserva.FechaInicio = dtmInicial.ToString("HH:mm");
                            reserva.FechaFin = item2.FechaInicio;
                            reserva.Coordenadas = "14.644836805197727, -90.47603107394566";
                            reserva.Geolocalizacion = "14.644836805197727, -90.47603107394566";
                            reserva.Vehiculo = item2.Vehiculo;
                            reserva.NumeroEntregaDia = "I";
                            reserva.ColorTipoEvento = "#00b050";
                            dt = resultado.Tables[0].Select("ID = '" + item2.Id + "'").First();

                            DateTime dtInit = Convert.ToDateTime(dt["FechaInicio"].ToString());
                            dtInit = dtInit.AddHours(-1);
                            NumMinutos = (dtmInicial.Minute - dtInit.Minute) + (dtmInicial - dtInit).Hours * 60;

                            SumEspacios = (NumMinutos / 60) * 40 * 1;
                            reserva.TamanioTarjetaTranspareante = (NumMinutos * 4 * 1) + SumEspacios;
                            if (reserva.TamanioTarjetaTranspareante < 0)
                            {
                                reserva.TamanioTarjetaTranspareante = reserva.TamanioTarjetaTranspareante * -1;
                            }


                            dtInit = Convert.ToDateTime(dt["FechaInicio"].ToString());
                            dtInit = dtInit.AddHours(-1);
                            NumMinutos = (Convert.ToDateTime(dt["FechaInicio"].ToString()).Minute - dtInit.Minute) + (Convert.ToDateTime(dt["FechaInicio"].ToString()) - dtInit).Hours * 60;

                            //DateTime dtm = Convert.ToDateTime(dt["FechaInicio"].ToString());
                            //NumMinutos = (Convert.ToDateTime(dt["FechaInicio"].ToString()).Minute - dtmInicial.Minute) + (Convert.ToDateTime(dt["FechaInicio"].ToString()) - dtmInicial).Hours * 60;
                            SumEspacios = (NumMinutos / 60) * 40 * 1;
                            if (SumEspacios == 0)
                            {
                                SumEspacios = 40;
                            }
                            reserva.TamanioTarjeta = (NumMinutos * 4 * 1) + SumEspacios;
                            if (reserva.TamanioTarjeta < 0)
                            {
                                reserva.TamanioTarjeta = reserva.TamanioTarjeta * -1;
                            }
                            listadoreservas.Add(reserva);
                        }
                       
 
                        List<Models.Reserva> listTemp = new List<Reserva>();
                        listTemp = listadoreservas.Where(ve => ve.Id != 0).OrderBy(x => x.NumeroEntregaDia).ToList();
                        Models.Reserva someVariable3 = ((Reserva)(from s in listTemp
                                                                  where s.NumeroEntregaDia == listTemp[listTemp.Count - 1].NumeroEntregaDia
                                                                  select s).First());
                        dt = resultado.Tables[0].Select("ID = '" + someVariable3.Id + "'").First();
                        int temp = 0;
                        temp = Gettiempo(someVariable3.Geolocalizacion.TrimStart('(').TrimEnd(')'), "14.644836805197727, -90.47603107394566") / 60;

                        Models.Reserva reserva1 = new Models.Reserva();

                        DateTime Fechainicio = Convert.ToDateTime(dt["FechaFin"].ToString());

                        reserva1.NumeroEntregaDia = "F";
                        reserva1.Vehiculo = someVariable3.Vehiculo;
                        reserva1.Geolocalizacion = "14.644836805197727, -90.47603107394566";
                        reserva1.FechaInicio = Convert.ToDateTime(dt["FechaFin"].ToString()).ToString("HH:mm");

                        FechaFin = Fechainicio.AddMinutes(temp);
                        reserva1.FechaFin = FechaFin.ToString("HH:mm");

                        if (FechaFin.Hour > 16)
                        {
                            reserva1.ColorTipoEvento = "#FF0000";
                        }
                        else
                        {
                            reserva1.ColorTipoEvento = "#00b050";
                        }

                        if (dtmFinal.Equals(DateTime.Today))
                        {
                            dtmFinal = FechaFin;
                        }
                        if (dtmFinal < FechaFin)
                        {
                            dtmFinal = FechaFin;
                        }

                        NumMinutos = (FechaFin.Minute - Fechainicio.Minute) + (FechaFin.Hour - Fechainicio.Hour) * 60;
                        SumEspacios = (NumMinutos / 60) * 40 * 1;
                        if (SumEspacios == 0)
                        {
                            SumEspacios = 40;
                        }

                        reserva1.TamanioTarjeta = (NumMinutos * 4 * 1) + SumEspacios;
                        if (reserva1.TamanioTarjeta < 0)
                        {
                            reserva1.TamanioTarjeta = reserva1.TamanioTarjeta * -1;
                        }
                        listadoreservas.Add(reserva1);


                    }

                    string f = Convert.ToDateTime(fecha2).ToString("dd/MM/yyyy");
                    ReservaPorVehiculo res = new ReservaPorVehiculo {
                        Id = id,
                        Fecha = f.ToString(),
                        Listado = listadoreservas
                    };
                    listadoFinal.Add(res);
                }
                respuesta.Listado = listadoFinal;
                respuesta.Horas = new List<string>();
                dtmInicialC = dtmInicialC.AddMinutes(-dtmInicialC.Minute);
                int DiferenciaHoras = dtmFinal.Hour - dtmInicialC.Hour;
                respuesta.Horas.Add(dtmInicialC.ToString("HH:mm"));
                for (int i = 0; i < DiferenciaHoras; i++)
                {
                    dtmInicialC = dtmInicialC.AddHours(1);
                    respuesta.Horas.Add(dtmInicialC.ToString("HH:mm"));
                }
                dtmInicialC = dtmInicialC.AddHours(1);
                respuesta.Horas.Add(dtmInicialC.ToString("HH:mm"));

                return respuesta;
            }
            catch (Exception ex)
            {
                return respuesta;
            }
        }


        public string OrdenarPorMovimiento(List<string> Temporal, string PanelInicial,DateTime FechaPedido)
        {
            dB = new conexionDB();
            try
            {
                string split = Temporal[0];

                for (int j = 1; j < Temporal.Count; j++)
                {
                    if (!string.IsNullOrEmpty(Temporal[j]))
                    {
                        split += "," + Temporal[j];
                    }
                }
 
                var resultado = dB.ConsultarDB(string.Format("SELECT * FROM T_ENC_ENTREGAS WHERE ID IN ({0})", split), "T_ENC_ENTREGAS");

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
                    //if (reserva.FechaInicio.)
                    //{

                    //}
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

                resultado = dB.ConsultarDB(string.Format(@"SELECT HORAINICIOLABORES FROM T_VEHICULOS WHERE ID = '{0}';", PanelInicial), "T_ENC_ENTREGAS");

                DateTime horainicioLabores = DateTime.Today;

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    horainicioLabores = Convert.ToDateTime(item["HORAINICIOLABORES"].ToString());
                }

                string[] arreglo = split.Split(',');

                Models.Reserva listaTemp = ListadoReservas.Where(x => x.Id.Equals(Convert.ToInt32(arreglo[0]))).First();

                DateTime FechaInicio = FechaPedido.Date;
                DateTime FechaArmado = FechaPedido.Date;
                DateTime FechaFin = FechaPedido.Date;

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
                    FechaFin = FechaFin.AddMinutes(Convert.ToInt32(listaTemp.TiempoArmado));
                    FechaFin = FechaFin.AddMinutes(HolguraFinal);
                    string Color = "#00b050";
                    string hora = listaTemp.FechaRestriccionF.ToString("hh:mm:ss t");
                    /// Cambiar por el siguieten codigo
                    DateTime date = DateTime.Today;
                    if (FechaArmado.Date != listaTemp.FechaRestriccionF)
                    {
                        date = FechaArmado.Date;
                        date = date.AddHours(listaTemp.FechaRestriccionF.Hour) ;
                        date = date.AddMinutes(listaTemp.FechaRestriccionF.Minute);
                        date = date.AddSeconds(listaTemp.FechaRestriccionF.Second);
                        listaTemp.FechaRestriccionF = date;
                    }
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
                    if (listaTemp.ColorTipoEvento.SequenceEqual("#ffff00"))
                    {
                        Color = "#ffff00";
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
                            commandInsertar.Parameters.AddWithValue("@Vehiculo", SqlDbType.Int).Value = Convert.ToInt32(PanelInicial);
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
        public int Gettiempo(string Inicio, string Fin)
        {
            try
            {

                int distance = 0;
                string from = Inicio.Replace('(', ' ');
                from = from.Replace(')', ' ');

                string to = Fin.Replace('(', ' ');
                to = to.Replace(')', ' ');

                string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + from.Trim() + "&destinations=" + to.Trim() + "&key=AIzaSyDnIlcIlvpnZ6LplbZ7S-quFDZgZMh6Eig";
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
    }
}
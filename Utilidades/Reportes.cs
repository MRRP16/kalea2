using kalea2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{

    public class Reportes
    {
        conexionDB dB;
        DataSet resultadoArticulos;
        public List<ReporteDeProgramacion> getEntregas(string fecha, int vehiculo)
        {
            dB = new conexionDB();
            List<ReporteDeProgramacion> listado = new List<ReporteDeProgramacion>();
            //RespuestaReporteDeEntrega respuesta = new RespuestaReporteDeEntrega();
            if (fecha != null && vehiculo != null)
            {
                try
                {
                    string query = string.Empty;
                    switch (vehiculo)
                    {
                        case 0:

                            List<Models.Vehiculos> vehiculos = getVehiculos();
                            string numve = string.Empty;
                            foreach (var item in vehiculos)
                            {
                                if (item.Codigo != 0)
                                {
                                    numve += "'"+item.Codigo +"',";
                                }
                            }
                            numve = numve.Remove(numve.Length - 1, 1);

                            query = string.Format("SELECT T.id, T.codigoevento, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, " +
                        "T.telefono, T.celular, T.tiempoarmado, T.comentariostorre, T.VEHICULO, SUM(T.PESO) AS PESO, SUM(T.VOLUMEN)AS VOLUMEN, SUM(T.COSTO) AS COSTO, " +
                        "1 AS RADIO, T.ETIQUETAS, T.PRIORIDAD, T.FechaRestriccionInicio, T.FECHARECTRICCIONFIN " +
                        "FROM( SELECT t0.id, t1.codigoevento, t0.nombrecliente, t0.direccionentrega, t0.geolocalizacion, t0.fechainicio, t0.fechafin, t0.telefono, t0.celular, " +
                        "t0.tiempoarmado, to_char(substr(t0.COMENTARIOSTORRE, 1, 500)) as COMENTARIOSTORRE, v.descripcion AS VEHICULO, (COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PESO, 0)) AS PESO, 0 AS VOLUMEN, " +
                        "(COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PRECIOBASE, 0)) AS COSTO, 1 AS RADIO, 'NA' AS ETIQUETAS , t0.numeroentregadia AS PRIORIDAD, " +
                        "T0.FechaRestriccionInicio, T0.FECHARECTRICCIONFIN " +
                        "FROM T_ENC_ENTREGAS T0 " +
                        "INNER JOIN T_DET_ENTREGAS T1 ON T0.ID = t1.identrega " +
                        "INNER JOIN Naf47.Arinda T2 ON T1.CODIGOARTICULO = T2.NO_ARTI " +
                        "INNER JOIN T_VEHICULOS v ON t0.vehiculo = v.id " +
                        "WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') " +
                        "and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss') " +
                        "AND t0.vehiculo IN ({1}) " +
                        "AND T0.ESTADO <> 'AN' " +
                        "GROUP BY t0.nombrecliente, t0.direccionentrega, t0.fechainicio, t0.fechafin, t0.geolocalizacion, t0.id, t1.codigoevento, t0.nitcliente, " +
                        "t0.telefono, t0.celular, t0.tiempoarmado, to_char(substr(t0.COMENTARIOSTORRE, 1, 500)), v.descripcion, t0.numeroentregadia, (COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PESO, 0)), " +
                        "(COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PRECIOBASE, 0)), T1.CODIGOARTICULO, T0.FECHARECTRICCIONFIN, T0.FechaRestriccionInicio) T " +
                        "GROUP BY T.id, T.codigoevento, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, T.telefono, T.celular, " +
                        "T.tiempoarmado, T.comentariostorre, T.VEHICULO, T.RADIO, T.ETIQUETAS, T.PRIORIDAD, T.FECHARECTRICCIONFIN, T.FechaRestriccionInicio " +
                        "ORDER BY T.PRIORIDAD ASC; ", fecha, numve.ToString());
                            break;
                        default:
                            query = string.Format("SELECT T.id, T.codigoevento, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, " +
                        "T.telefono, T.celular, T.tiempoarmado, T.comentariostorre, T.VEHICULO, SUM(T.PESO) AS PESO, SUM(T.VOLUMEN)AS VOLUMEN, SUM(T.COSTO) AS COSTO, " +
                        "1 AS RADIO, T.ETIQUETAS, T.PRIORIDAD, T.FechaRestriccionInicio, T.FECHARECTRICCIONFIN " +
                        "FROM( SELECT t0.id, t1.codigoevento, t0.nombrecliente, t0.direccionentrega, t0.geolocalizacion, t0.fechainicio, t0.fechafin, t0.telefono, t0.celular, " +
                        "t0.tiempoarmado, to_char(substr(t0.COMENTARIOSTORRE, 1, 500)) as COMENTARIOSTORRE, v.descripcion AS VEHICULO, (COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PESO, 0)) AS PESO, 0 AS VOLUMEN, " +
                        "(COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PRECIOBASE, 0)) AS COSTO, 1 AS RADIO, 'NA' AS ETIQUETAS , t0.numeroentregadia AS PRIORIDAD, " +
                        "T0.FechaRestriccionInicio, T0.FECHARECTRICCIONFIN " +
                        "FROM T_ENC_ENTREGAS T0 " +
                        "INNER JOIN T_DET_ENTREGAS T1 ON T0.ID = t1.identrega " +
                        "INNER JOIN Naf47.Arinda T2 ON T1.CODIGOARTICULO = T2.NO_ARTI " +
                        "INNER JOIN T_VEHICULOS v ON t0.vehiculo = v.id " +
                        "WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss') " +
                        "and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss') " +
                        "AND t0.vehiculo = '{1}' " +
                        "AND T0.ESTADO <> 'AN' " +
                        "GROUP BY t0.nombrecliente, t0.direccionentrega, t0.fechainicio, t0.fechafin, t0.geolocalizacion, t0.id, t1.codigoevento, t0.nitcliente, " +
                        "t0.telefono, t0.celular, t0.tiempoarmado, to_char(substr(t0.COMENTARIOSTORRE, 1, 500)), v.descripcion, t0.numeroentregadia, (COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PESO, 0)), " +
                        "(COALESCE(T1.CANTIDAD, 0) * COALESCE(T2.PRECIOBASE, 0)), T1.CODIGOARTICULO, T0.FECHARECTRICCIONFIN, T0.FechaRestriccionInicio) T " +
                        "GROUP BY T.id, T.codigoevento, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, T.telefono, T.celular, " +
                        "T.tiempoarmado, T.comentariostorre, T.VEHICULO, T.RADIO, T.ETIQUETAS, T.PRIORIDAD, T.FECHARECTRICCIONFIN, T.FechaRestriccionInicio " +
                        "ORDER BY T.PRIORIDAD ASC; ", fecha, vehiculo.ToString());
                            break;
                    }


                    var resultado = dB.ConsultarDB(query, "T_ENC_ENTREGAS");

                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {

                        var geolocalizacion = item["GEOLOCALIZACION"].ToString();
                        string[] arreglo = geolocalizacion.Split(',');
                        string[] lat = arreglo[0].Split('(');
                        string[] lon = arreglo[1].Split(')');
                        var latitud = lat[1];
                        var longitud = lon[0];

                        ReporteDeProgramacion reporte = new ReporteDeProgramacion()
                        {
                            Evento = item["CODIGOEVENTO"].ToString(),
                            Cliente = item["NOMBRECLIENTE"].ToString(),
                            DireccionEntrega = item["DIRECCIONENTREGA"].ToString(),
                            Latitud = latitud,
                            Longitud = longitud,
                            TiempoInicio = Convert.ToDateTime(item["FECHAINICIO"].ToString()).ToString("yyyy.MM.dd HH:mm"),
                            TiempoFin = Convert.ToDateTime(item["FECHAFIN"].ToString()).ToString("yyyy.MM.dd HH:mm"),
                            Telefono1 = item["TELEFONO"].ToString(),
                            Telefono2 = item["CELULAR"].ToString(),
                            Email = "NULL",
                            TiempoArmado = item["TIEMPOARMADO"].ToString(),
                            Peso = "NULL",
                            Volumen = "NULL",
                            Costo = "NULL",
                            TipoVehiculo = item["VEHICULO"].ToString(),
                            Radio = "1",
                            Etiquetas = "NULL",
                            Prioridad = item["PRIORIDAD"].ToString(),
                        };
                        listado.Add(reporte);
                    }

                    switch (vehiculo)
                    {
                        case 0:

                            List<Models.Vehiculos> vehiculos = getVehiculos();
                            string numve = string.Empty;
                            foreach (var item in vehiculos)
                            {
                                if (item.Codigo != 0)
                                {
                                    numve += "'" + item.Codigo + "',";
                                }
                            }
                            numve = numve.Remove(numve.Length - 1, 1);

                            query = string.Format(@"SELECT T.id, T1.NUMCASO as codigoevento, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, T.telefono, T.celular, T.tiempoarmado,to_char(substr(T.comentariostorre, 1, 500)) as comentariostorre ,
                                                    v.descripcion AS VEHICULO, '0' AS PESO, '0' AS VOLUMEN, '0' AS COSTO, 1 AS RADIO, 'NA' AS ETIQUETAS, T.numeroentregadia AS PRIORIDAD, T.FechaRestriccionInicio, T.FECHARECTRICCIONFIN
                                                    FROM T_ENC_ENTREGAS T
                                                    INNER JOIN T_DET_CASOS_ENTREGAS T1 ON T.ID = t1.identrega
                                                    INNER JOIN T_VEHICULOS v ON T.vehiculo = v.id
                                                    WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss')
                                                    and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')
                                                    AND T.vehiculo IN({1})
                                                    AND T.ESTADO <> 'AN'
                                                    GROUP BY T.id, T1.NUMCASO, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, T.telefono, T.celular, T.tiempoarmado,to_char(substr(T.comentariostorre, 1, 500)),
                                                    v.descripcion, '0', '0', '0', 1, 'NA', T.numeroentregadia, T.FechaRestriccionInicio, T.FECHARECTRICCIONFIN
                                                    ORDER BY T.numeroentregadia ASC; ", fecha, numve.ToString());
                            break;
                        default:
                            query = string.Format(@"SELECT T.id, T1.NUMCASO as codigoevento, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, T.telefono, T.celular, T.tiempoarmado,to_char(substr(T.comentariostorre, 1, 500)) as comentariostorre,
                                                    v.descripcion AS VEHICULO, '0' AS PESO, '0' AS VOLUMEN, '0' AS COSTO, 1 AS RADIO, 'NA' AS ETIQUETAS, T.numeroentregadia AS PRIORIDAD, T.FechaRestriccionInicio, T.FECHARECTRICCIONFIN
                                                    FROM T_ENC_ENTREGAS T
                                                    INNER JOIN T_DET_CASOS_ENTREGAS T1 ON T.ID = t1.identrega
                                                    INNER JOIN T_VEHICULOS v ON T.vehiculo = v.id
                                                    WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'yyyy-MM-dd hh24:mi:ss')
                                                    and FechaInicio <= to_timestamp('{0} 23:59:59', 'yyyy-MM-dd hh24:mi:ss')
                                                    AND T.vehiculo = '{1}'
                                                    AND T.ESTADO <> 'AN'
                                                    GROUP BY T.id, T1.NUMCASO, T.nombrecliente, T.direccionentrega, T.geolocalizacion, T.fechainicio, T.fechafin, T.telefono, T.celular, T.tiempoarmado, to_char(substr(T.comentariostorre, 1, 500)),
                                                    v.descripcion, '0', '0', '0', 1, 'NA', T.numeroentregadia, T.FechaRestriccionInicio, T.FECHARECTRICCIONFIN
                                                    ORDER BY T.numeroentregadia ASC; ", fecha, vehiculo.ToString());
                            break;
                    }


                     resultado = dB.ConsultarDB(query, "T_ENC_ENTREGAS");

                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {

                        var geolocalizacion = item["GEOLOCALIZACION"].ToString();
                        string[] arreglo = geolocalizacion.Split(',');
                        string[] lat = arreglo[0].Split('(');
                        string[] lon = arreglo[1].Split(')');
                        var latitud = lat[1];
                        var longitud = lon[0];

                        ReporteDeProgramacion reporte = new ReporteDeProgramacion()
                        {
                            Evento = item["CODIGOEVENTO"].ToString(),
                            Cliente = item["NOMBRECLIENTE"].ToString(),
                            DireccionEntrega = item["DIRECCIONENTREGA"].ToString(),
                            Latitud = latitud,
                            Longitud = longitud,
                            TiempoInicio = Convert.ToDateTime(item["FECHAINICIO"].ToString()).ToString("yyyy.MM.dd HH:mm"),
                            TiempoFin = Convert.ToDateTime(item["FECHAFIN"].ToString()).ToString("yyyy.MM.dd HH:mm"),
                            Telefono1 = item["TELEFONO"].ToString(),
                            Telefono2 = item["CELULAR"].ToString(),
                            Email = "NULL",
                            TiempoArmado = item["TIEMPOARMADO"].ToString(),
                            Peso = "NULL",
                            Volumen = "NULL",
                            Costo = "NULL",
                            TipoVehiculo = item["VEHICULO"].ToString(),
                            Radio = "1",
                            Etiquetas = "NULL",
                            Prioridad = item["PRIORIDAD"].ToString(),
                        };
                        listado.Add(reporte);
                    }

                    //respuesta.ListadoReporteDeProgramacion = listado;
                    //respuesta.Vehiculos = getVehiculos();

                    return listado;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                //respuesta.ListadoReporteDeProgramacion = listado;
                //respuesta.Vehiculos = getVehiculos();
                return null;
            }

        }

        public List<Models.Vehiculos> getVehiculos()
        {
            dB = new conexionDB();
            List<Models.Vehiculos> listado = new List<Models.Vehiculos>();
            var resultado = dB.ConsultarDB("SELECT * FROM T_VEHICULOS", "T_VEHICULOS");
            Models.Vehiculos vehiculo = new Models.Vehiculos();
            //{
            //    Codigo = 0,
            //    Descripcion = "Todos"
            //};
            //listado.Add(vehiculo);
            foreach (DataRow item in resultado.Tables[0].Rows)
            {
                vehiculo = new Models.Vehiculos()
                {
                    Codigo = int.Parse(item["ID"].ToString()),
                    Descripcion = item["DESCRIPCION"].ToString()
                };
                listado.Add(vehiculo);
            }

            return listado;
        }

        public List<Models.Vehiculos> getVehiculosTransporte()
        {
            dB = new conexionDB();
            List<Models.Vehiculos> listado = new List<Models.Vehiculos>();
            var resultado = dB.ConsultarDB("SELECT * FROM T_VEHICULOS", "T_VEHICULOS");
            Models.Vehiculos vehiculo = new Models.Vehiculos()
            {
                Codigo = 0,
                Descripcion = "Todos"
            };
            listado.Add(vehiculo);
            foreach (DataRow item in resultado.Tables[0].Rows)
            {
                vehiculo = new Models.Vehiculos()
                {
                    Codigo = int.Parse(item["ID"].ToString()),
                    Descripcion = item["DESCRIPCION"].ToString()
                };
                listado.Add(vehiculo);
            }

            return listado;
        }

        public List<ReportesGuias> GetEventosCasosParaGuiasDeTransporte(string vehiculoId, string fecha)
        {
            dB = new conexionDB();
            string id = "2";
            List<ReportesGuias> listadoDeEventos = new List<ReportesGuias>();
            try
            {
                string query = string.Format(@"SELECT t1.descripcion as vehiculo, t2.codigoevento, t2.codigoarticulo, t3.NOMBRE_LARGO AS descripcion, t2.cantidad, t4.nombre_vendedor as vendedor, t0.nombrecliente,T0.personarecepcion,T0.personarecepcion, t0.telefono, t0.celular, t0.direccionentrega, 
                    to_char(substr(t0.comentariosventas, 1, 500)) as comentariosventas,to_char(substr(t0.COMENTARIOSTORRE, 1, 500)) as COMENTARIOSTORRE, t0.fechainicio, t0.fecharestriccioninicio as restriccioninicio, t0.fecharectriccionfin as restriccionfin, t5.numcaso, t5.observaciones, t5.acciones, t0.id as identrega,T6.BODEGA,T7.DESCRIPCION AS TIPOINSTALACION
                    FROM t_enc_entregas T0 
                    LEFT JOIN t_vehiculos T1 ON t1.id = t0.vehiculo
                    LEFT JOIN t_det_entregas T2 ON t0.id = t2.identrega
                    LEFT JOIN naf47.arinda T3 ON t2.codigoarticulo = t3.no_arti
                    LEFT JOIN naf47.v_eventos_pendientes T4 ON t4.evento = t2.codigoevento
                    LEFT JOIN t_det_casos_entregas T5 ON t0.id = t5.identrega
                    LEFT JOIN Naf47.Pvlineas_movimiento T6 ON T6.NO_TRANSA_MOV = T2.CODIGOEVENTO AND t2.codigoarticulo = T6.NO_ARTI
                    LEFT JOIN T_TIPOS_CASO T7 ON T7.ID = T0.TIPOINSTALACION
                    WHERE FechaInicio >= to_timestamp('{0} 00:00:00', 'dd/MM/yy hh24:mi:ss') AND FechaInicio <= to_timestamp('{0} 23:59:59', 'dd/MM/yy hh24:mi:ss') AND t0.vehiculo = {1}
                    AND T0.ESTADO='A'
                    GROUP BY t1.descripcion, t2.codigoevento, t2.codigoarticulo, t3.NOMBRE_LARGO, t2.cantidad, t4.nombre_vendedor , t0.nombrecliente,T0.personarecepcion,t0.personarecepcion, t0.telefono, t0.celular, t0.direccionentrega, 
                    to_char(substr(t0.comentariosventas, 1, 500)),to_char(substr(t0.COMENTARIOSTORRE, 1, 500)) , t0.fechainicio, t0.fecharestriccioninicio , t0.fecharectriccionfin, t5.numcaso, t5.observaciones, t5.acciones, t0.id ,T6.BODEGA,T7.DESCRIPCION;", fecha, vehiculoId);

                var resultado = dB.ConsultarDB(query, "T_EVENTOS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    if (listadoDeEventos.Count == 0)
                    {
                        AgregarEventoNuevo(item, listadoDeEventos);
                    }
                    else
                    {
                        var posicion = ValidarSiExisteEventoEnListado(item, listadoDeEventos);
                        if (posicion != null)
                        {
                            AgregarProductoAEventoExistente(item, posicion, listadoDeEventos);
                        }
                        else
                        {
                            AgregarEventoNuevo(item, listadoDeEventos);
                        }
                    }
                }
                return listadoDeEventos;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        private string GetHorarioRestriccion(string horarioRestriccionInicio, string horarioRestriccionFin)
        {
            string[] restriccionInicio = horarioRestriccionInicio.Split(' ', '.');
            string[] restriccionFin = horarioRestriccionFin.Split(' ', '.');
            string respuesta = restriccionInicio[1] + " - " + restriccionFin[1];
            return respuesta;
        }

        private void AgregarEventoNuevo(DataRow item, List<ReportesGuias> listado)
        {
            ReportesGuias evento = new ReportesGuias();
            evento.IdEntrega = item["identrega"].ToString();
            evento.FechaDeEntrega = item["fechaInicio"].ToString();
            evento.Vehiculo = item["vehiculo"].ToString();
            evento.EventoCaso = item["codigoevento"].ToString();
            evento.Armadores = "";
            evento.HorarioRestriccion = GetHorarioRestriccion(item["restriccioninicio"].ToString(), item["restriccionfin"].ToString());
            evento.VendedorNombre = item["vendedor"].ToString();
            evento.SolucionesNombre = "";
            evento.ClienteNombre = item["nombreCliente"].ToString();
            evento.ClienteTelefono = item["telefono"].ToString() + " - " + item["celular"].ToString();
            evento.ClienteDireccionEntrega = item["direccionentrega"].ToString();
            evento.ObservacionesTorre = item["comentariostorre"].ToString();
            evento.ObservacionesEvento = item["comentariosventas"].ToString();
            evento.NumeroCaso = item["numcaso"].ToString();
            evento.ObservacionesCaso = item["observaciones"].ToString();
            evento.AccionesCaso = item["acciones"].ToString();
            evento.TipoInstalacion = item["TIPOINSTALACION"].ToString();
            evento.PersonaRecepcion = item["personarecepcion"].ToString();
            ReportesGuiasProductos producto = new ReportesGuiasProductos();
            List<ReportesGuiasProductos> listadoProductos = new List<ReportesGuiasProductos>();
            producto.Sku = item["codigoarticulo"].ToString();
            producto.Descripcion = item["descripcion"].ToString();
            producto.Cantidad = item["cantidad"].ToString();
            producto.Bodega = item["BODEGA"].ToString();
            listadoProductos.Add(producto);

            evento.Productos = listadoProductos;

            listado.Add(evento);
        }

        private dynamic ValidarSiExisteEventoEnListado(DataRow item, List<ReportesGuias> listado)
        {
            for (int i = 0; i < listado.Count; i++)
            {
                if (listado[i].IdEntrega == item["identrega"].ToString())
                {
                    return i;
                }
            }
            return null;
        }

        private void AgregarProductoAEventoExistente(DataRow item, int posicionEnListado, List<ReportesGuias> listado)
        {
            ReportesGuias evento = listado[posicionEnListado];

            ReportesGuiasProductos producto = new ReportesGuiasProductos();
            List<ReportesGuiasProductos> listadoProductos = evento.Productos;
            producto.Sku = item["codigoarticulo"].ToString();
            producto.Descripcion = item["descripcion"].ToString();
            producto.Cantidad = item["cantidad"].ToString();
            producto.Bodega = item["Bodega"].ToString();
            listadoProductos.Add(producto);

            evento.Productos = listadoProductos;
        }

        public List<ReportesEventosEntregas> GetEventosParaEntregas(string bodegaId, string fecha)
        {
            dB = new conexionDB();
            List<ReportesEventosEntregas> listadoDeEventos = new List<ReportesEventosEntregas>();
            try
            {
                string query = string.Empty;
                query = string.Format(@"SELECT no_cia, no_arti, sum(articulo.inmediatas(no_cia,no_arti, bodega)) total_inmediatas 
                                                        FROM ARINMA 
                                                        WHERE no_cia = '01' 
                                                        AND no_arti in (SELECT CODIGOARTICULO FROM T_DET_ENTREGAS T4
                                                        INNER JOIN T_ENC_ENTREGAS T5 ON T5.ID = T4.IDENTREGA
                                                        WHERE T5.FechaInicio >= to_timestamp('{0} 00:00:00', 'dd/MM/yyyy hh24:mi:ss') 
                                                        AND T5.FechaInicio <= to_timestamp('{0} 23:59:59', 'dd/MM/yyyy hh24:mi:ss')
                                                        GROUP BY CODIGOARTICULO) 
                                                        GROUP BY no_cia, no_arti;", fecha);
                resultadoArticulos = dB.ConsultarDB(query, "T_EVENTOS");
                switch (bodegaId)
                {
                    case "0000":
                        Controllers.ReporteDeBodegaController n = new Controllers.ReporteDeBodegaController();
                        string bodegas = string.Empty;
                       

                        foreach (var item in n.ListadoBodegas())
                        {
                            if (!item.Codigo.Equals("0000"))
                            {
                                //bodegas += "'" + item.Codigo + "',";

                                query = string.Format(@"SELECT T4.CODIGOEVENTO AS NO_TRANSA_MOV,T4.CANTIDAD,T0.BODEGA,T4.CODIGOARTICULO AS NO_ARTI,T3.NOMBRE_LARGO AS DESCRIPCION,TO_CHAR(T5.COMENTARIOSVENTAS) AS COMENTARIOSVENTAS,T8.DESCRIPCION AS VEHICULO FROM T_DET_ENTREGAS T4
                                                        INNER JOIN T_ENC_ENTREGAS T5 ON T5.ID = T4.IDENTREGA
                                                        INNER JOIN Naf47.Pvlineas_movimiento T0 ON T0.NO_ARTI = T4.CODIGOARTICULO AND T0.NO_TRANSA_MOV = T4.CODIGOEVENTO AND T0.CANTIDAD = T4.CANTIDAD AND T0.ENTREGADOMICILIO = 'D'
                                                        LEFT JOIN T_VEHICULOS T8 ON T8.ID = T5.VEHICULO
                                                        INNER JOIN naf47.arinda T3 ON T4.CODIGOARTICULO = T3.NO_ARTI
                                                        WHERE T5.FechaInicio >= to_timestamp('{0} 00:00:00', 'dd/MM/yyyy hh24:mi:ss') 
                                                        AND T5.FechaInicio <= to_timestamp('{0} 23:59:59', 'dd/MM/yyyy hh24:mi:ss')
                                                        AND T5.ESTADO = 'A'
                                                        AND T0.BODEGA = '{1}'
                                                        GROUP BY T4.CODIGOEVENTO,T4.CANTIDAD,T0.BODEGA,T4.CODIGOARTICULO,T3.NOMBRE_LARGO,TO_CHAR(T5.COMENTARIOSVENTAS),T8.DESCRIPCION;", fecha, item.Codigo);

                                var resultado2 = dB.ConsultarDB(query, "T_EVENTOS");

                               
                                int contador2 = resultado2.Tables[0].Rows.Count;
                                while (contador2 > 0)
                                {
                                    resultado2 = AgregarEventosRecursivo(resultado2, ref listadoDeEventos);
                                    contador2 = resultado2.Tables[0].Rows.Count;
                                }
                            }

                        }
                       
                        break;
                    default:
                        query = string.Format(@"SELECT T4.CODIGOEVENTO AS NO_TRANSA_MOV,T4.CANTIDAD,T0.BODEGA,T4.CODIGOARTICULO AS NO_ARTI,T3.NOMBRE_LARGO AS DESCRIPCION,TO_CHAR(T5.COMENTARIOSVENTAS) AS COMENTARIOSVENTAS,T8.DESCRIPCION AS VEHICULO FROM T_DET_ENTREGAS T4
                                                        INNER JOIN T_ENC_ENTREGAS T5 ON T5.ID = T4.IDENTREGA
                                                        INNER JOIN Naf47.Pvlineas_movimiento T0 ON T0.NO_ARTI = T4.CODIGOARTICULO AND T0.NO_TRANSA_MOV = T4.CODIGOEVENTO AND T0.CANTIDAD = T4.CANTIDAD AND T0.ENTREGADOMICILIO = 'D'
                                                        LEFT JOIN T_VEHICULOS T8 ON T8.ID = T5.VEHICULO
                                                        INNER JOIN naf47.arinda T3 ON T4.CODIGOARTICULO = T3.NO_ARTI
                                                        WHERE T5.FechaInicio >= to_timestamp('{0} 00:00:00', 'dd/MM/yyyy hh24:mi:ss') 
                                                        AND T5.FechaInicio <= to_timestamp('{0} 23:59:59', 'dd/MM/yyyy hh24:mi:ss')
                                                        AND T5.ESTADO = 'A'
                                                        AND T0.BODEGA = '{1}'
                                                        GROUP BY T4.CODIGOEVENTO,T4.CANTIDAD,T0.BODEGA,T4.CODIGOARTICULO,T3.NOMBRE_LARGO,TO_CHAR(T5.COMENTARIOSVENTAS),T8.DESCRIPCION;", fecha, bodegaId);

                        var resultado = dB.ConsultarDB(query, "T_EVENTOS");

                        int contador = resultado.Tables[0].Rows.Count;
                        while (contador > 0)
                        {
                            resultado = AgregarEventosRecursivo(resultado, ref listadoDeEventos);
                            contador = resultado.Tables[0].Rows.Count;
                        }
                        break;
                }
                return listadoDeEventos;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        private DataSet AgregarEventosRecursivo(DataSet resultado, ref List<ReportesEventosEntregas> listadoDeEventos)
        {
            foreach (DataRow item in resultado.Tables[0].Rows)
            {
                string qr = string.Format("NO_TRANSA_MOV = '{0}' AND NO_ARTI = '{1}'", item["NO_TRANSA_MOV"].ToString(), item["NO_ARTI"].ToString());
                int Inmediatas = 0;
                DataRow results = resultado.Tables[0].Select(qr).First();

            
                if (listadoDeEventos.Count == 0)
                {
                    AgregarEventoNuevoEntregas(item, listadoDeEventos);

                }
                else
                {
                    var posicion = ValidarSiExisteEventoEnListadoEntregas(item, listadoDeEventos);
                    if (posicion != null)
                    {
                        AgregarProductoAEventoExistenteEntregas(item, posicion, listadoDeEventos);
                    }
                    else
                    {
                        AgregarEventoNuevoEntregas(item, listadoDeEventos);
                    }
                }
             
                resultado.Tables[0].Rows.Remove(results);
               
                break;
            }
            return resultado;

        }

        private void AgregarEventoNuevoEntregas(DataRow item, List<ReportesEventosEntregas> listado)
        {
            ReportesEventosEntregas reporte = new ReportesEventosEntregas();
            List<ProductosEventosEntregas> listadoProductos = new List<ProductosEventosEntregas>();
            reporte.Evento = item["NO_TRANSA_MOV"].ToString();
            char[] EspaciosBlanco = { '\r', '\n' };
            string observaciones = item["COMENTARIOSVENTAS"].ToString().TrimEnd(EspaciosBlanco);
            reporte.Observaciones = observaciones;
            reporte.Bodega = item["BODEGA"].ToString();

            reporte.Vendedor = null;


            string query = string.Format(@"SELECT NOMBRE_VENDEDOR FROM NAF47.V_EVENTOS_PENDIENTES WHERE EVENTO = '{0}';", reporte.Evento);
            var resultado = dB.ConsultarDB(query, "T_EVENTOS");
            foreach (DataRow item2 in resultado.Tables[0].Rows)
            {
                reporte.Vendedor = item2["NOMBRE_VENDEDOR"].ToString();
            }

            string qr = string.Format("NO_ARTI = '{0}'", item["NO_ARTI"].ToString());
           
            DataRow results = resultadoArticulos.Tables[0].Select(qr).First();

            ProductosEventosEntregas producto = new ProductosEventosEntregas
            {
                Sku = item["NO_ARTI"].ToString(),
                Descripcion = item["DESCRIPCION"].ToString(),
                Cantidad = item["CANTIDAD"].ToString(),
                Bodega = item["BODEGA"].ToString(),
                Vehiculo = item["VEHICULO"].ToString(),
                Inmediatas = results["total_inmediatas"].ToString()

            };
            listadoProductos.Add(producto);
            reporte.Productos = listadoProductos;

            listado.Add(reporte);
        }

        private dynamic ValidarSiExisteEventoEnListadoEntregas(DataRow item, List<ReportesEventosEntregas> listado)
        {
            for (int i = 0; i < listado.Count; i++)
            {
                if (listado[i].Evento == item["NO_TRANSA_MOV"].ToString())
                {
                    return i;
                }
            }
            return null;
        }

        private void AgregarProductoAEventoExistenteEntregas(DataRow item, int posicionEnListado, List<ReportesEventosEntregas> listado)
        {
            ReportesEventosEntregas evento = listado[posicionEnListado];
            List<ProductosEventosEntregas> listadoProductos = evento.Productos;
            string qr = string.Format("NO_ARTI = '{0}'", item["NO_ARTI"].ToString());

            DataRow results = resultadoArticulos.Tables[0].Select(qr).First();

            ProductosEventosEntregas producto = new ProductosEventosEntregas
            {
                Sku = item["NO_ARTI"].ToString(),
                Descripcion = item["DESCRIPCION"].ToString(),
                Cantidad = item["CANTIDAD"].ToString(),
                Bodega = item["BODEGA"].ToString(),
                Vehiculo = item["VEHICULO"].ToString(),
                Inmediatas = results["total_inmediatas"].ToString()

            };
         
            listadoProductos.Add(producto);
            evento.Productos = listadoProductos;
        }
    }
}
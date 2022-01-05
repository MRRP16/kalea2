using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;

namespace kalea2.Utilidades
{
    public class Roles : conexionDB
    {
        conexionDB dB;
        public List<Models.Roles> ListadoRoles()
        {
            dB = new conexionDB();

            List<Models.Roles> ListadoParametrosGenerales = null;
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_ROLES;", "T_PARAMETROS_GENERALES");

                ListadoParametrosGenerales = new List<Models.Roles>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Roles parametroGeneral = new Models.Roles();
                    parametroGeneral.Id = Convert.ToInt32(item["Id"].ToString());
                    parametroGeneral.Descripcion = item["Descripcion"].ToString();
                    parametroGeneral.Estado = item["Estado"].ToString();
                    ListadoParametrosGenerales.Add(parametroGeneral);
                }
                return ListadoParametrosGenerales;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Models.Roles ObtenerModeloRolCrear()
        {
            dB = new conexionDB();
            Models.Roles roles = new Models.Roles();
            List<Models.Rol_Permiso> ListadoPermisos = null;
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_PERMISOS;", "T_PARAMETROS_GENERALES");

                ListadoPermisos = new List<Models.Rol_Permiso>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Rol_Permiso rol_permiso = new Models.Rol_Permiso();
                    rol_permiso.Codigo_Permiso = Convert.ToInt32(item["Id"].ToString());
                    rol_permiso.Descripcion_Permiso = item["Descripcion"].ToString();
                    rol_permiso.Crear = false;
                    rol_permiso.Editar = false;
                    rol_permiso.Visualizar = false;
                    rol_permiso.Eliminar = false;
                    ListadoPermisos.Add(rol_permiso);
                }
                roles.rol_Permiso = ListadoPermisos;
                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string CrearRol(Models.Roles Rol)
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

                    string Query = "INSERT INTO T_ROLES(Descripcion,FechaCreacion,Estado) VALUES (?,?,?)";

                    commandInsertar.Parameters.AddWithValue("@Descripcion ", SqlDbType.VarChar).Value = Rol.Descripcion;
                    commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Today;
                    //commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.Int).Value = Convert.ToInt32(UsrCreacion);
                    commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";

                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();
                    transaction.Commit();

                    Query = string.Format("SELECT Id FROM T_ROLES WHERE Descripcion = '{0}';", Rol.Descripcion);
                    var ds = ConsultarDB(Query, "T_ROLES");
                    int IdRol = 0;
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        IdRol = Convert.ToInt32(item["Id"].ToString());
                    }
                    if (IdRol == 0)
                    {
                        throw new Exception("Erro: Ocurrio un error al tratar de obtener el codigo del nuevo Rol. ");
                    }

                    for (int i = 0; i < Rol.rol_Permiso.Count; i++)
                    {
                        Query = string.Format("INSERT INTO T_ROLES_PERMISOS(CodigoRol,CodigoPermiso,Crear,Editar,Eliminar,Visualizar)VALUES(?,?,?,?,?,?)");

                        transaction = myConnection.BeginTransaction();
                        commandInsertar = myConnection.CreateCommand();
                        commandInsertar.Transaction = transaction;
                        commandInsertar.Parameters.AddWithValue("@CodigoRol ", SqlDbType.Int).Value = IdRol;
                        commandInsertar.Parameters.AddWithValue("@CodigoPermiso ", SqlDbType.Int).Value = Rol.rol_Permiso[i].Codigo_Permiso;
                        switch (Rol.rol_Permiso[i].Crear)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Crear", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Crear", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                        switch (Rol.rol_Permiso[i].Editar)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Editar", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Editar", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                        switch (Rol.rol_Permiso[i].Visualizar)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Visualizar", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Visualizar", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                        switch (Rol.rol_Permiso[i].Eliminar)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Eliminar", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Eliminar", SqlDbType.VarChar).Value = "0";
                                break;
                        }

                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();
                        transaction.Commit();
                    }


                    //transaction.Commit();
                    return "Rol creado exitosamente";
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

        public Models.Roles ObtenerModeloRolEditar(int id)
        {
            dB = new conexionDB();
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_ROLES WHERE Id = '" + id + "';", "T_PARAMETROS_GENERALES");
                Models.Roles rol = new Models.Roles();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    rol.Id = Convert.ToInt32(item["Id"].ToString());
                    rol.Descripcion = item["Descripcion"].ToString();
                    rol.Estado = item["Estado"].ToString();
                }
                resultado = dB.ConsultarDB("SELECT T0.*,T1.Descripcion FROM T_ROLES_PERMISOS T0 LEFT JOIN T_PERMISOS T1 ON T0.CodigoPermiso = T1.Id WHERE CodigoRol = '" + id + "' ORDER BY T1.ID ASC;", "T_ROLES_PERMISOS");
                var resultado2 = dB.ConsultarDB("SELECT * FROM T_PERMISOS", "T_PARAMETROS_GENERALES");
                if (resultado2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in resultado2.Tables[0].Rows)
                    {
                        DataRow[] drow = resultado.Tables[0].Select("CODIGOPERMISO = '" + item["ID"].ToString() + "'");
                        if (drow.Length>0)
                        {
                            Models.Rol_Permiso rol_permiso = new Models.Rol_Permiso();
                            rol_permiso.Codigo_Permiso = Convert.ToInt32(drow[0]["CodigoPermiso"].ToString());
                            rol_permiso.Descripcion_Permiso = drow[0]["Descripcion"].ToString();

                            switch (drow[0]["Crear"].ToString())
                            {
                                case "1":
                                    rol_permiso.Crear = true;
                                    break;
                            }
                            switch (drow[0]["Editar"].ToString())
                            {
                                case "1":
                                    rol_permiso.Editar = true;
                                    break;
                            }
                            switch (drow[0]["Visualizar"].ToString())
                            {
                                case "1":
                                    rol_permiso.Visualizar = true;
                                    break;
                            }
                            switch (drow[0]["Eliminar"].ToString())
                            {
                                case "1":
                                    rol_permiso.Eliminar = true;
                                    break;
                            }
                            rol_permiso.Codigo_rol = id;
                            rol.rol_Permiso.Add(rol_permiso);
                        }
                        else
                        {
                            Models.Rol_Permiso rol_permiso = new Models.Rol_Permiso();
                            rol_permiso.Codigo_Permiso = Convert.ToInt32(item["ID"].ToString());
                            rol_permiso.Descripcion_Permiso = item["Descripcion"].ToString();
                            rol_permiso.Codigo_rol = id;
                            rol.rol_Permiso.Add(rol_permiso);
                        }
                    }
                }
                else
                {
                    rol.rol_Permiso = ObtenerModeloRolCrear().rol_Permiso;
                }
                rol.rol_Permiso.OrderBy(x => x.Codigo_Permiso);
                return rol;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string ActualizarRol(int id, Models.Roles rol)
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

                    string Query = string.Format("UPDATE T_ROLES SET Descripcion ='{0}' WHERE Id = '{1}'",
                        rol.Descripcion, id);
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    Query = string.Format("DELETE FROM T_ROLES_PERMISOS  WHERE CodigoRol = '{0}'", id);
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();
                    transaction.Commit();

                    for (int i = 0; i < rol.rol_Permiso.Count; i++)
                    {
                        Query = string.Format("INSERT INTO T_ROLES_PERMISOS(CodigoRol,CodigoPermiso,Crear,Editar,Eliminar,Visualizar)VALUES(?,?,?,?,?,?)");
                        commandInsertar.Dispose();
                        transaction = myConnection.BeginTransaction();
                        commandInsertar = myConnection.CreateCommand();
                        commandInsertar.Transaction = transaction;
                        commandInsertar.Parameters.AddWithValue("@CodigoRol ", SqlDbType.Int).Value = id;
                        commandInsertar.Parameters.AddWithValue("@CodigoPermiso ", SqlDbType.Int).Value = rol.rol_Permiso[i].Codigo_Permiso;
                        switch (rol.rol_Permiso[i].Crear)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Crear", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Crear", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                        switch (rol.rol_Permiso[i].Editar)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Editar", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Editar", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                        switch (rol.rol_Permiso[i].Eliminar)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Eliminar", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Eliminar", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                        switch (rol.rol_Permiso[i].Visualizar)
                        {
                            case true:
                                commandInsertar.Parameters.AddWithValue("@Visualizar", SqlDbType.VarChar).Value = "1";
                                break;
                            default:
                                commandInsertar.Parameters.AddWithValue("@Visualizar", SqlDbType.VarChar).Value = "0";
                                break;
                        }
                      

                        commandInsertar.CommandText = Query;
                        commandInsertar.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    return "Solicitud actualizada exitosamente";
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


                    var resultado = dB.ConsultarDB("SELECT Estado FROM T_ROLES WHERE Id = '" + id + "';", "T_VEHICULOS");

                    string estado = "A";
                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {
                        estado = item["Estado"].ToString();
                    }
                    string Query = string.Empty;
                    switch (estado)
                    {
                        case "A":
                            Query = string.Format("UPDATE T_ROLES SET Estado = 'I' WHERE Id = '{0}'", id);
                            break;
                        default:
                            Query = string.Format("UPDATE T_ROLES SET Estado = 'A' WHERE Id = '{0}'", id);
                            break;
                    }
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Rol actualizado exitosamente";
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
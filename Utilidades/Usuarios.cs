using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kalea2.Utilidades
{
    public class Usuarios : conexionDB
    {

        conexionDB dB;
        public List<Models.Usuarios> ListadoDeUsuarios()
        {
            dB = new conexionDB();

            List<Models.Usuarios> ListadoUsuarios = null;
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_USUARIOS;", "T_USUARIOS");

                ListadoUsuarios = new List<Models.Usuarios>();

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    Models.Usuarios usuario = new Models.Usuarios();
                    usuario.Codigo = Convert.ToInt32(item["Id"].ToString());
                    usuario.Nombre = item["Nombre"].ToString();
                    usuario.Apellido = item["Apellido"].ToString();
                    usuario.NombreUsuario = item["NombreUsuario"].ToString();
                    usuario.Estado = item["Estado"].ToString();
                    ListadoUsuarios.Add(usuario);
                }

                return ListadoUsuarios;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool GetPermisos(string CodigoUsuario, string Permiso, string Accion)
        {
            try
            {
                switch (CodigoUsuario)
                {
                    case "-1":
                        return true;
                    default:
                        string Query = "SELECT COUNT(*) FROM T_USUARIO\" T0" +
                                " INNER JOIN T_ROL_PERMISO\" T1 ON T0.CodigoRol = T1.CodigoRol" +
                                " WHERE T1.CodigoPermiso = '" + Permiso + "'" +
                                " AND T0.Codigo = '" + CodigoUsuario + "'";

                        DataSet codigo = ConsultarDB(Query, "T_USUARIO");

                        if (codigo.Tables["T_USUARIO"].Rows.Count > 0)
                        {
                            foreach (DataRow item in codigo.Tables["T_USUARIO"].Rows)
                            {
                                switch (item[Accion].ToString())
                                {
                                    case "S":
                                        return true;
                                    case "N":
                                        return false;
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public List<SelectListItem> GetRolesCreate()
        {
            dB = new conexionDB();

            List<SelectListItem> roles = new List<SelectListItem>();
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_ROLES WHERE Estado = 'A';", "T_ROLES");


                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    roles.Add(new SelectListItem { Text = item["Descripcion"].ToString(), Value = item["Id"].ToString() });
                }

                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<SelectListItem> GetUsuarios()
        {
            dB = new conexionDB();

            List<SelectListItem> roles = new List<SelectListItem>();
            try
            {
                var resultado = dB.ConsultarDB("select USUARIO from NAF47.tasgroleuser where ID_ROLE = '65' group by USUARIO", "T_ROLES");


                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    roles.Add(new SelectListItem { Text = item["USUARIO"].ToString(), Value = item["USUARIO"].ToString() });
                }

                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string CrearUsuario(Models.Usuarios usuario)
        {
            dB = new conexionDB();
            Seguridad seguridad = new Seguridad();

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

                    string Query = "INSERT INTO T_USUARIOS(Nombre,Apellido,NombreUsuario,Contraseña,IdRol,FechaCreacion,Estado) VALUES (?,?,?,?,?,?,?)";

                    commandInsertar.Parameters.AddWithValue("@Nombre ", SqlDbType.VarChar).Value = usuario.Nombre;
                    commandInsertar.Parameters.AddWithValue("@Apellido", SqlDbType.VarChar).Value = usuario.Apellido;
                    commandInsertar.Parameters.AddWithValue("@NombreUsuario", SqlDbType.VarChar).Value = usuario.NombreUsuario;
                    commandInsertar.Parameters.AddWithValue("@Contraseña", SqlDbType.VarChar).Value = "";
                    commandInsertar.Parameters.AddWithValue("@IdRol", SqlDbType.Int).Value = Convert.ToInt32( usuario.Rol);
                    commandInsertar.Parameters.AddWithValue("@FechaCreacion", SqlDbType.Date).Value = DateTime.Today;
                    //commandInsertar.Parameters.AddWithValue("@UsrCreacion", SqlDbType.Int).Value = Convert.ToInt32(UsrCreacion);
                    commandInsertar.Parameters.AddWithValue("@Estado", SqlDbType.VarChar).Value = "A";

                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();

                    transaction.Commit();
                    return "Usuario creado exitosamente";
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

        public Models.Usuarios ObtenerUsuario(int id)
        {
            dB = new conexionDB();
            Seguridad seg = new Seguridad();
            Models.Usuarios usuario = new Models.Usuarios();
            try
            {
                var resultado = dB.ConsultarDB("SELECT * FROM T_USUARIOS WHERE Id = '" + id + "';", "T_usuarioS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    usuario.Codigo = Convert.ToInt32(item["Id"].ToString());
                    usuario.Nombre = item["Nombre"].ToString();
                    usuario.Apellido = item["Apellido"].ToString();
                    usuario.NombreUsuario = item["NombreUsuario"].ToString();
                    //usuario.Contrasenia = seg.Desencripta(item["Contraseña"].ToString());
                    usuario.Rol = item["IdRol"].ToString();
                    usuario.Estado = item["Estado"].ToString();
                    usuario.Roles = GetRolesCreate();
                    usuario._Usuarios = GetUsuarios();
                }

                return usuario;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string ActualizarUsuario(int id, Models.Usuarios usuario)
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

                    string Query = string.Format("UPDATE T_USUARIOS SET Nombre ='{0}',Apellido='{1}',NombreUsuario = '{2}',IdRol = '{3}' WHERE Id = '{4}'",
                        usuario.Nombre, usuario.Apellido, usuario.NombreUsuario, usuario.Rol,id);
                    commandInsertar.CommandText = Query;
                    commandInsertar.ExecuteNonQuery();
                    transaction.Commit();

                    if (!string.IsNullOrEmpty(usuario.Contrasenia))
                    {
                        commandInsertar = myConnection.CreateCommand();

                        transaction = myConnection.BeginTransaction();

                        commandInsertar.Connection = myConnection;
                        commandInsertar.Transaction = transaction;

                        Seguridad s = new Seguridad();

                        //Query = string.Format("UPDATE T_USUARIOS SET Contraseña ='{0}' WHERE Id = '{1}'",s.Encripta(usuario.Contrasenia), id);
                        //commandInsertar.CommandText = Query;
                        //commandInsertar.ExecuteNonQuery();
                        transaction.Commit();

                    }


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


                    var resultado = dB.ConsultarDB("SELECT Estado FROM T_USUARIOS WHERE Id = '" + id + "';", "T_usuarioS");

                    string estado = "A";
                    foreach (DataRow item in resultado.Tables[0].Rows)
                    {
                        estado = item["Estado"].ToString();
                    }
                    string Query = string.Empty;
                    switch (estado)
                    {
                        case "A":
                            Query = string.Format("UPDATE T_USUARIOS SET Estado = 'I' WHERE Id = '{0}'", id);
                            break;
                        default:
                            Query = string.Format("UPDATE T_USUARIOS SET Estado = 'A' WHERE Id = '{0}'", id);
                            break;
                    }
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

        public Models.Usuarios VerificaLogin(string Usuario)
        {
            dB = new conexionDB();
            try
            {
                Seguridad seg = new Seguridad();
                Models.Usuarios usuario = new Models.Usuarios();
                var resultado = dB.ConsultarDB("SELECT * FROM T_USUARIOS WHERE NombreUsuario = '" + Usuario + "';", "T_usuarioS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    usuario.Codigo = Convert.ToInt32(item["Id"].ToString());
                    usuario.Nombre = item["Nombre"].ToString();
                    usuario.Apellido = item["Apellido"].ToString();
                    usuario.NombreUsuario = item["NombreUsuario"].ToString();
                    //usuario.Contrasenia = seg.Desencripta(item["Contraseña"].ToString());
                    usuario.Rol = item["IdRol"].ToString();
                    usuario.Estado = item["Estado"].ToString();
                    usuario.Roles = GetRolesCreate();
                }

                return usuario;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Models.Usuarios VerificarLoginkalea2(string Usuario)
        {
            dB = new conexionDB();
            try
            {
                Seguridad seg = new Seguridad();
                Models.Usuarios usuario = new Models.Usuarios();
                var resultado = dB.ConsultarDB("SELECT * FROM naf47.pvusuarios WHERE COD_USUARIO = '" + Usuario + "';", "T_usuarioS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    usuario.Codigo = Convert.ToInt32(item["COD_USUARIO"].ToString());
                    usuario.Nombre = item["NOM_USUARIO"].ToString();
                    usuario.Apellido = item["APE_USAURIO"].ToString();
                    usuario.NombreUsuario = item["NOMBRE"].ToString();
                    //usuario.Contrasenia = seg.Desencripta(item["Contraseña"].ToString());
                    usuario.Rol = item["ROL"].ToString();
                    //usuario.Estado = item["Estado"].ToString();
                    //usuario.Roles = GetRolesCreate();
                }

                return usuario;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Models.Usuarios VerificarLoginEnSek(string Usuario)
        {
            dB = new conexionDB();
            try
            {
                Seguridad seg = new Seguridad();
                Models.Usuarios usuario = new Models.Usuarios();
                var resultado = dB.ConsultarDB("SELECT * FROM T_USUARIOS WHERE NombreUsuario = '" + Usuario + "';", "T_usuarioS");

                foreach (DataRow item in resultado.Tables[0].Rows)
                {
                    usuario.Codigo = Convert.ToInt32(item["Id"].ToString());
                    usuario.Nombre = item["Nombre"].ToString();
                    usuario.Apellido = item["Apellido"].ToString();
                    usuario.NombreUsuario = item["NombreUsuario"].ToString();
                    usuario.Contrasenia = seg.Desencripta(item["Contraseña"].ToString());
                    usuario.Rol = item["IdRol"].ToString();
                    usuario.Estado = item["Estado"].ToString();
                    usuario.Roles = GetRolesCreate();
                }

                return usuario;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
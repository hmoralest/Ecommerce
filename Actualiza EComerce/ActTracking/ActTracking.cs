using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using LogProcesos;
using System.IO;

namespace ActTrackings
{
    public class ActTracking
    {
        SqlConnection sql;
        Conexion oConexion = new Conexion();

        MySqlConnection mysql;
        Conexion oConexionMySql = new Conexion();
        

        public string[] ActualizaTrackin(string orden, string tracking)//0=error 1=ok
        {
            string[] ejecuto;
            string result = "";

            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    //abrir la conexion
                    mysql = oConexionMySql.getConexionMySQL();
                    mysql.Open();

                    // setear parametros del command
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = mysql;
                    cmd.CommandText = "USP_ActualizaTracking";

                    //asignar paramentros
                    cmd.Parameters.AddWithValue("ref_order", orden);
                    cmd.Parameters.AddWithValue("tracking", tracking);

                    //ejecutar el query
                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        result = Convert.ToString(dr["exe"]);
                    }
                    mysql.Close();
                }
                catch (Exception ex)
                {
                    mysql.Close();
                    ejecuto = new string[] { "0", ex.Message };
                    return ejecuto;
                } // end try
            } // end using

            // Evalua el resultado obtenido
            if (result == "1")
            {
                ejecuto = new string[] { result, "Ejecución OK." };
            }
            else
            {
                ejecuto = new string[] { result, "Error al ejecutar SP." };
            }
            return ejecuto;
        }

        public static void Main()
        {
            ActTracking exe = new ActTracking();
            exe.ActualizaTrackin("XKBKNABJK","123123");
        }
    }
}
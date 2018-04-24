using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace ActDescuentos
{
    class Conexion
    {
        public SqlConnection getConexionLog()
        {
            SqlConnection sqllog = new SqlConnection("Data Source=ecommerce.bgr.pe;Initial Catalog=BD_ECOMMERCE;Integrated Security=False;User ID=ecommerce;Password=Bata2018.*@=?++;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            return sqllog;
        }

        /// <summary>
        /// método que obtiene la Conexión para Obtener datos de Descuentos
        /// </summary>
        /// <returns>SQL Server Conexión - obtener Descuentos</returns>
        public SqlConnection getConexionSQL()
        {
            SqlConnection sql;
            DataTable result = new DataTable();

            using (SqlConnection con = getConexionLog())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("USP_Obtiene_DatosConexion", con);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@Id", SqlDbType.VarChar).Value = "04";

                    da.Fill(result);
                    con.Close();

                    sql = new SqlConnection("Data Source=" + result.Rows[0]["Url"].ToString() + ";Initial Catalog=" + result.Rows[0]["BaseDatos"].ToString() + ";User ID=" + result.Rows[0]["Usuario"].ToString() + ";Password=" + result.Rows[0]["Contrasena"].ToString() + ";TrustServerCertificate=" + result.Rows[0]["Trusted_Conection"].ToString() + "");

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return sql;
        }

        /// <summary>
        /// método que obtiene la Conexión para PrestaShop
        /// </summary>
        /// <returns>MySQL Conexion - PrestaShop</returns>
        public MySqlConnection getConexionMySQL()
        {
            MySqlConnection mysql;
            DataTable result = new DataTable();

            using (SqlConnection con = getConexionLog())//Conexión Principal
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("USP_Obtiene_DatosConexion", con);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@Id", SqlDbType.VarChar).Value = "01";

                    da.Fill(result);
                    con.Close();

                    mysql = new MySqlConnection("Server=" + result.Rows[0]["Url"].ToString() + ";Database=" + result.Rows[0]["BaseDatos"].ToString() + ";Uid=" + result.Rows[0]["Usuario"].ToString() + ";Password=" + result.Rows[0]["Contrasena"].ToString() + "");

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return mysql;
        }
    }
}

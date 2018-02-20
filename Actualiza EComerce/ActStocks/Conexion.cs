using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace ActStocks
{ 

    class Conexion
    {
        public SqlConnection getConexionLog()
        {
            SqlConnection sqllog = new SqlConnection(ConfigurationManager.ConnectionStrings["sqllog"].ConnectionString);
            return sqllog;
        }
        public SqlConnection getConexionSQL()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
            return sql;
        }
        public MySqlConnection getConexionMySQL()
        {
            MySqlConnection mysql = new MySqlConnection(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
            return mysql;
        }
    }
}

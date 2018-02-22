using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace LeerPedido
{
    public class Conexion
    {
        public MySqlConnection getConexionMySQL()
        {
            MySqlConnection mysql = new MySqlConnection(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
            return mysql;
        }
    }
}

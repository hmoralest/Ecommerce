using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace LeerVenta208
{ 

    class Conexion
    {
        public SqlConnection getConexionSQL()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["sql_ori"].ConnectionString);
            return sql;
        }
        public SqlConnection getConexionSQL208()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["sql_des"].ConnectionString);
            return sql;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace LeerVentas
{ 

    class Conexion
    {
        public SqlConnection getConexionSQL()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
            return sql;
        }
    }
}

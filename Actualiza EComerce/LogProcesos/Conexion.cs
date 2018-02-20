using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace LogProcesos
{
    public class Conexion
    {
        public SqlConnection getConexionLog()
        {
            SqlConnection sqllog = new SqlConnection(ConfigurationManager.ConnectionStrings["sqllog"].ConnectionString);
            return sqllog;
        }
    }
}

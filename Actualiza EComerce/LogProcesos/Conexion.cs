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
        /// <summary>
        /// método que obtiene la Conexión para Obtener datos de Descuentos
        /// </summary>
        /// <returns>SQL Server Conexión - obtener Descuentos</returns>
        public SqlConnection getConexionLog()
        {
            SqlConnection sqllog = new SqlConnection("Data Source=ecommerce.bgr.pe;Initial Catalog=BD_ECOMMERCE;Integrated Security=False;User ID=ecommerce;Password=Bata2018.*@=?++;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            return sqllog;
        }
    }
}

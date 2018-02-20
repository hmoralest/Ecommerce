using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LogProcesos
{
    public class LogProceso
    {
        SqlConnection sqllog;
        Conexion oConexion = new Conexion();

        public void CreaLogProceso(string proceso)
        {
            sqllog = oConexion.getConexionLog();
            sqllog.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "ECOM_CREALOGPROCESO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqllog;

            cmd.Parameters.Add("@Proceso", SqlDbType.VarChar).Value = proceso;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizaLogProceso(string proceso, int estado)
        {
            sqllog = oConexion.getConexionLog();
            sqllog.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "ECOM_ACTLOGPROCESO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqllog;

            cmd.Parameters.Add("@proceso", SqlDbType.VarChar).Value = proceso;
            cmd.Parameters.Add("@estado", SqlDbType.Int).Value = estado;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ValidaProceso(string proceso)
        {
            int estado = 0;

            sqllog = oConexion.getConexionLog();
            SqlDataAdapter da = new SqlDataAdapter("ECOM_VALIDAPROCESO", sqllog);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@proceso", SqlDbType.VarChar).Value = proceso;
            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            foreach (DataRow row in dt.Rows)
            {
                estado = Convert.ToInt32(row["estado"]);
            }

            return estado;
        }
        
    }
}

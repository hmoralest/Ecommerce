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

namespace ActStocks
{
    public class ActStock
    {
        SqlConnection sql;
        Conexion oConexion = new Conexion();

        MySqlConnection mysql;
        Conexion oConexionMySql = new Conexion();

        string tienda = "";
        string error = "";
        int cant_reg = 0;

        public static StreamWriter sw;
        string archivo = "";
        private static bool escribe_archivo = true;

        /// <summary>
        /// Metodo que efectúa el llenado de todas las variables globales
        /// </summary>
        private void Llenar_datos()
        {
            tienda = Obten_DatoGeneral("cod_alma");
        }
        /// <summary>
        /// metodo que obtiene los datos Genéricos usados
        /// </summary>
        /// <param name="codigo">codigo que referencia los datos genéricos almacenados en SQL</param>
        /// <returns>dato obtenido desde la BD E_COMMERCE</returns>
        public string Obten_DatoGeneral(string codigo)
        {
            string rtpa = "";
            DataTable dt = new DataTable();
            using (sql = oConexion.getConexionSQL())
            {
                try
                {
                    string query = "SELECT dbo.UFN_Obtiene_DatosGenerales('" + codigo + "') As dato;";

                    SqlCommand cmd = new SqlCommand(query, sql);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(dt);

                    rtpa = dt.Rows[0]["dato"].ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return rtpa;
        }

        public DataTable ListaStocks(string tienda)
        {
            SqlDataAdapter da = new SqlDataAdapter("USP_ECOM_LISTASTOCK", sql);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public void ActualizaOrigen(string tienda, string mov_id, string detmov_id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "USP_ECOM_ACTSTOCK";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sql;

            cmd.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
            cmd.Parameters.Add("@mov_id", SqlDbType.VarChar).Value = mov_id;
            cmd.Parameters.Add("@det_mov_id", SqlDbType.VarChar).Value = detmov_id;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ControlaTrans(int estado)
        {
            try
            {
                if (estado == 0)    // INICIA
                {
                    SqlCommand tranSQL = new SqlCommand("BEGIN TRAN STOCK;", sql);
                    tranSQL.ExecuteNonQuery();

                    MySqlCommand tranMySQL = mysql.CreateCommand();
                    tranMySQL.CommandText = "BEGIN;";
                    tranMySQL.ExecuteNonQuery();
                }
                if (estado == 1)    // FINALIZA OK 
                {
                    SqlCommand tranSQL = new SqlCommand("COMMIT TRAN STOCK;", sql);
                    tranSQL.ExecuteNonQuery();

                    MySqlCommand tranMySQL = mysql.CreateCommand();
                    tranMySQL.CommandText = "COMMIT;";
                    tranMySQL.ExecuteNonQuery();
                }
                if (estado == 2)    // ERROR ENCONTRADO
                {
                    SqlCommand tranSQL = new SqlCommand("ROLLBACK TRAN STOCK;", sql);
                    tranSQL.ExecuteNonQuery();

                    MySqlCommand tranMySQL = mysql.CreateCommand();
                    tranMySQL.CommandText = "ROLLBACK;";
                    tranMySQL.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método para crear archivo que se usará como log
        /// </summary>
        public void CrearArchivoLog()
        {
            //datos para archivo LOG
            string path = Environment.CurrentDirectory;
            string nombre = "Log Errores";
            string nom_archiv = DateTime.Today.ToString("yyyy.MM.dd") + " Log Actualiza Stock";
            //crea directorio si no existe
            if (!Directory.Exists(path + "\\" + nombre))
            {   //Crea el directorio
                DirectoryInfo di = Directory.CreateDirectory(path + "\\" + nombre);
            }
            //Crea el Writer
            sw = File.AppendText(path + "\\" + nombre + "\\" + nom_archiv + ".txt");
            archivo = path + "\\" + nombre + "\\" + nom_archiv + ".txt";
        }

        public void ActualizaStocks(DataTable precios)
        {
            //Valida con Productos Existentes
            string queryvalida = "Select Distinct reference From ps_product_attribute";
            MySqlCommand cmd = new MySqlCommand(queryvalida, mysql);
            MySqlDataAdapter returnVal = new MySqlDataAdapter(queryvalida, mysql);
            DataTable Productos = new DataTable();
            try
            {
                returnVal.Fill(Productos);
            }
            catch (Exception ex)
            {
                error = "No se pudo obtener datos de productos de Prestashop.";
                throw ex;
            }

            //Crea Datatable con datos Finales
            DataTable Final = new DataTable();
            Final = precios.Clone();
            Final.Clear();

            //Validacion
            string val;
            foreach (DataRow row1 in precios.Rows)
            {
                val = "no";
                foreach (DataRow row2 in Productos.Rows)
                {
                    if (row1["product_id"].ToString() == row2["reference"].ToString())
                    {
                        val = "si";
                        Final.ImportRow(row1);
                        Productos.Rows.Remove(row2);
                        break;
                    }
                }
                if (val == "no")
                {
                    error = "No se encontró producto " + row1["product_id"] + " en Prestashop.";
                    if (escribe_archivo)
                    {
                        sw.WriteLine(error);
                    }
                    //throw new System.ArgumentException("Código de Producto Inválido", "reference");
                }
            }

            //Elimino datos anteriores
            MySqlCommand comdel = mysql.CreateCommand();
            comdel.CommandText = "Delete From ps_erp;";
            try
            {
                comdel.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                error = "No se pudo limpiar la tabla ps_erp";
                throw ex;
            }

            string id_mov = "";
            //recorro datatable final
            foreach (DataRow row in Final.Rows)
            {
                id_mov = row["mov_id"].ToString();
                //Actualizar en Prestashop
                MySqlCommand comm = mysql.CreateCommand();
                comm.CommandText = "Insert into ps_erp (ref_product, stock) values ('" + row["product_id"] + "'," + row["cantidad"] + ");";

                try
                {
                    //ejecucion
                    comm.ExecuteNonQuery();
                    //Actualiza estado para no repetir
                    ActualizaOrigen(tienda, row["mov_id"].ToString(), row["det_mov_id"].ToString());
                }
                catch (Exception ex)
                {
                    error = "No se pudo insertar datos en la tabla ps_erp.";
                    throw ex;
                }
                cant_reg = cant_reg + 1;
            }
            
        }

        public string EjecutaStock()
        {
            // Se debe comentar para integrar
            // Creación de Archivo
            try
            {
                CrearArchivoLog();
                sw.WriteLine("************ Inicio Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
            }
            catch
            {
                escribe_archivo = false;
            }

            try
            {
                sql = oConexion.getConexionSQL();
                sql.Open();

                mysql = oConexionMySql.getConexionMySQL();
                mysql.Open();

                ControlaTrans(0);
            }
            catch(Exception ex)
            {
                if (escribe_archivo)
                {
                    sw.WriteLine("No se pudo abrir Transaccion. // " + ex.Message);
                    sw.Close();
                }
                return "No se pudo abrir Transaccion. // " + ex.Message;
            }

            // Obtener Datos desde SQL
            try
            {
                Llenar_datos();
            }
            catch (Exception ex)
            {
                if (escribe_archivo)
                {
                    sw.WriteLine("Error en Obtener Datos Generales en SQL.// " + ex.Message);
                    sw.WriteLine(ex.Message);
                    sw.Close();
                }
                return "Error en Obtener Datos Generales en SQL. // " + ex.Message;
            }

            try
            {
                DataTable tabla = new DataTable();

                tabla = ListaStocks(tienda);

                ActualizaStocks(tabla);
            }
            catch (Exception ex)
            {
                ControlaTrans(2);
                sql.Close();
                mysql.Close();
                if (escribe_archivo)
                {
                    sw.WriteLine("Error: " + error + " // " + ex.Message);
                    sw.WriteLine("Se actualizaron " + cant_reg.ToString() + " registros.");
                    sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
                    sw.Close();
                }
                return "Error: " + error + " // " + ex.Message;
            }
            ControlaTrans(1);
            sql.Close();
            mysql.Close();
            if (escribe_archivo)
            {
                sw.WriteLine("Se actualizaron " + cant_reg.ToString() + " registros.");
                sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
                sw.Close();
            }
            return "";
        }

        public static void Main()
        {
            ActStock exe = new ActStock();
            exe.EjecutaStock();
        }


    }
}

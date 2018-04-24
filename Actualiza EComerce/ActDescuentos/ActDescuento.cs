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

namespace ActDescuentos
{
    public class ActDescuento
    {
        SqlConnection sql;
        Conexion oConexion = new Conexion();

        MySqlConnection mysql;
        Conexion oConexionMySql = new Conexion();
        
        LogProceso log = new LogProceso();

        string proceso = "ActDescuentos";
        string archivo = "";
        string tienda = ""; // cambio de 005 a 095 - 20/04/2018
        public static StreamWriter sw;

        private static bool escribe_archivo = true;
        private static bool escribe_log = true;

        public DataTable ListaDescuentos(string tienda)
        {
            sql = oConexion.getConexionSQL();
            SqlDataAdapter da = new SqlDataAdapter("USP_ECOM_LISTADESCUENTOS", sql);
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
        /// <summary>
        /// Metodo que efectúa el llenado de todas las variables globales
        /// </summary>
        private void Llenar_datos()
        {
            tienda = Obten_DatoGeneral("pref_tda") + "" + Obten_DatoGeneral("cod_tienda");
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

        public void CrearArchivoLog()
        {
            //datos para archivo LOG
            string path = Environment.CurrentDirectory;
            string nombre = "Log Errores";
            string nom_archiv = DateTime.Today.ToString("yyyy.MM.dd") + " Log Actualiza Descuentos";
            //crea directorio si no existe
            if (!Directory.Exists(path + "\\" + nombre))
            {   //Crea el directorio
                DirectoryInfo di = Directory.CreateDirectory(path + "\\" + nombre);
            }
            //Crea el Writer
            sw = File.AppendText(path + "\\" + nombre + "\\" + nom_archiv + ".txt");
            archivo = path + "\\" + nombre + "\\" + nom_archiv + ".txt";
        }

        public void ActualizaDescuentos(DataTable descuentos)
        {
            int valida = 0;
            if (escribe_log)
            {
                log.ValidaProceso(proceso);
            }
            if (valida != 1)
            {
                //datos generales
                int estado = 1;
                mysql = oConexionMySql.getConexionMySQL();
                mysql.Open();
                //Actualiza estado a proceso abierto
                if (escribe_log)
                {
                    log.ActualizaLogProceso(proceso, -1);
                }

                /*//datos para archivo LOG
                int contador = 1;
                string path = Environment.CurrentDirectory;
                string nombre = "Log Errores";
                string archivo = DateTime.Today.ToString("yyyy.MM.dd")+ " Log Error Descuentos - ";
                //crea directorio si no existe
                if (!Directory.Exists(path+"\\"+nombre))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path + "\\" + nombre);
                }
                //correlativo de archivo de ejecución en el día
                while(File.Exists(path + "\\" + nombre+"\\"+archivo + contador.ToString() + ".txt"))
                {
                    contador = contador + 1;
                }
                //Crea el Writer
                StreamWriter sw = new StreamWriter(path + "\\" + nombre + "\\" + archivo + contador.ToString() + ".txt");*/

                //Valida con Productos Existentes
                string queryvalida = "Select Distinct replace(reference,'-','') as id_product  From ps_product;";
                MySqlCommand cmd = new MySqlCommand(queryvalida, mysql);
                MySqlDataAdapter returnVal = new MySqlDataAdapter(queryvalida, mysql);
                DataTable Productos = new DataTable();
                try
                {
                    returnVal.Fill(Productos);
                }
                catch (Exception ex)
                {
                    estado = 0;
                    if (escribe_log) { log.ActualizaLogProceso(proceso, estado); }
                    if (escribe_archivo)
                    {
                        sw.WriteLine(ex.Message);
                        sw.WriteLine("Error en lectura de productos en Prestashop.");
                        sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
                    }
                    return;
                }

                //Crea Datatable con datos Finales
                DataTable Final = new DataTable();
                Final = descuentos.Clone();
                Final.Clear();

                //Validacion
                //string val;
                foreach (DataRow row1 in descuentos.Rows)
                {
                    //val = "no";
                    foreach (DataRow row2 in Productos.Rows)
                    {
                        if(row1["product_id"].ToString()== row2["id_product"].ToString())
                        {
                            //val = "si";
                            Final.ImportRow(row1);
                            Productos.Rows.Remove(row2);
                            break;
                        }
                    }
                    /*if (val == "no")
                    {
                        estado = 0;
                        sw.WriteLine("No se encontró el producto " + row1["product_id"] + " en Prestashop.");
                    }*/
                }

                //Elimino datos anteriores
                MySqlCommand comdel = mysql.CreateCommand();
                comdel.CommandText = "Delete From ps_specific_price;";
                try
                {
                    comdel.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    estado = 0;
                    if (escribe_log) { log.ActualizaLogProceso(proceso, estado); }
                    if (escribe_archivo)
                    {
                        sw.WriteLine(ex.Message);
                        sw.WriteLine("Error en limpiar tabla de descuentos en Prestashop.");
                        sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
                    }
                    return;
                }

                int cant_reg = 0;
                //recorro datatable final
                foreach (DataRow row in Final.Rows)
                {
                    string cadena;
                    cant_reg = cant_reg + 1;
                    //Actualizar en Prestashop
                    MySqlCommand comm = mysql.CreateCommand();
                    cadena =            "Insert into ps_specific_price ";
                    cadena = cadena +   "Select " + cant_reg.ToString() + ",0,0,id_product,1,0,0,0,0,0,0,-1,1," + row["Monto"].ToString() + ",1,'amount','" + row["Fecha_Ini"].ToString() + "','" + row["Fecha_Fin"].ToString() + "' ";
                    cadena = cadena +   "From ps_product Where replace(reference,'-', '') = '" + row["product_id"].ToString() + "'";
                    comm.CommandText = cadena;
                    try
                    {
                        //ejecucion
                        comm.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        estado = 0;
                        if (escribe_log) { log.ActualizaLogProceso(proceso, estado); }
                        if (escribe_archivo)
                        {
                            sw.WriteLine(ex.Message);
                            sw.WriteLine("Error en ingresar descuentos de producto " + row["product_id"].ToString() + " y descuento " + row["Monto"].ToString() + " en Prestashop.");
                        }
                    }
                }
                string termino_proceso;
                if (escribe_log) { log.ActualizaLogProceso(proceso, estado); }
                if (estado == 1)
                {
                    termino_proceso = "correctamente.";
                }
                else
                {
                    termino_proceso = "con errores.";
                }
                if (escribe_archivo)
                {
                    sw.WriteLine("Se actualizaron " + cant_reg.ToString() + " registros.");
                    sw.WriteLine("El proceso terminó " + termino_proceso);
                }

            }
            else
            {
                if (escribe_archivo)
                {
                    sw.WriteLine("El proceso se aborto por el flag de control.");
                }
            }
            if (escribe_archivo)
            {
                sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
            }
        }

        public static void Main(string[] args)
        {
            ActDescuento exe = new ActDescuento();

            // Creación de Archivo
            try
            {
                exe.CrearArchivoLog();
                sw.WriteLine("************ Inicio Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
            }
            catch
            {
                escribe_archivo = false;
            }
            // Creación de Log SQL
            try
            {
                exe.log.CreaLogProceso(exe.proceso);
            }
            catch (Exception ex)
            {
                escribe_log = false;
                if (escribe_archivo)
                {
                    sw.WriteLine("Error en Creación de Flag de Procesos en SQL.");
                    sw.WriteLine(ex.Message);
                }
            }
            // Obtener Datos desde SQL
            try
            {
                exe.Llenar_datos();
            }
            catch (Exception ex)
            {
                escribe_log = false;
                if (escribe_archivo)
                {
                    sw.WriteLine("Error en Obtener Datos Generales en SQL.");
                    sw.WriteLine(ex.Message);
                }
            }
            
            try
            {
                DataTable tabla = new DataTable();

                tabla = exe.ListaDescuentos(exe.tienda);

                exe.ActualizaDescuentos(tabla);
                sw.Close();
            }
            catch (Exception exep)
            {
                if (escribe_log) { exe.log.ActualizaLogProceso(exe.proceso, -1);  }
                if (escribe_archivo)
                {
                    sw.WriteLine(exep.Message);
                    sw.Close();
                }
            }
        }

    }
}

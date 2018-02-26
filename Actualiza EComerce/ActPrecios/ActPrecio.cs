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
using System.Globalization;

namespace ActPrecios
{
    public class ActPrecio
    {
        SqlConnection sql;
        Conexion oConexion = new Conexion();

        MySqlConnection mysql;
        Conexion oConexionMySql = new Conexion();
        
        LogProceso log = new LogProceso();

        string proceso = "ActPrecios";
        string archivo = "";
        string moneda = "PEN";
        string seccion = "5";
        string tienda = "e-com";
        public static StreamWriter sw;

        public DataTable ListaPrecios(string moneda, string seccion, string tienda)
        {
            sql = oConexion.getConexionSQL();
            SqlDataAdapter da = new SqlDataAdapter("USP_ECOM_LISTAPRECIOS", sql);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@moneda", SqlDbType.VarChar).Value = moneda;
            da.SelectCommand.Parameters.Add("@seccion", SqlDbType.VarChar).Value = seccion;
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

        public void CrearArchivoLog()
        {
            //datos para archivo LOG
            string path = Environment.CurrentDirectory;
            string nombre = "Log Errores";
            string nom_archiv = DateTime.Today.ToString("yyyy.MM.dd") + " Log Actualiza Precios";
            //crea directorio si no existe
            if (!Directory.Exists(path + "\\" + nombre))
            {   //Crea el directorio
                DirectoryInfo di = Directory.CreateDirectory(path + "\\" + nombre);
            }
            //Crea el Writer
            sw = File.AppendText(path + "\\" + nombre + "\\" + nom_archiv + ".txt");
            archivo = path + "\\" + nombre + "\\" + nom_archiv + ".txt";
        }
        

        public void ActualizaPrecios(DataTable precios)
        {
            if (log.ValidaProceso(proceso) != 1)
            {
                int estado = 1;
                //datos generales
                log.ActualizaLogProceso(proceso, -1);
                mysql = oConexionMySql.getConexionMySQL();
                mysql.Open();

                /*//datos para archivo LOG
                int contador = 1;
                string path = Environment.CurrentDirectory;
                string nombre = "Log Errores";
                string archivo = DateTime.Today.ToString("yyyy.MM.dd")+ " Log Error Precios - ";
                //crea directorio si no existe
                if (!Directory.Exists(path+"\\"+nombre))
                {   //Crea el directorio
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
                    sw.WriteLine(ex.Message);
                    estado = 0;
                    log.ActualizaLogProceso(proceso, estado);
                    sw.WriteLine("Error en lectura de productos en Prestashop.");
                    sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
                    return;
                }

                //Crea Datatable con datos Finales
                DataTable Final = new DataTable();
                Final = precios.Clone();
                Final.Clear();

                //Validacion
                //string val;
                foreach (DataRow row1 in precios.Rows)
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
                /*MySqlCommand comdel = mysql.CreateCommand();
                comdel.CommandText = "Delete From ps_specific_price;";
                try
                {
                    comdel.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    sw.WriteLine(ex.Message);
                    estado = 0;
                    log.ActualizaLogProceso(proceso, estado);
                    sw.WriteLine("Error en limpiar tabla de descuentos en Prestashop.");
                    sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
                    return;
                }*/

                int cant_reg = 0;
                //recorro datatable final
                foreach (DataRow row in Final.Rows)
                {
                    cant_reg = cant_reg + 1;
                    //Actualizar en Prestashop
                    double precio = Math.Round((Convert.ToDouble(row["precio1"]) / 1.18), 1);
                    MySqlCommand comm = mysql.CreateCommand();
                    MySqlCommand com2 = mysql.CreateCommand();
                    comm.CommandText = "Update ps_product p set p.price = " + precio.ToString() + " where replace(p.reference,'-','') = '" + row["product_id"] + "';";
                    com2.CommandText = "Update ps_product_shop ps Inner Join ps_product p On ps.id_product = p.id_product Set ps.price = " + precio.ToString() + " Where replace(p.reference, '-', '') = '" + row["product_id"] + "'; ";

                    try
                    {
                        //ejecucion
                        comm.ExecuteNonQuery();
                        com2.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        sw.WriteLine(ex.Message);
                        estado = 0;
                        log.ActualizaLogProceso(proceso, estado);
                        sw.WriteLine("Error en actualizar precio de producto " + row["product_id"] + " y descuento " + row["Monto"] + " en Prestashop.");
                    }
                }

                string termino_proceso;
                log.ActualizaLogProceso(proceso, estado);
                if (estado == 1)
                {
                    termino_proceso = "correctamente.";
                }
                else
                {
                    termino_proceso = "con errores.";
                }
                sw.WriteLine("Se actualizaron " + cant_reg.ToString() + " registros.");
                sw.WriteLine("El proceso terminó " + termino_proceso);
            }
            else
            {
                sw.WriteLine("El proceso se aborto por el flag de control.");
            }
            sw.WriteLine("************ Fin Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");
        }

        public static void Main(string[] args)
        {
            ActPrecio exe = new ActPrecio();

            exe.CrearArchivoLog();
            sw.WriteLine("************ Inicio Proceso:  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "");

            try
            {
                exe.log.CreaLogProceso(exe.proceso);
                
                DataTable tabla = new DataTable();

                tabla = exe.ListaPrecios(exe.moneda, exe.seccion, exe.tienda);

                exe.ActualizaPrecios(tabla);
                sw.Close();
            }
            catch (Exception ex)
            {
                exe.log.ActualizaLogProceso(exe.proceso, -1);
                sw.WriteLine(ex.Message);
                sw.Close();
            }

        }

    }
}

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

        public void ActualizaPrecios(DataTable precios)
        {
            if (log.ValidaProceso(proceso) != 1)
            {
                //datos generales
                int estado = 1;
                mysql = oConexionMySql.getConexionMySQL();
                mysql.Open();
                //Actualiza estado a proceso abierto
                log.ActualizaLogProceso(proceso, -1);

                //datos para archivo LOG
                int contador = 1;
                string path = Environment.CurrentDirectory;
                string nombre = "Log Errores";
                string archivo = DateTime.Today.ToString("yyyy.MM.dd")+ " Log Error Precios - ";
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
                StreamWriter sw = new StreamWriter(path + "\\" + nombre + "\\" + archivo + contador.ToString() + ".txt");

                //Valida con Productos Existentes
                string queryvalida = "Select Distinct id_product  From ps_product";
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
                    return;
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
                        if(row1["product_id"].ToString()== row2["id_product"].ToString())
                        {
                            val = "si";
                            Final.ImportRow(row1);
                            Productos.Rows.Remove(row2);
                            //descuentos.Rows.Remove(row1);
                            break;
                        }
                    }
                    if (val == "no")
                    {
                        estado = 0;
                        sw.WriteLine("No se encontró el producto " + row1["product_id"] + " en Prestashop.");
                    }
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
                    comm.CommandText = "Update ps_product p set p.price = " + precio.ToString() + " where p.id_product = " + row["product_id"] + ";";
                    com2.CommandText = "Update ps_product_shop p set p.price = "+ precio.ToString() + " where p.id_product = "+ row["product_id"] + ";";

                    try
                    {
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

                log.ActualizaLogProceso(proceso, estado);
                sw.Close();
            }
        }

        public static void Main(string[] args)
        {
            ActPrecio exe = new ActPrecio();

            exe.log.CreaLogProceso(exe.proceso);

            DataTable tabla = new DataTable();

            tabla = exe.ListaPrecios("PEN","5","e-com");

            exe.ActualizaPrecios(tabla);
        }

    }
}

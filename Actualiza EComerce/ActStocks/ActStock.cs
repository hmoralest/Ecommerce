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
        
        LogProceso log = new LogProceso();

        string proceso = "ActStocks";

        public DataTable ListaStocks(string tienda)
        {
            sql = oConexion.getConexionSQL();
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

        public void ActualizaOrigen(string tienda, string mov_id, string det_mov_id)
        {
            sql = oConexion.getConexionSQL();
            sql.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "USP_ECOM_ACTSTOCK";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sql;

            cmd.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
            cmd.Parameters.Add("@mov_id", SqlDbType.VarChar).Value = mov_id;
            cmd.Parameters.Add("@det_mov_id", SqlDbType.VarChar).Value = det_mov_id;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizaStocks(DataTable precios)
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
                string archivo = DateTime.Today.ToString("yyyy.MM.dd")+ " Log Error Stocks - ";
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
                StreamWriter sw = new StreamWriter(path + "\\" + nombre + "\\" + archivo + contador.ToString() + ".txt");

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
                        if(row1["product_id"].ToString()== row2["reference"].ToString())
                        {
                            val = "si";
                            Final.ImportRow(row1);
                            Productos.Rows.Remove(row2);
                            break;
                        }
                    }
                    if (val == "no")
                    {
                        estado = 0;
                        sw.WriteLine("No se encontró el producto con referencia " + row1["product_id"] + " en Prestashop.");
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
                    sw.WriteLine(ex.Message);
                    estado = 0;
                    log.ActualizaLogProceso(proceso, estado);
                    sw.WriteLine("Error en limpiar tabla de Movimientos en Prestashop.");
                    return;
                }

                int cant_reg = 0;
                //recorro datatable final
                foreach (DataRow row in Final.Rows)
                {
                    cant_reg = cant_reg + 1;
                    //Actualizar en Prestashop
                    MySqlCommand comm = mysql.CreateCommand();
                    comm.CommandText = "Insert into ps_erp values ('"+ row["product_id"] + "',"+ row["cantidad"] + ");";
                    
                    try
                    {
                        //ejecucion
                        comm.ExecuteNonQuery();
                        //Actualizar Movimiento para no repetir
                        ActualizaOrigen("11", row["mov_id"].ToString(), row["det_mov_id"].ToString());
                    }
                    catch(Exception ex)
                    {
                        sw.WriteLine(ex.Message);
                        estado = 0;
                        log.ActualizaLogProceso(proceso, estado);
                        sw.WriteLine("Error en actualizar Stock de producto con referencia " + row["product_id"] + " y movimiento " + row["cantidad"] + " en Prestashop.");
                    }
                }

                log.ActualizaLogProceso(proceso, estado);
                sw.Close();
        }

        public static void Main(string[] args)
        {
            ActStock exe = new ActStock();

            exe.log.CreaLogProceso(exe.proceso);

            DataTable tabla = new DataTable();

            tabla = exe.ListaStocks("11");

            exe.ActualizaStocks(tabla);
        }

    }
}

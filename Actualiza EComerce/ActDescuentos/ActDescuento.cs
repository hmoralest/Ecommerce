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

        public void ActualizaDescuentos(DataTable descuentos)
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
                StreamWriter sw = new StreamWriter(path + "\\" + nombre + "\\" + archivo + contador.ToString() + ".txt");

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
                    return;
                }

                //Crea Datatable con datos Finales
                DataTable Final = new DataTable();
                Final = descuentos.Clone();
                Final.Clear();

                //Validacion
                foreach (DataRow row1 in descuentos.Rows)
                {
                    foreach (DataRow row2 in Productos.Rows)
                    {
                        if(row1["product_id"].ToString()== row2["id_product"].ToString())
                        {
                            Final.ImportRow(row1);
                            Productos.Rows.Remove(row2);
                            //descuentos.Rows.Remove(row1);
                            break;
                        }
                    }
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
                    sw.WriteLine(ex.Message);
                    estado = 0;
                    log.ActualizaLogProceso(proceso, estado);
                    sw.WriteLine("Error en limpiar tabla de descuentos en Prestashop.");
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
                    cadena = cadena +   "Select " + cant_reg.ToString() + ",0,0,id_product,1,0,0,0,0,0,0,-1,1," + row["Monto"] + ",1,'amount','" + row["Fecha_Ini"] + "','" + row["Fecha_Fin"] + "' ";
                    cadena = cadena +   "From ps_product Where replace(reference,'-', '') = '" + row["product_id"] + "'";
                    comm.CommandText = cadena;
                    try
                    {
                        //ejecucion
                        comm.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        sw.WriteLine(ex.Message);
                        estado = 0;
                        log.ActualizaLogProceso(proceso, estado);
                        sw.WriteLine("Error en ingresar descuentos de producto " + row["product_id"] + " y descuento " + row["Monto"] + " en Prestashop.");
                    }
                }

                log.ActualizaLogProceso(proceso, estado);
                sw.Close();
            }
        }

        public static void Main(string[] args)
        {
            ActDescuento exe = new ActDescuento();

            exe.log.CreaLogProceso(exe.proceso);

            DataTable tabla = new DataTable();

            tabla = exe.ListaDescuentos("e-com");

            exe.ActualizaDescuentos(tabla);
        }

    }
}

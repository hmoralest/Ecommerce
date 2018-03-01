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

        string tienda = "11";
        string error = "";

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

        public void ActualizaOrigen(string tienda, string mov_id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "USP_ECOM_ACTSTOCK";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sql;

            cmd.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
            cmd.Parameters.Add("@mov_id", SqlDbType.VarChar).Value = mov_id;

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
                    error = "No se encontró producto " + row1["product_id"] + " en Prestashop";
                    throw new System.ArgumentException("Código de Producto Inválido", "reference");
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

            int cant_reg = 0;
            string id_mov = "";
            //recorro datatable final
            foreach (DataRow row in Final.Rows)
            {
                //Actualizar Movimiento para no repetir
                if (id_mov == row["mov_id"].ToString() && id_mov != "")
                {
                    try
                    {
                        ActualizaOrigen(tienda, id_mov);
                    }
                    catch (Exception ex)
                    {
                        error = "No se pudo actualizar estado de Movimiento " + id_mov + ".";
                        throw ex;
                    }
                }

                id_mov = row["mov_id"].ToString();
                cant_reg = cant_reg + 1;
                //Actualizar en Prestashop
                MySqlCommand comm = mysql.CreateCommand();
                comm.CommandText = "Insert into ps_erp values ('" + row["product_id"] + "'," + row["cantidad"] + ");";

                try
                {
                    //ejecucion
                    comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = "No se pudo insertar datos en la tabla ps_erp.";
                    throw ex;
                }
            }

            try
            {
                //Actualizar Movimiento para no repetir
                ActualizaOrigen(tienda, id_mov);
            }
            catch (Exception ex)
            {
                error = "No se pudo actualizar estado de Movimiento " + id_mov + ".";
                throw ex;
            }
        }

        public string EjecutaStock()
        {
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
                return "No se pudo abrir Transaccion. // " + ex.Message;
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
                return "Error: " + error + " // " + ex.Message;
            }
            sql.Close();
            mysql.Close();
            ControlaTrans(1);
            return "";
        }

        public static void Main()
        {
            ActStock exe = new ActStock();
            exe.EjecutaStock();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

namespace LeerVentas
{
    public class LeerVenta
    {
        Conexion oConexion = new Conexion();

        string path = "D:\\Actualizar Stock Almacen con Ventas\\Tablas\\";

        string tienda = "11"; //SQL

        string calidad = "1"; //DBF (producto)
        string almacen = "5"; //DBF (stock producto)
        string concepto = "60"; //DBF (movimiento)
        string client = "00000"; //DBF (movimiento - cliente generico)
        string empresa = "02"; //DBF (stock producto)
        string canal = "5"; //DBF (stock producto)
        string cadena = "BA"; //DBF (stock producto)
        string pack = "00001"; //DBF (stock producto)
        string cant_pack = "0"; //DBF (movimiento)

        DateTime Hoy = DateTime.Now;

        public DataTable ListaVentas()
        {
            DataTable result = new DataTable();

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlDataAdapter da = new SqlDataAdapter("USP_ListaVentas_PS", sql);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
                    
                    da.Fill(result);
                    sql.Close();
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            return result;
        }
        public DataTable ListaProductoVentas()
        {
            DataTable result = new DataTable();

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlDataAdapter da = new SqlDataAdapter("USP_ListaProdVentas_PS", sql);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;

                    da.Fill(result);
                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
            return result;
        }

        public void ActualizaVentas(string id_venta, string estado)
        {

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlCommand cmd = new SqlCommand();

                    cmd.CommandText = "USP_ActualizaVentas_PS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sql;

                    cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id_venta;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = estado;

                    cmd.ExecuteNonQuery();

                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        public void ActualizaLogVentas(string id_venta, string mensaje, string msje_sist)
        {

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlCommand cmd = new SqlCommand();

                    cmd.CommandText = "USP_Agrega_LogVentas";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sql;

                    cmd.Parameters.Add("@id_venta", SqlDbType.VarChar).Value = id_venta;
                    cmd.Parameters.Add("@mensaje", SqlDbType.VarChar).Value = mensaje;
                    cmd.Parameters.Add("@sistema", SqlDbType.VarChar).Value = msje_sist;

                    cmd.ExecuteNonQuery();

                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        public void Envia_correo()
        {

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlCommand cmd = new SqlCommand();

                    cmd.CommandText = "USP_CorreoAlmacen_PS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sql;
                    
                    cmd.ExecuteNonQuery();

                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        private void ValidaProductoVentas()
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            //string sConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(path) + ";Extended Properties=dBASE IV;";
            string sConn = "Provider = vfpoledb.1;Data Source=" + System.IO.Path.GetDirectoryName(path) + ";Collating Sequence=general";

            DataTable productos = new DataTable();
            productos = ListaProductoVentas();

            using (System.Data.OleDb.OleDbConnection dbConn = new System.Data.OleDb.OleDbConnection(sConn))
            {
                try
                {
                    dbConn.Open();
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas("", "Error en encontrar DBF.", ex.Message);
                }
                foreach (DataRow row in productos.Rows)
                {
                    int weekofyear = myCal.GetWeekOfYear(Convert.ToDateTime(row["fecha"]), myCWR, myFirstDOW);

                    try
                    {
                        string stock_pri = "SELECT * FROM SCACSAL WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(row["fecha"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + row["producto"].ToString() + "' And CSAL_CALID = '" + calidad + "'";
                        System.Data.OleDb.OleDbCommand stk1 = new System.Data.OleDb.OleDbCommand(stock_pri, dbConn);
                        System.Data.OleDb.OleDbDataAdapter cn_stk1 = new System.Data.OleDb.OleDbDataAdapter(stk1);
                        DataTable stock1 = new DataTable();
                        cn_stk1.Fill(stock1);

                        string stock_seg = "SELECT * FROM SCACSALP WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(row["fecha"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + row["producto"].ToString() + "' And CSAL_CALID = '" + calidad + "'";
                        System.Data.OleDb.OleDbCommand stk2 = new System.Data.OleDb.OleDbCommand(stock_seg, dbConn);
                        System.Data.OleDb.OleDbDataAdapter cn_stk2 = new System.Data.OleDb.OleDbDataAdapter(stk2);
                        DataTable stock2 = new DataTable();
                        cn_stk2.Fill(stock2);

                        if (stock1.Rows.Count == 0 || stock2.Rows.Count == 0)
                        {
                            ActualizaVentas(row["id"].ToString(), "E");
                            ActualizaLogVentas(row["id"].ToString(), "Producto " + row["producto"].ToString() + " no tiene registro semanal.", "Tablas SCACSAL y SCACSALP");
                        }
                    }
                    catch (Exception ex)
                    {
                        ActualizaLogVentas("", "Error en Validación de Productos.", ex.Message);
                    }
                }
                try
                {
                    //Se termina transacción y cerramos conexión
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas("", "Error en Cierre de Conexión a DBF.", ex.Message);
                }
            }
        }

        private void ActualizaenDBF()
        {
            //string sConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(path) + ";Extended Properties=dBASE IV;";
            string sConn = "Provider = vfpoledb.1;Data Source=" + System.IO.Path.GetDirectoryName(path) + ";Collating Sequence=general";
            string producto = "";
            string validavta = "";
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            //validacion de errores
            bool ActSCACODI = false;
            bool InsSCCCSXN = false;
            bool InsSCDRSXN = false;
            bool ActSCACSAL = false;
            //bool InsSCACSAL = false;
            bool ActSCACSALP = false;
            //bool InsSCACSALP = false;
            // Datos para hacer rollback
            string codigo1_ant = "";
            string codigo2_ant = "";
            string codigo = "";
            string[] datscasal = new string[3];
            List<string[]> scasal = new List<string[]>();
            List<string[]> scasalp = new List<string[]>();

            //Valida semanas anteriores
            int min_week = Convert.ToInt32(Hoy.ToString("yyyy")+ Convert.ToString(myCal.GetWeekOfYear(Hoy, myCWR, myFirstDOW)).PadLeft(2).Replace(" ", "0"));

            DataTable datos = new DataTable();

            try
            {
                ValidaProductoVentas();

                //Lleno datos iniciales
                datos = ListaVentas();
            }
            catch (Exception ex)
            {
                ActualizaLogVentas("", "Error en Obtención de datos de SLQ Server.", ex.Message);
                return;
            }

            using (System.Data.OleDb.OleDbConnection dbConn = new System.Data.OleDb.OleDbConnection(sConn))
            {
                //System.Data.OleDb.OleDbTransaction tran;
                try
                {
                    // Abro conexión
                    dbConn.Open();
                    // transaccion Abierta
                    //tran = dbConn.BeginTransaction();
                }
                catch(Exception ex)
                {
                    ActualizaLogVentas("", "Error en encontrar DBF.", ex.Message);
                    //throw ex;
                }

                foreach(DataRow venta in datos.Rows)
                {
                    producto = venta["product_id"].ToString();

                    // variables
                    string codigo1 = "";
                    string codigo2 = "";

                    int weekofyear = myCal.GetWeekOfYear(Convert.ToDateTime(venta["fecha_vta"]), myCWR, myFirstDOW);
                    //Valida la menor semana q se está actualizando
                    if(Convert.ToInt32(Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + Convert.ToString(weekofyear).PadLeft(2).Replace(" ", "0")) < min_week)
                    {
                        min_week = Convert.ToInt32(Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + Convert.ToString(weekofyear).PadLeft(2).Replace(" ", "0"));
                    }
                    try
                    {

                        if (validavta != venta["id_venta"].ToString())
                        {
                            // Se Setean en False (Nueva Venta)
                            ActSCACODI = false;
                            InsSCCCSXN = false;
                            InsSCDRSXN = false;
                            ActSCACSAL = false;
                            //InsSCACSAL = false;
                            ActSCACSALP = false;
                            //InsSCACSALP = false;
                            // se limpian variables para hacer rollback
                            codigo1_ant = "";
                            codigo2_ant = "";
                            scasal.Clear();
                            scasalp.Clear();

                            //**-- CODIGO
                            //-------------------------------------
                            // Obtengo código
                            //-------------------------------------
                            string consulta = "SELECT TAB_CPAR2,TAB_CPAR1 FROM SCACODI WHERE TAB_EMPRE='02' And TAB_TIPO='090' And TAB_CTAB = '004'";
                            System.Data.OleDb.OleDbCommand con = new System.Data.OleDb.OleDbCommand(consulta, dbConn);
                            System.Data.OleDb.OleDbDataAdapter cona = new System.Data.OleDb.OleDbDataAdapter(con);
                            DataTable dt = new DataTable();
                            //con.Transaction = tran;
                            cona.Fill(dt);

                            foreach (DataRow r in dt.Rows)
                            {
                                codigo1_ant = Convert.ToString(r["TAB_CPAR2"]).Trim();
                                codigo2_ant = Convert.ToString(r["TAB_CPAR1"]).Trim();
                                if (Convert.ToString(r["TAB_CPAR2"]).Trim() == Hoy.ToString("yyyy"))
                                {
                                    codigo1 = Convert.ToString(r["TAB_CPAR2"]).Trim();
                                    codigo2 = Convert.ToString(Convert.ToInt32(r["TAB_CPAR1"]) + 1).PadLeft(8).Replace(" ", "0");
                                }
                                else
                                {
                                    codigo1 = Hoy.ToString("yyyy");
                                    codigo2 = Convert.ToString(1).Trim().PadLeft(8).Replace(" ", "0");
                                }
                                codigo = codigo1 + codigo2;
                            }

                            //-------------------------------------
                            // Actualizo datos
                            //-------------------------------------
                            //string actualiza = "Insert Into SCACODI(TAB_EMPRE, TAB_TIPO, TAB_CTAB, TAB_CDESC, TAB_CPAR1, TAB_CPAR2, TAB_TIPO2, TAB_CDESL, TAB_CDANT, TAB_ALMAC, TAB_DUA, TAB_HUA, TAB_CUU) Select '02','090','004', 'Descripcion', '00000000','2018','','descrip2','descrip3','5','05/03/2018','',''";
                            string actualiza = "UPDATE SCACODI SET TAB_CPAR1='" + codigo2 + "',TAB_CPAR2='" + codigo1 + "'  WHERE TAB_EMPRE='"+empresa+"' And TAB_TIPO='090' And TAB_CTAB = '004'";
                            System.Data.OleDb.OleDbCommand act = new System.Data.OleDb.OleDbCommand(actualiza, dbConn);
                            //act.Transaction = tran;
                            act.ExecuteNonQuery();
                            ActSCACODI = true;

                            //**-- MOVIMIENTO
                            //-------------------------------------
                            // Ingreso datos de Movimiento Cabecera
                            //-------------------------------------
                            string mov_cab = "Insert Into SCCCSXN (CSXN_ALMAC, CSXN_CODIG, CSXN_FSALI, CSXN_CONCE, CSXN_ENTID, CSXN_GGUIA, CSXN_ESTAD, CSXN_TUCAL, CSXN_TUNCA, CSXN_TENTI, CSXN_TDOC1, CSXN_NDOC1, CSXN_TDOC2, CSXN_NDOC2, CSXN_SECCI, CSXN_HSALI, CSXN_PROVE, CSXN_DESTI, CSXN_AORIG, CSXN_OBSER, CSXN_EMPRE, CSXN_CANAL, CSXN_CADEN, CSXN_NMOVC, CSXN_FLASE, CSXN_UUSUA, CSXN_FUACT, CSXN_HUACT, CSXN_FTX, CSXN_FTXAQ, CSXN_TXLX, CSXN_LOG) ";
                            //mov_cab += "Select '" + almacen + "','" + codigo + "','" + venta["fecha_vta"].ToString() + "','"+concepto+"', '"+ client + "', '','','" + venta["cant_calzado"].ToString() + "','" + venta["cant_no_calzado"].ToString() + "','','" + venta["tpdoc"].ToString() + "','" + venta["nro_doc"].ToString() + "','" + venta["tp_ped"].ToString() + "','" + venta["cod_ped"].ToString() + "','"+almacen+"','" + venta["hora_vta"].ToString() + "','','','','Venta por E-Commerce Pedido: " + venta["cod_ped"].ToString() + "','"+empresa+"','"+canal+"','"+cadena+"','','','E-COM','" + venta["fecha_reg"].ToString() + "','" + venta["hora_reg"].ToString() + "','','','','"+Hoy.ToString("yyyy-MM-dd HH:mm:ss")+ " E-COMMERCE'";
                            mov_cab += "VALUES ('" + almacen + "','" + codigo + "',CTOD('" + venta["fecha_vta"].ToString() + "'),'" + concepto + "', '" + client + "', '',''," + venta["cant_calzado"].ToString() + "," + venta["cant_no_calzado"].ToString() + ",'','" + venta["tpdoc"].ToString() + "','" + venta["nro_doc"].ToString() + "','" + venta["tp_ped"].ToString() + "','" + venta["cod_ped"].ToString() + "','" + almacen + "','" + venta["hora_vta"].ToString() + "','','','','Venta por E-Commerce Pedido: " + venta["cod_ped"].ToString() + "','" + empresa + "','" + canal + "','" + cadena + "','','','E-COM',CTOD('" + venta["fecha_reg"].ToString() + "'),'" + venta["hora_reg"].ToString() + "','','','','" + Hoy.ToString("yyyy-MM-dd HH:mm:ss") + " E-COMMERCE')";
                            System.Data.OleDb.OleDbCommand mov1 = new System.Data.OleDb.OleDbCommand(mov_cab, dbConn);
                            //mov1.Transaction = tran;
                            mov1.ExecuteNonQuery();
                            InsSCCCSXN = true;
                        }

                        //-------------------------------------
                        // Ingreso datos de Movimiento Detalle
                        //-------------------------------------
                        string cadenaMED = "";
                        if(venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "00")
                        {
                            cadenaMED = "RSXN_MED00, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }else if(venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "01")
                        {
                            cadenaMED = "RSXN_MED01, RSXN_MED00, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "02")
                        {
                            cadenaMED = "RSXN_MED02, RSXN_MED01, RSXN_MED00, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "03")
                        {
                            cadenaMED = "RSXN_MED03, RSXN_MED01, RSXN_MED02, RSXN_MED00, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "04")
                        {
                            cadenaMED = "RSXN_MED04, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED00, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "05")
                        {
                            cadenaMED = "RSXN_MED05, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED00, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "06")
                        {
                            cadenaMED = "RSXN_MED06, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED00, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "07")
                        {
                            cadenaMED = "RSXN_MED07, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED00, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "08")
                        {
                            cadenaMED = "RSXN_MED08, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED00, RSXN_MED09, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "09")
                        {
                            cadenaMED = "RSXN_MED09, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED00, RSXN_MED10, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "10")
                        {
                            cadenaMED = "RSXN_MED10, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED00, RSXN_MED11";
                        }
                        else if (venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") == "11")
                        {
                            cadenaMED = "RSXN_MED11, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED00";
                        }
                        //, RSXN_MED00, RSXN_MED01, RSXN_MED02, RSXN_MED03, RSXN_MED04, RSXN_MED05, RSXN_MED06, RSXN_MED07, RSXN_MED08, RSXN_MED09, RSXN_MED10, RSXN_MED11
                        string mov_det = "Insert Into SCDRSXN (RSXN_ALMAC, RSXN_CODIG, RSXN_ARTIC, RSXN_CALID, RSXN_CALDV, RSXN_TOUNI, RSXN_CPACK, RSXN_CODPP, RSXN_PPACK, RSXN_ORDCO, RSXN_GPROV, RSXN_RMED, "+cadenaMED+", RSXN_FECHA, RSXN_EMPRE, RSXN_SECCI, RSXN_PRECI, RSXN_CODV, RSXN_PREDV, RSXN_COSDV, RSXN_CPORC, RSXN_VPORC, RSXN_FLAGC, RSXN_MERC, RSXN_CATEG, RSXN_SUBCA, RSXN_MARCA, RSXN_MERC3, RSXN_CATE3, RSXN_SUBC3, RSXN_MARC3, RSXN_FTXV, RSXN_FTX) ";
                        //mov_det += "Select '" + almacen + "','" + codigo + "','" + producto + "', '" + venta["calidad"].ToString() + "', '', '" + Convert.ToString(Convert.ToInt32(venta["cant_calzado"])+ Convert.ToInt32(venta["cant_no_calzado"])) +"','"+pack+"','','"+cant_pack+ "','','','" + venta["tipo_med"].ToString() + "','" + Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"])) + "','" + venta["fecha_vta"].ToString() + "','"+empresa+"','"+almacen+"',"+ venta["precio"].ToString() + ","+ venta["costo"].ToString() + ",0.00,0.00,'',0.00,'"+venta["estandar_consig"].ToString()+ "','" + venta["linea"].ToString() + "','" + venta["categ"].ToString() + "','" + venta["subcat"].ToString() + "','" + venta["marca"].ToString() + "','" + venta["rims_linea"].ToString() + "','" + venta["rims_categ"].ToString() + "','" + venta["rims_subcat"].ToString() + "','" + venta["rims_marca"].ToString() + "','',''";
                        mov_det += "VALUES ('" + almacen + "','" + codigo + "','" + producto + "', '" + venta["calidad"].ToString() + "', '', " + Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"])) + ",'" + pack + "',''," + cant_pack + ",'','','" + venta["tipo_med"].ToString() + "'," + Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"])) + ",0,0,0,0,0,0,0,0,0,0,0,CTOD('" + venta["fecha_vta"].ToString() + "'),'" + empresa + "','" + almacen + "'," + venta["precio"].ToString() + "," + venta["costo"].ToString() + ",0.00,0.00,'',0.00,'" + venta["estandar_consig"].ToString() + "','" + venta["linea"].ToString() + "','" + venta["categ"].ToString() + "','" + venta["subcat"].ToString() + "','" + venta["marca"].ToString() + "','" + venta["rims_linea"].ToString() + "','" + venta["rims_categ"].ToString() + "','" + venta["rims_subcat"].ToString() + "','" + venta["rims_marca"].ToString() + "','','')";
                        System.Data.OleDb.OleDbCommand mov2 = new System.Data.OleDb.OleDbCommand(mov_det, dbConn);
                        //mov2.Transaction = tran;
                        mov2.ExecuteNonQuery();
                        InsSCDRSXN = true;
                        

                        //**-- STOCK
                        //-------------------------------------
                        // Busco datos del producto en tabla 1
                        //-------------------------------------
                        //string stock_pri = "SELECT * FROM SCACSAL WHERE CSAL_ANO = '"+ Hoy.ToString("yyyy")+ "' And CSAL_SEMAN = '"+weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '"+almacen+ "' And CSAL_ARTIC='"+producto+ "' And CSAL_CALID = '"+calidad+ "' And CSAL_EMPRE ='"+empresa+"'";
                        /*string stock_pri = "SELECT * FROM SCACSAL WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_CALID = '" + calidad + "' And CSAL_ARTIC='" + producto + "'";
                        System.Data.OleDb.OleDbCommand stk1 = new System.Data.OleDb.OleDbCommand(stock_pri, dbConn);
                        System.Data.OleDb.OleDbDataAdapter cn_stk1 = new System.Data.OleDb.OleDbDataAdapter(stk1);
                        DataTable stock1 = new DataTable();
                        //stk1.Transaction = tran;
                        cn_stk1.Fill(stock1);*/

                        string act_stock_pri="";
                        //Validar si existe                                                                                                                                             
                        //if(stock1.Rows.Count == 0)
                        //{
                            
                            // Se lanza error para que no registre a menos que haya cierre semanal.
                            //throw new System.ArgumentException("Producto "+ producto+" no tiene registro semanal.", "SCACSAL");
                            // Se toma datos de la semana anterior para crear un nuevo registro
                            /*string stock_pri_ant = "SELECT * FROM SCACSAL WHERE CSAL_ANO = '" + Hoy.ToString("yyyy") + "' And CSAL_SEMAN = '" + (weekofyear-1).ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + producto + "' And CSAL_CALID = '1'";
                            System.Data.OleDb.OleDbCommand stk1_ant = new System.Data.OleDb.OleDbCommand(stock_pri_ant, dbConn);
                            System.Data.OleDb.OleDbDataAdapter cn_stk1_ant = new System.Data.OleDb.OleDbDataAdapter(stk1_ant);
                            DataTable stock1_ant = new DataTable();
                            stk1_ant.Transaction = tran;
                            cn_stk1_ant.Fill(stock1_ant);

                            foreach (DataRow dat_ant in stock1_ant.Rows)
                            {
                                act_stock_pri  = "Insert Into SCACSAL (CSAL_ANO, CSAL_SEMAN, CSAL_ALMAC, CSAL_ARTIC, CSAL_CALID, CSAL_STKTO, CSAL_TPRES, CSAL_TEMBA, CSAL_DESPA, CSAL_RECIB, CSAL_DEVPR, CSAL_REDTC, CSAL_RECNO, CSAL_EMBAL, CSAL_SALNO, CSAL_STKAN, CSAL_CPACK, CSAL_CODPP, CSAL_PPACK, CSAL_MED00, CSAL_MED01, CSAL_MED02, CSAL_MED03, CSAL_MED04, CSAL_MED05, CSAL_MED06, CSAL_MED07, CSAL_MED08, CSAL_MED09, CSAL_MED10, CSAL_MED11, CSAL_PRE00, CSAL_PRE01, CSAL_PRE02, CSAL_PRE03, CSAL_PRE04, CSAL_PRE05, CSAL_PRE06, CSAL_PRE07, CSAL_PRE08, CSAL_PRE09, CSAL_PRE10, CSAL_PRE11, CSAL_EMB00, CSAL_EMB01, CSAL_EMB02, CSAL_EMB03, CSAL_EMB04, CSAL_EMB05, CSAL_EMB06, CSAL_EMB07, CSAL_EMB08, CSAL_EMB09, CSAL_EMB10, CSAL_EMB11, CSAL_COSUN, CSAL_VALAN, CSAL_PROVE, CSAL_CLASE, CSAL_MERC, CSAL_CATEG, CSAL_SUBC, CSAL_MARCA, CSAL_MERC3, CSAL_CATE3, CSAL_SUBC3, CSAL_MARC3, CSAL_CNDME, CSAL_ORIGE, CSAL_RMED, CSAL_EMPRE, CSAL_CADEN, CSAL_CANAL, CSAL_CODAL, CSAL_SECCI, CSAL_ESTAD, CSAL_FTX) ";
                                act_stock_pri += "SELECT '" + Hoy.ToString("yyyy") + "', '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "', '"+almacen+"', '"+producto+"', '1', "+ Convert.ToString(Convert.ToInt32(dat_ant["CSAL_STKTO"]) - Convert.ToInt32(venta["cantidad"]))+", "+ dat_ant["CSAL_TPRES"].ToString()+", 0, "+ Convert.ToString(venta["cantidad"]) + ",'0','0','0','0','0','0','"+ dat_ant["CSAL_STKTO"] + "','','"+ dat_ant["CSAL_CODPP"]+"','"+ dat_ant["CSAL_PPACK"]+"','"+ dat_ant["CSAL_MED00"] + "','" + dat_ant["CSAL_MED01"] + "','" + dat_ant["CSAL_MED02"] + "','" + dat_ant["CSAL_MED03"] + "','" + dat_ant["CSAL_MED04"] + "','" + dat_ant["CSAL_MED05"] + "','" + dat_ant["CSAL_MED06"] + "','" + dat_ant["CSAL_MED07"] + "','" + dat_ant["CSAL_MED08"] + "','" + dat_ant["CSAL_MED09"] + "','" + dat_ant["CSAL_MED10"] + "','" + dat_ant["CSAL_MED11"] + "','" + dat_ant["CSAL_PRE00"] + "','" + dat_ant["CSAL_PRE01"] + "','" + dat_ant["CSAL_PRE02"] + "','" + dat_ant["CSAL_PRE03"] + "','" + dat_ant["CSAL_PRE04"] + "','" + dat_ant["CSAL_PRE05"] + "','" + dat_ant["CSAL_PRE06"] + "','" + dat_ant["CSAL_PRE07"] + "','" + dat_ant["CSAL_PRE08"] + "','" + dat_ant["CSAL_PRE09"] + "','" + dat_ant["CSAL_PRE10"] + "','" + dat_ant["CSAL_PRE11"] + "','" + dat_ant["CSAL_EMB00"] + "','" + dat_ant["CSAL_EMB01"] + "','" + dat_ant["CSAL_EMB02"] + "','" + dat_ant["CSAL_EMB03"] + "','" + dat_ant["CSAL_EMB04"] + "','" + dat_ant["CSAL_EMB05"] + "','" + dat_ant["CSAL_EMB06"] + "','" + dat_ant["CSAL_EMB07"] + "','" + dat_ant["CSAL_EMB08"] + "','" + dat_ant["CSAL_EMB09"] + "','" + dat_ant["CSAL_EMB10"] + "','" + dat_ant["CSAL_EMB11"] + "','" + dat_ant["CSAL_COSUN"] + "','" + dat_ant["CSAL_VALAN"] + "','" + dat_ant["CSAL_PROVE"] + "','" + dat_ant["CSAL_CLASE"] + "','" + dat_ant["CSAL_MERC"] + "','" + dat_ant["CSAL_CATEG"] + "','" + dat_ant["CSAL_SUBC"] + "','" + dat_ant["CSAL_MARCA"] + "','" + dat_ant["CSAL_MERC3"] + "','" + dat_ant["CSAL_CATE3"] + "','" + dat_ant["CSAL_SUBC3"] + "','" + dat_ant["CSAL_MARC3"] + "','" + dat_ant["CSAL_CNDME"] + "','" + dat_ant["CSAL_ORIGE"] + "','" + dat_ant["CSAL_RMED"] + "','" + dat_ant["CSAL_EMPRE"] + "','" + dat_ant["CSAL_CADEN"] + "','" + dat_ant["CSAL_CANAL"] + "','" + dat_ant["CSAL_CODAL"] + "','" + dat_ant["CSAL_SECCI"] + "','" + dat_ant["CSAL_ESTAD"] + "','" + dat_ant["CSAL_FTX"]+"'";
                            }
                            // Se ejecuta sentencia
                            System.Data.OleDb.OleDbCommand act_stk1 = new System.Data.OleDb.OleDbCommand(act_stock_pri, dbConn);
                            act_stk1.Transaction = tran;
                            act_stk1.ExecuteNonQuery();
                            InsSCACSAL = true;*/
                        /*}
                        else
                        {*/
                            // Actualizo datos del registro que se tiene 
                            //foreach (DataRow dat in stock1.Rows)
                            //{
                                datscasal[0] = producto;
                                datscasal[1] = Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"]));
                                datscasal[2] = venta["col_med"].ToString();
                                act_stock_pri = "UPDATE SCACSAL SET CSAL_STKTO=CSAL_STKTO-" + datscasal[1] + ", CSAL_SALNO = CSAL_SALNO+" + datscasal[1] + ",CSAL_MED"+ venta["col_med"].ToString().PadLeft(2).Replace(" ", "0")+ "=CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + "-" + datscasal[1] + " WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_CALID = '" + calidad + "' And CSAL_ARTIC='" + producto + "'";
                            //}
                            // Se ejecuta sentencia
                            System.Data.OleDb.OleDbCommand act_stk1 = new System.Data.OleDb.OleDbCommand(act_stock_pri, dbConn);
                            //act_stk1.Transaction = tran;
                            act_stk1.ExecuteNonQuery();
                            scasal.Add(datscasal);
                            ActSCACSAL = true;
                        //}

                        //throw new System.ArgumentException("Código de Producto Inválido", "reference");
                        //-------------------------------------
                        // Busco datos del producto en tabla 2
                        //-------------------------------------
                        /*string stock_seg = "SELECT * FROM SCACSALP WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_CALID = '" + calidad + "' And CSAL_ARTIC='" + producto + "' And CSAL_CPACK='" + pack + "'";
                        System.Data.OleDb.OleDbCommand stk2 = new System.Data.OleDb.OleDbCommand(stock_seg, dbConn);
                        System.Data.OleDb.OleDbDataAdapter cn_stk2 = new System.Data.OleDb.OleDbDataAdapter(stk2);
                        DataTable stock2 = new DataTable();
                        //stk2.Transaction = tran;
                        cn_stk2.Fill(stock2);*/

                        string act_stock_seg = "";
                        //Validar si existe
                        //if (stock2.Rows.Count == 0)
                        //{
                            // Se lanza error para que no registre a menos que haya cierre semanal.
                            //throw new System.ArgumentException("Producto " + producto + " no tiene registro semanal.", "SCACSALP");
                            // Se toma datos de la semana anterior para crear un nuevo registro
                            /*string stock_seg_ant = "SELECT * FROM SCACSALP WHERE CSAL_ANO = '" + Hoy.ToString("yyyy") + "' And CSAL_SEMAN = '" + (weekofyear - 1).ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + producto + "' And CSAL_CALID = '1' And CSAL_CPACK='00001'";
                            System.Data.OleDb.OleDbCommand stk2_ant = new System.Data.OleDb.OleDbCommand(stock_seg_ant, dbConn);
                            System.Data.OleDb.OleDbDataAdapter cn_stk2_ant = new System.Data.OleDb.OleDbDataAdapter(stk2_ant);
                            DataTable stock2_ant = new DataTable();
                            stk2_ant.Transaction = tran;
                            cn_stk2_ant.Fill(stock2_ant);

                            foreach (DataRow dat_ant in stock2_ant.Rows)
                            {
                                act_stock_seg = "Insert Into SCACSALP (CSAL_ANO, CSAL_SEMAN, CSAL_ALMAC, CSAL_ARTIC, CSAL_CALID, CSAL_STKTO, CSAL_TPRES, CSAL_TEMBA, CSAL_DESPA, CSAL_RECIB, CSAL_DEVPR, CSAL_REDTC, CSAL_RECNO, CSAL_EMBAL, CSAL_SALNO, CSAL_STKAN, CSAL_CPACK, CSAL_CODPP, CSAL_PPACK, CSAL_MED00, CSAL_MED01, CSAL_MED02, CSAL_MED03, CSAL_MED04, CSAL_MED05, CSAL_MED06, CSAL_MED07, CSAL_MED08, CSAL_MED09, CSAL_MED10, CSAL_MED11, CSAL_PRE00, CSAL_PRE01, CSAL_PRE02, CSAL_PRE03, CSAL_PRE04, CSAL_PRE05, CSAL_PRE06, CSAL_PRE07, CSAL_PRE08, CSAL_PRE09, CSAL_PRE10, CSAL_PRE11, CSAL_EMB00, CSAL_EMB01, CSAL_EMB02, CSAL_EMB03, CSAL_EMB04, CSAL_EMB05, CSAL_EMB06, CSAL_EMB07, CSAL_EMB08, CSAL_EMB09, CSAL_EMB10, CSAL_EMB11, CSAL_COSUN, CSAL_VALAN, CSAL_PROVE, CSAL_CLASE, CSAL_MERC, CSAL_CATEG, CSAL_SUBC, CSAL_MARCA, CSAL_MERC3, CSAL_CATE3, CSAL_SUBC3, CSAL_MARC3, CSAL_CNDME, CSAL_ORIGE, CSAL_RMED, CSAL_EMPRE, CSAL_CADEN, CSAL_CANAL, CSAL_CODAL, CSAL_SECCI, CSAL_ESTAD, CSAL_FTX) ";
                                act_stock_seg += "SELECT '" + Hoy.ToString("yyyy") + "', '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "', '" + almacen + "', '" + producto + "', '1', '" + Convert.ToString(Convert.ToInt32(dat_ant["CSAL_STKTO"]) - Convert.ToInt32(venta["cantidad"])) + "', '" + dat_ant["CSAL_TPRES"].ToString() + "', 0, '" + Convert.ToString(venta["cantidad"]) + "',0,0,0,0,0,0,'" + dat_ant["CSAL_STKTO"] + "','','" + dat_ant["CSAL_CODPP"] + "','" + dat_ant["CSAL_PPACK"] + "','" + dat_ant["CSAL_MED00"] + "','" + dat_ant["CSAL_MED01"] + "','" + dat_ant["CSAL_MED02"] + "','" + dat_ant["CSAL_MED03"] + "','" + dat_ant["CSAL_MED04"] + "','" + dat_ant["CSAL_MED05"] + "','" + dat_ant["CSAL_MED06"] + "','" + dat_ant["CSAL_MED07"] + "','" + dat_ant["CSAL_MED08"] + "','" + dat_ant["CSAL_MED09"] + "','" + dat_ant["CSAL_MED10"] + "','" + dat_ant["CSAL_MED11"] + "','" + dat_ant["CSAL_PRE00"] + "','" + dat_ant["CSAL_PRE01"] + "','" + dat_ant["CSAL_PRE02"] + "','" + dat_ant["CSAL_PRE03"] + "','" + dat_ant["CSAL_PRE04"] + "','" + dat_ant["CSAL_PRE05"] + "','" + dat_ant["CSAL_PRE06"] + "','" + dat_ant["CSAL_PRE07"] + "','" + dat_ant["CSAL_PRE08"] + "','" + dat_ant["CSAL_PRE09"] + "','" + dat_ant["CSAL_PRE10"] + "','" + dat_ant["CSAL_PRE11"] + "','" + dat_ant["CSAL_EMB00"] + "','" + dat_ant["CSAL_EMB01"] + "','" + dat_ant["CSAL_EMB02"] + "','" + dat_ant["CSAL_EMB03"] + "','" + dat_ant["CSAL_EMB04"] + "','" + dat_ant["CSAL_EMB05"] + "','" + dat_ant["CSAL_EMB06"] + "','" + dat_ant["CSAL_EMB07"] + "','" + dat_ant["CSAL_EMB08"] + "','" + dat_ant["CSAL_EMB09"] + "','" + dat_ant["CSAL_EMB10"] + "','" + dat_ant["CSAL_EMB11"] + "','" + dat_ant["CSAL_COSUN"] + "','" + dat_ant["CSAL_VALAN"] + "','" + dat_ant["CSAL_PROVE"] + "','" + dat_ant["CSAL_CLASE"] + "','" + dat_ant["CSAL_MERC"] + "','" + dat_ant["CSAL_CATEG"] + "','" + dat_ant["CSAL_SUBC"] + "','" + dat_ant["CSAL_MARCA"] + "','" + dat_ant["CSAL_MERC3"] + "','" + dat_ant["CSAL_CATE3"] + "','" + dat_ant["CSAL_SUBC3"] + "','" + dat_ant["CSAL_MARC3"] + "','" + dat_ant["CSAL_CNDME"] + "','" + dat_ant["CSAL_ORIGE"] + "','" + dat_ant["CSAL_RMED"] + "','" + dat_ant["CSAL_EMPRE"] + "','" + dat_ant["CSAL_CADEN"] + "','" + dat_ant["CSAL_CANAL"] + "','" + dat_ant["CSAL_CODAL"] + "','" + dat_ant["CSAL_SECCI"] + "','" + dat_ant["CSAL_ESTAD"] + "','" + dat_ant["CSAL_FTX"] + "'";
                            }
                            // Se ejecuta sentencia
                            System.Data.OleDb.OleDbCommand act_stk2 = new System.Data.OleDb.OleDbCommand(act_stock_seg, dbConn);
                            act_stk2.Transaction = tran;
                            act_stk2.ExecuteNonQuery();
                            InsSCACSAL = true;*/
                        /*}
                        else
                        {*/
                            // Actualizo datos del registro que se tiene 
                            //foreach (DataRow dat in stock2.Rows)
                            //{
                                datscasal[0] = producto;
                                datscasal[1] = Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"]));
                                datscasal[2] = venta["col_med"].ToString();
                                act_stock_seg = "UPDATE SCACSALP SET CSAL_STKTO=CSAL_STKTO-" + datscasal[1] + ", CSAL_SALNO = CSAL_SALNO+" + datscasal[1] + ",CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + "-" + datscasal[1] + " WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_CALID = '" + calidad + "' And CSAL_ARTIC='" + producto + "' And CSAL_CPACK='"+pack+"'";
                            //}
                            // Se ejecuta sentencia
                            System.Data.OleDb.OleDbCommand act_stk2 = new System.Data.OleDb.OleDbCommand(act_stock_seg, dbConn);
                            //act_stk2.Transaction = tran;
                            act_stk2.ExecuteNonQuery();
                            scasalp.Add(datscasal);
                            ActSCACSALP = true;
                        //}
                        // Termina de Actualizar y actualiza en la BD P = PROCESADO
                        ActualizaVentas(venta["id_venta"].ToString(), "P");
                    }
                    catch (Exception ex)
                    {
                        //tran.Rollback();
                        // RollBack Manual
                        /*if (ActSCACODI)
                        {
                            string actualiza_rb = "UPDATE SCACODI SET TAB_CPAR1='" + codigo2_ant + "',TAB_CPAR2='" + codigo1_ant + "' WHERE TAB_EMPRE='"+empresa+"' And TAB_TIPO='090' And TAB_CTAB = '004'";
                            System.Data.OleDb.OleDbCommand act_rb = new System.Data.OleDb.OleDbCommand(actualiza_rb, dbConn);
                            act_rb.ExecuteNonQuery();
                        }*/
                        if (InsSCCCSXN)
                        {
                            string mov_cab_rb = "DELETE FROM SCCCSXN WHERE CSXN_ALMAC='"+almacen+ "' And CSXN_CODIG='"+codigo+ "'";
                            System.Data.OleDb.OleDbCommand mov1_rb = new System.Data.OleDb.OleDbCommand(mov_cab_rb, dbConn);
                            mov1_rb.ExecuteNonQuery();
                        }
                        if (InsSCDRSXN)
                        {
                            string mov_det_rb = "DELETE FROM SCDRSXN WHERE RSXN_ALMAC='" + almacen + "' And RSXN_CODIG='" + codigo + "'";
                            System.Data.OleDb.OleDbCommand mov2_rb = new System.Data.OleDb.OleDbCommand(mov_det_rb, dbConn);
                            mov2_rb.ExecuteNonQuery();
                        }
                        if (ActSCACSAL)
                        {
                            foreach (string[] dat_rb in scasal)
                            {
                                string act_stock_pri_rb = "UPDATE SCACSAL SET CSAL_STKTO=CSAL_STKTO+" + dat_rb[1].ToString() + ", CSAL_SALNO=CSAL_SALNO-" + dat_rb[1].ToString() + ", CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + "-"+ dat_rb[1].ToString() + " WHERE CSAL_ANO = '" + Hoy.ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + dat_rb[0].ToString() + "' And CSAL_CALID = '" + calidad+ "' And CSAL_EMPRE ='" + empresa + "'";
                                System.Data.OleDb.OleDbCommand act_stk1_rb = new System.Data.OleDb.OleDbCommand(act_stock_pri_rb, dbConn);
                                act_stk1_rb.ExecuteNonQuery();
                            }
                        }
                        //if (InsSCACSAL) { }
                        if (ActSCACSALP)
                        {
                            foreach (string[] dat_rb in scasalp)
                            {
                                string act_stock_seg_rb = "UPDATE SCACSAL SET CSAL_STKTO=CSAL_STKTO+" + dat_rb[1].ToString() + ", CSAL_SALNO=CSAL_SALNO-" + dat_rb[1].ToString() + ", CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + "-" + dat_rb[1].ToString() + " WHERE CSAL_ANO = '" + Hoy.ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + dat_rb[0].ToString() + "' And CSAL_CALID = '" + calidad+ "' And CSAL_EMPRE ='" + empresa + "' And CSAL_CPACK='" + pack + "'";
                                System.Data.OleDb.OleDbCommand act_stk2_rb = new System.Data.OleDb.OleDbCommand(act_stock_seg_rb, dbConn);
                                act_stk2_rb.ExecuteNonQuery();
                            }
                        }
                        // Se realiza RollBack del Estado en la BD si es necesario "" ROLLBACK
                        ActualizaVentas(venta["id_venta"].ToString(), "");
                        //if (InsSCACSALP) { }
                        ActualizaLogVentas(venta["id_venta"].ToString(), "Error en Actualización de DBF.", ex.Message);
                        //throw ex;
                    }
                    validavta = venta["id_venta"].ToString();
                }// fin foreach
                try
                {
                    //Se termina transacción y cerramos conexión
                    //tran.Commit();
                    if (min_week < Convert.ToInt32(Hoy.ToString("yyyy") + Convert.ToString(myCal.GetWeekOfYear(Hoy, myCWR, myFirstDOW)).PadLeft(2).Replace(" ", "0")))
                    {
                        Envia_correo();
                    }
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas("", "Error en Cierre de Conexión a DBF.", ex.Message);
                    //tran.Rollback();
                    //throw ex;
                }
            }// fin using
        }

        private static void Main()
        {
            LeerVenta exe = new LeerVenta();

            exe.ActualizaenDBF();
        }
    }
}
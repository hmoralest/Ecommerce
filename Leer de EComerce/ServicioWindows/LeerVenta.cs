using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.Configuration;


namespace ServicioWindows
{
    public class LeerVenta
    {
        static Conexion oConexion = new Conexion();

        //static string path = "D:\\Actualizar Stock Almacen con Ventas\\Tablas\\";
        static string path = ConfigurationManager.ConnectionStrings["dbf"].ConnectionString;

        static string tienda = "11"; //SQL
        public static string proceso_log = "ActStock_DBF";

        static string calidad = "1"; //DBF (producto) OK
        static string almacen = "I"; //DBF (stock producto) OK
        static string concepto_vt = "98"; //DBF (movimiento_venta) OK
        static string concepto_nc = "98"; //DBF (movimiento_notacred) OK
        static string client = "00000"; //DBF (movimiento - cliente generico)
        static string empresa = "02"; //DBF (stock producto) ok
        static string canal = "5"; //DBF (stock producto) OK
        static string cadena = "BA"; //DBF (stock producto) OK
        static string pack = "00001"; //DBF (stock producto) OK
        static string cant_pack = "0"; //DBF (movimiento) OK

        static DateTime Hoy = DateTime.Now;

        private static DataTable ListaVentas()
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
        private static DataTable ListaProductoVentas()
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

        private static void ActualizaVentas(string id_venta, string estado, string tipo)
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
                    cmd.Parameters.Add("@tipo", SqlDbType.VarChar).Value = tipo;        // NC: Nota de Credito; VT: Ventas

                    cmd.ExecuteNonQuery();

                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        public static void ActualizaLogVentas(string id_venta, string mensaje, string msje_sist, string proceso)
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
                    cmd.Parameters.Add("@proceso", SqlDbType.VarChar).Value = proceso;

                    cmd.ExecuteNonQuery();

                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        public static void Envia_correo()
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

        private static void ValidaProductoVentas()
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
                    ActualizaLogVentas("", "Error en encontrar DBF en "+path, ex.Message, proceso_log);
                    throw(ex);
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
                            ActualizaVentas(row["id"].ToString(), "E", row["tipo"].ToString());
                            ActualizaLogVentas(row["id"].ToString(), row["tipo"].ToString() + " - Producto " + row["producto"].ToString() + " no tiene registro semanal.", "Tablas SCACSAL y SCACSALP", proceso_log);
                        }
                    }
                    catch (Exception ex)
                    {
                        ActualizaLogVentas("", "Error en Validación de Productos.", ex.Message, proceso_log);
                    }
                }
                try
                {
                    //Se termina transacción y cerramos conexión
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas("", "Error en Cierre de Conexión a DBF.", ex.Message, proceso_log);
                }
            }
        }

        public static void ActualizaenDBF()
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
            bool ActSCACSALP = false;
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
                ActualizaLogVentas("", "Error en Obtención de datos de SLQ Server.", ex.Message, proceso_log);
                return;
            }

            using (System.Data.OleDb.OleDbConnection dbConn = new System.Data.OleDb.OleDbConnection(sConn))
            {
                try
                {
                    // Abro conexión
                    dbConn.Open();
                }
                catch(Exception ex)
                {
                    ActualizaLogVentas("", "Error en encontrar DBF.", ex.Message, proceso_log);
                    return;
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
                            // Se da valor al concepto que se usará en la transaccion
                            string concepto = "";
                            if (venta["tipo"].ToString() == "VT")
                            {
                                concepto = concepto_vt;
                            }
                            if (venta["tipo"].ToString() == "NC")
                            {
                                concepto = concepto_nc;
                            }
                            // Se Setean en False (Nueva Venta)
                            ActSCACODI = false;
                            InsSCCCSXN = false;
                            InsSCDRSXN = false;
                            ActSCACSAL = false;
                            ActSCACSALP = false;
                            // se limpian variables para hacer rollback
                            codigo1_ant = "";
                            codigo2_ant = "";
                            scasal.Clear();
                            scasalp.Clear();

                            //**-- CODIGO
                            //-------------------------------------
                            // Obtengo código
                            //-------------------------------------
                            string condicion = "";
                            if (venta["tipo"].ToString() == "VT")
                            {
                                condicion = " And TAB_TIPO='090' And TAB_CTAB = '004'";
                            }
                            if (venta["tipo"].ToString() == "NC")
                            {
                                condicion = " And TAB_TIPO='090' And TAB_CTAB = '004'";
                            }
                            string consulta = "SELECT TAB_CPAR2,TAB_CPAR1 FROM SCACODI WHERE TAB_EMPRE='" + empresa + "' "+condicion;
                            System.Data.OleDb.OleDbCommand con = new System.Data.OleDb.OleDbCommand(consulta, dbConn);
                            System.Data.OleDb.OleDbDataAdapter cona = new System.Data.OleDb.OleDbDataAdapter(con);
                            DataTable dt = new DataTable();
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
                            string actualiza = "UPDATE SCACODI SET TAB_CPAR1='" + codigo2 + "',TAB_CPAR2='" + codigo1 + "'  WHERE TAB_EMPRE='"+empresa+"' "+ condicion;
                            System.Data.OleDb.OleDbCommand act = new System.Data.OleDb.OleDbCommand(actualiza, dbConn);
                            act.ExecuteNonQuery();
                            ActSCACODI = true;

                            //**-- MOVIMIENTO
                            //-------------------------------------
                            // Ingreso datos de Movimiento Cabecera
                            //-------------------------------------
                            string mov_cab = "Insert Into SCCCSXN (CSXN_ALMAC, CSXN_CODIG, CSXN_FSALI, CSXN_CONCE, CSXN_ENTID, CSXN_GGUIA, CSXN_ESTAD, CSXN_TUCAL, CSXN_TUNCA, CSXN_TENTI, CSXN_TDOC1, CSXN_NDOC1, CSXN_TDOC2, CSXN_NDOC2, CSXN_SECCI, CSXN_HSALI, CSXN_PROVE, CSXN_DESTI, CSXN_AORIG, CSXN_OBSER, CSXN_EMPRE, CSXN_CANAL, CSXN_CADEN, CSXN_NMOVC, CSXN_FLASE, CSXN_UUSUA, CSXN_FUACT, CSXN_HUACT, CSXN_FTX, CSXN_FTXAQ, CSXN_TXLX, CSXN_LOG) ";
                            mov_cab += "VALUES ('" + almacen + "','" + codigo + "',CTOD('" + venta["fecha_vta"].ToString() + "'),'" + concepto + "', '" + client + "', '',''," + venta["cant_calzado"].ToString() + "," + venta["cant_no_calzado"].ToString() + ",'','" + venta["tpdoc"].ToString() + "','" + venta["nro_doc"].ToString() + "','" + venta["tp_ped"].ToString() + "','" + venta["cod_ped"].ToString() + "','" + almacen + "','" + venta["hora_vta"].ToString() + "','','','','Venta por E-Commerce Pedido: " + venta["cod_ped"].ToString() + "','" + empresa + "','" + canal + "','" + cadena + "','','','E-COM',CTOD('" + venta["fecha_reg"].ToString() + "'),'" + venta["hora_reg"].ToString() + "','','','','" + Hoy.ToString("yyyy-MM-dd HH:mm:ss") + " E-COMMERCE')";
                            System.Data.OleDb.OleDbCommand mov1 = new System.Data.OleDb.OleDbCommand(mov_cab, dbConn);
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
                        string mov_det = "Insert Into SCDRSXN (RSXN_ALMAC, RSXN_CODIG, RSXN_ARTIC, RSXN_CALID, RSXN_CALDV, RSXN_TOUNI, RSXN_CPACK, RSXN_CODPP, RSXN_PPACK, RSXN_ORDCO, RSXN_GPROV, RSXN_RMED, "+cadenaMED+", RSXN_FECHA, RSXN_EMPRE, RSXN_SECCI, RSXN_PRECI, RSXN_CODV, RSXN_PREDV, RSXN_COSDV, RSXN_CPORC, RSXN_VPORC, RSXN_FLAGC, RSXN_MERC, RSXN_CATEG, RSXN_SUBCA, RSXN_MARCA, RSXN_MERC3, RSXN_CATE3, RSXN_SUBC3, RSXN_MARC3, RSXN_FTXV, RSXN_FTX) ";
                        mov_det += "VALUES ('" + almacen + "','" + codigo + "','" + producto + "', '" + venta["calidad"].ToString() + "', '', " + Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"])) + ",'" + pack + "',''," + cant_pack + ",'','','" + venta["tipo_med"].ToString() + "'," + Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"])) + ",0,0,0,0,0,0,0,0,0,0,0,CTOD('" + venta["fecha_vta"].ToString() + "'),'" + empresa + "','" + almacen + "'," + venta["precio"].ToString() + "," + venta["costo"].ToString() + ",0.00,0.00,'',0.00,'" + venta["estandar_consig"].ToString() + "','" + venta["linea"].ToString() + "','" + venta["categ"].ToString() + "','" + venta["subcat"].ToString() + "','" + venta["marca"].ToString() + "','" + venta["rims_linea"].ToString() + "','" + venta["rims_categ"].ToString() + "','" + venta["rims_subcat"].ToString() + "','" + venta["rims_marca"].ToString() + "','','')";
                        System.Data.OleDb.OleDbCommand mov2 = new System.Data.OleDb.OleDbCommand(mov_det, dbConn);
                        mov2.ExecuteNonQuery();
                        InsSCDRSXN = true;


                        //**-- STOCK
                        //-------------------------------------
                        // Busco datos del producto en tabla 1
                        //-------------------------------------
                        string signo1 = "";
                        string signo2 = "";
                        if (venta["tipo"].ToString() == "VT")
                        {
                            signo1 = "-";       // resta stock
                            signo2 = "+";       // aumenta ventas
                        }
                        if (venta["tipo"].ToString() == "NC")
                        {
                            signo1 = "+";       // aumenta stock
                            signo2 = "-";       // resta ventas
                        }
                        string act_stock_pri="";
                                datscasal[0] = producto;
                                datscasal[1] = Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"]));
                                datscasal[2] = venta["col_med"].ToString();
                                act_stock_pri = "UPDATE SCACSAL SET CSAL_STKTO=CSAL_STKTO" + signo1 + datscasal[1] + ", CSAL_SALNO = CSAL_SALNO" + signo2 + datscasal[1] + ",CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + signo1 + datscasal[1] + " WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_CALID = '" + calidad + "' And CSAL_ARTIC='" + producto + "'";
                           // Se ejecuta sentencia
                            System.Data.OleDb.OleDbCommand act_stk1 = new System.Data.OleDb.OleDbCommand(act_stock_pri, dbConn);
                            act_stk1.ExecuteNonQuery();
                            scasal.Add(datscasal);
                            ActSCACSAL = true;
                        

                        string act_stock_seg = "";
                                datscasal[0] = producto;
                                datscasal[1] = Convert.ToString(Convert.ToInt32(venta["cant_calzado"]) + Convert.ToInt32(venta["cant_no_calzado"]));
                                datscasal[2] = venta["col_med"].ToString();
                                act_stock_seg = "UPDATE SCACSALP SET CSAL_STKTO=CSAL_STKTO" + signo1 + datscasal[1] + ", CSAL_SALNO = CSAL_SALNO" + signo2 + datscasal[1] + ",CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + venta["col_med"].ToString().PadLeft(2).Replace(" ", "0") + signo1 + datscasal[1] + " WHERE CSAL_EMPRE ='" + empresa + "' And CSAL_ANO = '" + Convert.ToDateTime(venta["fecha_vta"]).ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_CALID = '" + calidad + "' And CSAL_ARTIC='" + producto + "' And CSAL_CPACK='" + pack + "'";
                        // Se ejecuta sentencia
                            System.Data.OleDb.OleDbCommand act_stk2 = new System.Data.OleDb.OleDbCommand(act_stock_seg, dbConn);
                            act_stk2.ExecuteNonQuery();
                            scasalp.Add(datscasal);
                            ActSCACSALP = true;
                        // Termina de Actualizar y actualiza en la BD P = PROCESADO
                        ActualizaVentas(venta["id_venta"].ToString(), "P", venta["tipo"].ToString());
                    }
                    catch (Exception ex)
                    {
                        // Signo para hacer rollback, deben ser inversos
                        string signo1 = "";
                        string signo2 = "";
                        if (venta["tipo"].ToString() == "VT")
                        {
                            signo1 = "+";       // resta stock
                            signo2 = "-";       // aumenta ventas
                        }
                        if (venta["tipo"].ToString() == "NC")
                        {
                            signo1 = "-";       // aumenta stock
                            signo2 = "+";       // resta ventas
                        }
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
                                string act_stock_pri_rb = "UPDATE SCACSAL SET CSAL_STKTO=CSAL_STKTO" + signo1 + dat_rb[1].ToString() + ", CSAL_SALNO=CSAL_SALNO" + signo2 + dat_rb[1].ToString() + ", CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + signo2 + dat_rb[1].ToString() + " WHERE CSAL_ANO = '" + Hoy.ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + dat_rb[0].ToString() + "' And CSAL_CALID = '" + calidad + "' And CSAL_EMPRE ='" + empresa + "'";
                                System.Data.OleDb.OleDbCommand act_stk1_rb = new System.Data.OleDb.OleDbCommand(act_stock_pri_rb, dbConn);
                                act_stk1_rb.ExecuteNonQuery();
                            }
                        }
                        if (ActSCACSALP)
                        {
                            foreach (string[] dat_rb in scasalp)
                            {
                                string act_stock_seg_rb = "UPDATE SCACSAL SET CSAL_STKTO=CSAL_STKTO" + signo1 + dat_rb[1].ToString() + ", CSAL_SALNO=CSAL_SALNO" + signo2 + dat_rb[1].ToString() + ", CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + "=CSAL_MED" + dat_rb[2].ToString().PadLeft(2).Replace(" ", "0") + signo2 + dat_rb[1].ToString() + " WHERE CSAL_ANO = '" + Hoy.ToString("yyyy") + "' And CSAL_SEMAN = '" + weekofyear.ToString().PadLeft(2).Replace(" ", "0") + "' And CSAL_ALMAC = '" + almacen + "' And CSAL_ARTIC='" + dat_rb[0].ToString() + "' And CSAL_CALID = '" + calidad + "' And CSAL_EMPRE ='" + empresa + "' And CSAL_CPACK='" + pack + "'";
                                System.Data.OleDb.OleDbCommand act_stk2_rb = new System.Data.OleDb.OleDbCommand(act_stock_seg_rb, dbConn);
                                act_stk2_rb.ExecuteNonQuery();
                            }
                        }
                        // Se realiza RollBack del Estado en la BD si es necesario "" ROLLBACK
                        ActualizaVentas(venta["id_venta"].ToString(), "", venta["tipo"].ToString());
                        ActualizaLogVentas(venta["id_venta"].ToString(), venta["tipo"].ToString() + " - Error en Actualización de DBF.", ex.Message, proceso_log);
                    }
                    validavta = venta["id_venta"].ToString();
                }// fin foreach
                try
                {
                    //Se termina transacción y cerramos conexión
                    if (min_week < Convert.ToInt32(Hoy.ToString("yyyy") + Convert.ToString(myCal.GetWeekOfYear(Hoy, myCWR, myFirstDOW)).PadLeft(2).Replace(" ", "0")))
                    {
                        Envia_correo();
                    }
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas("", "Error en Cierre de Conexión a DBF.", ex.Message, proceso_log);
                    //tran.Rollback();
                    //throw ex;
                }
            }// fin using
        }
        
    }
}
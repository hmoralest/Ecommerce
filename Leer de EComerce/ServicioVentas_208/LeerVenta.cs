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

namespace ServicioVentas_208
{
    public class LeerVenta
    {
        static Conexion oConexion = new Conexion();
        static Conexion oConexion208 = new Conexion();

        public static string proceso_log = "ActVenta_208";

        private static DataTable ListaVentas()
        {
            DataTable result = new DataTable();

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlDataAdapter da = new SqlDataAdapter("USP_ListaVenta_208", sql);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    //da.SelectCommand.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
                    
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

        private static void ActualizaVentas(string id_venta, string estado)
        {

            using (SqlConnection sql = oConexion.getConexionSQL())
            {
                try
                {
                    sql.Open();
                    SqlCommand cmd = new SqlCommand();

                    cmd.CommandText = "USP_ActualizaVentas_208";
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

        private static void InsertarVenta208(   string validavta = "",      string entidad = "",            string tipo_doc = "",           string tipo_doc_sunat = "",
                                                string serie_doc = "",      string numero_doc = "",         string ven_fecha = "",          string ven_hora = "",
                                                string pri_nom_cli = "",    string seg_nom_cli = "",        string pri_ape_cli = "",
                                                string seg_ape_cli = "",    string dir_cli = "",            string ruc_cli = "",
                                                string telf_cli = "",       string usu_crea = "",           string cod_vend = "",           string codigo = "",
                                                string moneda = "",         string tipo_camb = "",          string doc_pago = "",           string forma_pago = "",         string fec_pago = "", 
                                                int tot_cant = 0,           decimal tot_precio_sigv = 0,    decimal tot_dcto_sigv = 0,
                                                decimal total_venta = 0,    decimal tot_igv = 0,            string articulos = "",          string tallas = "",
                                                string items = "",          string cant_artic = "",         string alm_tien = "",           string seccion = "",
                                                string prec_artic = "",     string dcto_artic = "")
        {

            using (SqlConnection sql = oConexion208.getConexionSQL208())
            {
                try
                {
                    sql.Open();
                    SqlCommand cmd = new SqlCommand();
                    string valida = validavta+", "+entidad+", "+tipo_doc+", "+tipo_doc_sunat+", "+serie_doc+", "+numero_doc+", "+ven_fecha+", "+ven_hora+", "+
                                    pri_nom_cli+", "+seg_nom_cli+", "+pri_ape_cli+", "+seg_ape_cli+", "+dir_cli+", "+ruc_cli+", "+telf_cli+", "+usu_crea + ", " +
                                    cod_vend +", "+codigo+", "+moneda+", "+tipo_camb+", "+doc_pago+", "+forma_pago+", "+fec_pago+", "+tot_cant.ToString() + ", "+tot_precio_sigv.ToString() + ", " +
                                    tot_dcto_sigv.ToString() + ", "+total_venta.ToString() + ", "+tot_igv.ToString() + ", "+articulos+", "+tallas+", "+items+", "+cant_artic+", "+alm_tien+", "+seccion + ", " +
                                    prec_artic +", "+dcto_artic;

                    cmd.CommandText = "USP_Agrega_VentaPS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sql;

                    cmd.Parameters.Add("@validavta",    SqlDbType.VarChar).Value =  validavta;
                    cmd.Parameters.Add("@entidad",      SqlDbType.VarChar).Value =  entidad;

                    cmd.Parameters.Add("@tipo_doc",     SqlDbType.VarChar).Value =  tipo_doc;
                    cmd.Parameters.Add("@tipo_doc_sunat", SqlDbType.VarChar).Value = tipo_doc_sunat;
                    cmd.Parameters.Add("@serie_doc",    SqlDbType.VarChar).Value =  serie_doc;
                    cmd.Parameters.Add("@numero_doc",   SqlDbType.VarChar).Value =  numero_doc;
                    cmd.Parameters.Add("@ven_fecha",    SqlDbType.VarChar).Value =  ven_fecha;
                    cmd.Parameters.Add("@ven_hora",     SqlDbType.VarChar).Value =  ven_hora;
                    cmd.Parameters.Add("@pri_nom_cli",  SqlDbType.VarChar).Value =  pri_nom_cli;
                    cmd.Parameters.Add("@seg_nom_cli",  SqlDbType.VarChar).Value =  seg_nom_cli;
                    cmd.Parameters.Add("@pri_ape_cli",  SqlDbType.VarChar).Value =  pri_ape_cli;
                    cmd.Parameters.Add("@seg_ape_cli",  SqlDbType.VarChar).Value =  seg_ape_cli;
                    cmd.Parameters.Add("@dir_cli",      SqlDbType.VarChar).Value =  dir_cli;
                    cmd.Parameters.Add("@ruc_cli",      SqlDbType.VarChar).Value =  ruc_cli;
                    cmd.Parameters.Add("@telf_cli",     SqlDbType.VarChar).Value =  telf_cli;
                    cmd.Parameters.Add("@usu_crea",     SqlDbType.VarChar).Value =  usu_crea;
                    cmd.Parameters.Add("@cod_vend",     SqlDbType.VarChar).Value =  cod_vend;
                    cmd.Parameters.Add("@codigo",       SqlDbType.VarChar).Value =  codigo;
                    cmd.Parameters.Add("@moneda",       SqlDbType.VarChar).Value =  moneda;
                    cmd.Parameters.Add("@tipo_camb",    SqlDbType.VarChar).Value =  tipo_camb;
                    cmd.Parameters.Add("@doc_pago",     SqlDbType.VarChar).Value =  doc_pago;
                    cmd.Parameters.Add("@forma_pago",   SqlDbType.VarChar).Value =  forma_pago;
                    cmd.Parameters.Add("@fec_pago",     SqlDbType.VarChar).Value =  fec_pago;
                    cmd.Parameters.Add("@tot_cant",     SqlDbType.Int).Value =      tot_cant;
                    cmd.Parameters.Add("@tot_precio_sigv", SqlDbType.Decimal).Value = tot_precio_sigv;
                    cmd.Parameters.Add("@tot_dcto_sigv", SqlDbType.Decimal).Value = tot_dcto_sigv;
                    cmd.Parameters.Add("@total_venta",  SqlDbType.Decimal).Value =  total_venta;
                    cmd.Parameters.Add("@tot_igv",      SqlDbType.Decimal).Value =  tot_igv;

                    cmd.Parameters.Add("@articulos",    SqlDbType.VarChar).Value =  articulos;
                    cmd.Parameters.Add("@tallas",       SqlDbType.VarChar).Value =  tallas;
                    cmd.Parameters.Add("@items",        SqlDbType.VarChar).Value =  items;
                    cmd.Parameters.Add("@cant_artic",   SqlDbType.VarChar).Value =  cant_artic;
                    cmd.Parameters.Add("@seccion",      SqlDbType.VarChar).Value =  seccion;
                    cmd.Parameters.Add("@alm_tien",     SqlDbType.VarChar).Value =  alm_tien;
                    cmd.Parameters.Add("@prec_artic",   SqlDbType.VarChar).Value =  prec_artic;
                    cmd.Parameters.Add("@dcto_artic",   SqlDbType.VarChar).Value =  dcto_artic;

                    cmd.ExecuteNonQuery();

                    sql.Close();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        public static void Actualiza208()
        {
            string validavta = "";
            string entidad = "";

            string tipo_doc = "";
            string tipo_doc_sunat = "";
            string serie_doc = "";
            string numero_doc = "";
            string ven_fecha = "";
            string ven_hora = "";
            string pri_nom_cli = "";
            string seg_nom_cli = "";
            string pri_ape_cli = "";
            string seg_ape_cli = "";
            string dir_cli = "";
            string ruc_cli = "";
            string telf_cli = "";
            string usu_crea = "";
            string cod_vend = "";
            string codigo = "";
            string moneda = "";
            string tipo_camb = "";
            string doc_pago = "";
            string forma_pago = "";
            string fec_pago = "";
            int tot_cant = 0;
            decimal tot_precio_sigv = 0;
            decimal tot_dcto_sigv = 0;
            decimal total_venta = 0;
            decimal tot_igv = 0;
            
            string articulos = "";
            string tallas = "";
            string items = "";
            string cant_artic = "";
            string seccion = "";
            string alm_tien = "";
            string prec_artic = "";
            string dcto_artic = "";

            DataTable datos = new DataTable();

            try
            {
                //Lleno datos iniciales
                datos = ListaVentas();
            }
            catch (Exception ex)
            {
                ActualizaLogVentas("", "Error en consulta al Servidor telefónica.", ex.Message, proceso_log);
                //throw ex;
            }

            using (SqlConnection sql = oConexion.getConexionSQL208())
            {
                try
                {
                    // Abro conexión
                    sql.Open();
                }
                catch(Exception ex)
                {
                    ActualizaLogVentas("", "Error en conexión al Servidor 208.", ex.Message, proceso_log);
                    //throw ex;
                }

                foreach(DataRow venta in datos.Rows)
                {
                    // variables                    
                    try
                    {
                        if (validavta != venta["vta_id"].ToString())
                        {
                            if(validavta != "")
                            {
                                // Inserta datos de venta Anterior
                                InsertarVenta208(validavta, entidad, tipo_doc, tipo_doc_sunat, serie_doc, numero_doc, ven_fecha, ven_hora,
                                                    pri_nom_cli, seg_nom_cli, pri_ape_cli, seg_ape_cli, dir_cli, ruc_cli, telf_cli, usu_crea,
                                                    cod_vend, codigo, moneda, tipo_camb, doc_pago, forma_pago, fec_pago, tot_cant, tot_precio_sigv,
                                                    tot_dcto_sigv, total_venta, tot_igv, articulos, tallas, items, cant_artic, alm_tien, seccion,
                                                    prec_artic, dcto_artic);
                                // Se actualiza estado de Venta
                                ActualizaVentas(validavta.ToString(), "P");
                            }
                            // Se Setean datos unicos de venta
                            entidad = venta["entidad"].ToString();
                            tipo_doc = venta["tipo_doc"].ToString();
                            tipo_doc_sunat = venta["tipo_doc_sunat"].ToString();
                            serie_doc = venta["serie_doc"].ToString();
                            numero_doc = venta["numero_doc"].ToString();
                            ven_fecha = venta["ven_fecha"].ToString();
                            ven_hora = venta["ven_hora"].ToString();
                            pri_nom_cli = venta["pri_nom"].ToString();
                            seg_nom_cli = venta["seg_nom"].ToString();
                            pri_ape_cli = venta["pri_ape"].ToString();
                            seg_ape_cli = venta["seg_ape"].ToString();
                            dir_cli = venta["dir_cli"].ToString();
                            ruc_cli = venta["nro_doc_cli"].ToString();
                            telf_cli = venta["telf_cli"].ToString();
                            usu_crea = venta["usu_vta"].ToString();
                            cod_vend = venta["vendedor"].ToString();
                            codigo = venta["codigo"].ToString();
                            moneda = venta["moneda"].ToString();
                            tipo_camb = venta["tipo_cambio"].ToString();
                            doc_pago = venta["doc_pago"].ToString();
                            forma_pago = venta["forma_pago"].ToString();
                            fec_pago = venta["fec_pago"].ToString();
                            tot_cant = 0;
                            tot_precio_sigv = 0;
                            tot_dcto_sigv = 0;
                            total_venta = Convert.ToDecimal(venta["total_pago"]);
                            tot_igv = Convert.ToDecimal(venta["tot_igv"]);


                            // Se Limpian variables para ingresar nueva Venta
                            articulos = "";
                            tallas = "";
                            items = "";
                            cant_artic = "";
                            alm_tien = "";
                            seccion = "";
                            prec_artic = "";
                            dcto_artic = "";
                        }
                        else
                        {
                            // Se agrega separadores
                            articulos += "|";
                            tallas += "|";
                            items += "|";
                            cant_artic += "|";
                            alm_tien += "|";
                            seccion += "|";
                            prec_artic += "|";
                            dcto_artic += "|";
                        }
                        // Se agrega otro detalle
                        articulos += venta["artic"].ToString();
                        tallas += venta["talla"].ToString();
                        items += venta["item_artic"].ToString();
                        cant_artic += venta["cant_artic"].ToString();
                        alm_tien += venta["almacen"].ToString();
                        seccion += venta["seccion"].ToString();
                        prec_artic += venta["prec_artic"].ToString();
                        dcto_artic += venta["dcto_artic_tot"].ToString();

                        tot_cant = tot_cant + Convert.ToInt32(venta["cant_artic"]);
                        tot_precio_sigv = tot_precio_sigv + (Convert.ToDecimal(venta["prec_artic"])* Convert.ToDecimal(venta["cant_artic"]));
                        tot_dcto_sigv = tot_dcto_sigv + Convert.ToDecimal(venta["dcto_artic_tot"]);
                    }
                    catch (Exception ex)
                    {
                        ActualizaLogVentas(validavta, "Error en Registro de documento.", ex.Message, proceso_log);
                        //throw ex;
                    }

                    validavta = venta["vta_id"].ToString();
                }// fin foreach
                try
                {
                    // Inserta datos de última Venta
                    InsertarVenta208(validavta, entidad, tipo_doc, tipo_doc_sunat, serie_doc, numero_doc, ven_fecha, ven_hora,
                                        pri_nom_cli, seg_nom_cli, pri_ape_cli, seg_ape_cli, dir_cli, ruc_cli, telf_cli, usu_crea,
                                        cod_vend, codigo, moneda, tipo_camb, doc_pago, forma_pago, fec_pago, tot_cant, tot_precio_sigv,
                                        tot_dcto_sigv, total_venta, tot_igv, articulos, tallas, items, cant_artic, alm_tien, seccion,
                                        prec_artic, dcto_artic);
                    // Se actualiza estado de Venta
                    ActualizaVentas(validavta.ToString(), "P");
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas(validavta, "Error en Registro de documento.", ex.Message, proceso_log);
                    //throw ex;
                }
                try
                {
                    //Cierra conexión
                    sql.Close();
                }
                catch (Exception ex)
                {
                    ActualizaLogVentas("", "Error en cierre de Conexión al servidor 208.", ex.Message, proceso_log);
                    //throw ex;
                }
            }// fin using
        }

        /*public static void Main()
        {
            Actualiza208();
        }*/
    }
}
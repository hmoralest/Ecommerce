using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bukimedia.PrestaSharp.Factories;
using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Helpers;
using Bukimedia.PrestaSharp;

namespace PrestasharpChristian
{
    class Program
    {
        /*public static string BaseUrl = "http://7UAQDKE187QTB3JT14NLQ3V3XSB6R7HR@127.0.0.1/bata/api/";
        public static string Account = "7UAQDKE187QTB3JT14NLQ3V3XSB6R7HR";
        public static string Password = "";*/

        public static string BaseUrl = "http://10.10.10.247/bata/api/";
        public static string Account = "7UAQDKE187QTB3JT14NLQ3V3XSB6R7HR";
        public static string Password = "";

        public static OrderFactory of = new OrderFactory(BaseUrl, Account, Password);
        public static CartFactory oc = new CartFactory(BaseUrl, Account, Password);
        public static OrderCarrierFactory OrderCarrierFactory = new OrderCarrierFactory(BaseUrl, Account, Password);
        public static StockAvailableFactory sf = new StockAvailableFactory(BaseUrl, Account, Password);
        public static ProductFactory pf = new ProductFactory(BaseUrl, Account, Password);
        public static OrderPaymentFactory opf = new OrderPaymentFactory(BaseUrl, Account, Password);
        public static OrderStateFactory osf = new OrderStateFactory(BaseUrl, Account, Password);


        static void Main(string[] args)
        {
            int id_orden = 37;
            string serie_guia = "URB155";
            ActualizarGuia(id_orden, serie_guia);
            Console.WriteLine(String.Format("Pedido {0}: Se actulizo correctamente la guía Nro. {1}", id_orden, serie_guia));
            Console.ReadLine();
        }

        /// <summary>
        /// Procedimiento de Pedido: Actualizar Nro de Guia por Prestashop WebService
        /// </summary>
        /// <param name="id_orden">Id de Orden</param>
        /// <param name="serie_guia">Numero de Guia</param>
        /// <returns>Verdadero/Falso</returns>
        //public static bool ActualizarGuia(int id_orden, string serie_guia)
        //{
        //    bool result = false;
        //    try
        //    {
        //        OrderFactory of = new OrderFactory(BaseUrl, Account, Password);

        //        // Actualizar Orden
        //        order orden = of.Get(id_orden);
        //        orden.shipping_number = serie_guia;
        //        orden.current_state = 4; // Pedido Enviado (Shipped)
        //        of.Update(orden);

        //        // Actualizar Orden_Carrier
        //        // Filtro
        //        Dictionary<string, string> dtn = new Dictionary<string, string>();
        //        dtn.Add("id_order", id_orden.ToString());

        //        OrderCarrierFactory ocf = new OrderCarrierFactory(BaseUrl, Account, Password);
        //        order_carrier orden_trans = ocf.GetByFilter(dtn,null,null).FirstOrDefault();
        //        if (orden_trans != null)
        //        {
        //            orden_trans.tracking_number = serie_guia;
        //            ocf.Update(orden_trans);
        //            //Console.WriteLine(String.Format("{0}, {1}, {2}, {3}", orden_trans.id, orden_trans.id_carrier, orden_trans.id_order, orden_trans.id_order_invoice));
        //            //Console.ReadLine();
        //        }
        //        result = true;
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //    }
        //    return result;
        //}


        
        public static bool ActualizarGuia(int id_orden, string serie_guia)
        {
            bool result = false;
            int ID_CLIENTE = 20;
            int ID_PRODUCTO_1 = 8092950;
            int ID_PRODUCTO_2 = 8899998;
            int STOCK_ACTUAL = 10;
            int CANTIDAD_COMPRA = 2;
            int DIRECCION_ENVIO = 30;
            int ID_TRANSPORTISTA = 16;

            for (int i = 1; i <= 200; i++)
            {

                try
                {
                    // Obtener los productos
                    Dictionary<string, string> dtn = new Dictionary<string, string>();
                    dtn.Add("id", ID_PRODUCTO_1.ToString());
                    product pro1 = pf.GetByFilter(dtn, null, null).FirstOrDefault();


                    // Agregar Stock a Producto = 10
                    dtn = new Dictionary<string, string>();
                    dtn.Add("id_product", pro1.id.ToString());
                    dtn.Add("id_product_attribute", pro1.associations.combinations.FirstOrDefault().id.ToString());
                    stock_available osa = sf.GetByFilter(dtn, null, null).FirstOrDefault();
                    osa.quantity = STOCK_ACTUAL;
                    sf.Update(osa);

                    dtn = new Dictionary<string, string>();
                    dtn.Add("id", ID_PRODUCTO_2.ToString());
                    product pro2 = pf.GetByFilter(dtn, null, null).FirstOrDefault();

                    dtn = new Dictionary<string, string>();
                    dtn.Add("id_product", pro2.id.ToString());
                    dtn.Add("id_product_attribute", pro2.associations.combinations.FirstOrDefault().id.ToString());
                    osa = sf.GetByFilter(dtn, null, null).FirstOrDefault();
                    osa.quantity = STOCK_ACTUAL;
                    sf.Update(osa);

                    // Agregar Carrito
                    cart carrito = new cart();
                    carrito.id_shop_group = 1;
                    carrito.id_shop = 1;
                    carrito.id_carrier = ID_TRANSPORTISTA;
                    carrito.delivery_option = "a:1:{i:13;s:3:\"16,\";}";
                    carrito.id_lang = 1;
                    carrito.id_address_delivery = 30;
                    carrito.id_address_invoice = 30;
                    carrito.id_currency = 1;
                    carrito.id_customer = ID_CLIENTE;
                    carrito.id_guest = 254;

                    // Agregar Detalle Carrito
                    cart_row cr = new cart_row();
                    cr.id_product = ID_PRODUCTO_1;
                    cr.id_product_attribute = pro1.associations.combinations.FirstOrDefault().id;
                    cr.quantity = CANTIDAD_COMPRA;
                    cr.id_address_delivery = DIRECCION_ENVIO;
                    carrito.associations.cart_rows.Add(cr);

                    cr = new cart_row();
                    cr.id_product = ID_PRODUCTO_2;
                    cr.id_product_attribute = pro2.associations.combinations.FirstOrDefault().id;
                    cr.quantity = CANTIDAD_COMPRA;
                    cr.id_address_delivery = DIRECCION_ENVIO;
                    carrito.associations.cart_rows.Add(cr);

                    carrito = oc.Add(carrito);

                    // Agregar Orden
                    order orden = new order();
                    orden.id_shop_group = 1;
                    orden.id_shop = 1;
                    orden.id_carrier = ID_TRANSPORTISTA;
                    orden.id_lang = 1;
                    orden.id_customer = ID_CLIENTE;
                    orden.id_cart = carrito.id;
                    orden.id_currency = 1;
                    orden.id_address_delivery = DIRECCION_ENVIO;
                    orden.id_address_invoice = DIRECCION_ENVIO;
                    orden.current_state = 2;
                    orden.payment = "Cash on delivery (COD)";
                    orden.conversion_rate = 1.0M;
                    orden.module = "cashondelivery";
                    orden.recyclable = 0;
                    orden.gift = 0;
                    orden.gift_message = "";
                    orden.mobile_theme = 0;
                    orden.shipping_number = "";
                    orden.total_discounts = 0M;
                    orden.total_discounts_tax_incl = 0M;
                    orden.total_discounts_tax_excl = 0M;
                    orden.total_paid = 363.400M;
                    orden.total_paid_tax_incl = 363.400M;
                    orden.total_paid_tax_excl = 290.713M;
                    orden.total_paid_real = 0M;
                    orden.total_products = 284.413M;
                    orden.total_products_wt = 115.900M;
                    orden.total_shipping = 0M;
                    orden.total_shipping_tax_incl = 0M;
                    orden.total_shipping_tax_excl = 0M;
                    orden.carrier_tax_rate = 18M;
                    orden.total_wrapping = 0M;
                    orden.total_wrapping_tax_incl = 0M;
                    orden.total_wrapping_tax_excl = 0M;
                    orden.invoice_number = 0;
                    orden.delivery_number = 11;
                    orden.valid = 1;
                    orden.date_add = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    orden.date_upd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    order_row or = new order_row();
                    or.product_id = pro1.id;
                    or.product_attribute_id = pro1.associations.combinations.FirstOrDefault().id;
                    or.product_ean13 = "";
                    or.product_name = pro1.description.ToString();
                    or.product_quantity = 1;
                    or.product_price = 46.525424M;
                    or.product_reference = pro1.reference;
                    or.product_upc = "";
                    or.unit_price_tax_excl = 37.203391M;
                    or.unit_price_tax_incl = 43.900001M;
                    orden.associations.order_rows.Add(or);

                    or = new order_row();
                    or.product_id = pro2.id;
                    or.product_attribute_id = pro2.associations.combinations.FirstOrDefault().id;
                    or.product_ean13 = "";
                    or.product_name = pro2.description.ToString();
                    or.product_quantity = 1;
                    or.product_price = 76.186441M;
                    or.product_reference = pro2.reference;
                    or.product_upc = "";
                    or.unit_price_tax_excl = 60.932204M;
                    or.unit_price_tax_incl = 71.900001M;
                    orden.associations.order_rows.Add(or);

                    order ok = new order();
                    ok = of.Add(orden);

                    ok.current_state = 2;
                    of.Update(ok);

                    /*
                     Dictionary<string, string> dtn = new Dictionary<string, string>();
                     dtn.Add("id", id_orden.ToString());

                     OrderFactory ocf = new OrderFactory(BaseUrl, Account, Password);
                     order o = ocf.GetByFilter(dtn, null, null).FirstOrDefault();

                     of.Add(o);
                     */

                    result = true;
                    Console.WriteLine(String.Format("Se agrego la Orden Nro {0}", ok.id));

                }
                catch (PrestaSharpException ex)
                {
                    result = false;
                    Console.Write(ex.Message);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    result = false;
                    Console.Write(ex.Message);
                    Console.ReadKey();
                }


            }


            
            return result;
        }


    }
}

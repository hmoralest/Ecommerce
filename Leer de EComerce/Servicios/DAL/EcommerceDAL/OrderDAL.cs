using System.Collections.Generic;

using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;

using Servicios.Configuracion;
using System;
using System.Linq;

namespace Servicios.DAL.EcommerceDAL
{
    public class OrderDAL
    {
        OrderFactory orderFactory = new OrderFactory(Configuracion.Ecommerce.BaseURL, Configuracion.Ecommerce.Account, Configuracion.Ecommerce.Password);
        OrderPaymentFactory orderPaymentFactory = new OrderPaymentFactory(Configuracion.Ecommerce.BaseURL, Configuracion.Ecommerce.Account, Configuracion.Ecommerce.Password);

        public List<order> ListaPedidos()
        {
            List<order> orders = new List<order>();
            try
            {
                orders = orderFactory.GetAll();
                return orders;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<order_payment> ListaPagos()
        {
            List<order_payment> payments = new List<order_payment>();
            try
            {
                payments = orderPaymentFactory.GetAll();
                return payments;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<order> ListaPedidosPagados()
        {
            List<order> orders = new List<order>();

            //filtros para pedidos pagados (estado 2 o 12)
            Dictionary<string, string> dtn = new Dictionary<string, string>();
            //dtn.Add("current_state", "12");
            dtn.Add("current_state", "2");

            try
            {
                orders = orderFactory.GetByFilter(dtn, "id_ASC", null);
                return orders;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public order_payment GetPayment(string id)
        {
            order_payment payment = new order_payment();
            List<order_payment> todos = new List<order_payment>();
            try
            {
                todos = orderPaymentFactory.GetAll();
                
                todos.Select(x => (x.order_reference)).ToList();

                foreach (order_payment uno in todos)
                {
                    if (id == uno.order_reference)
                        payment = uno;
                }
                
                return payment;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //public List<order_row> ListaProductosOrden()
        //{
        //    List<order_row> orderRows = new List<order_row>();

        //    //filtros para pedidos pagados (estado 2 o 12)
        //    Dictionary<string, string> dtn = new Dictionary<string, string>();
        //    dtn.Add("current_state", "2");

        //    try
        //    {
        //        orderRows = orderFactory.GetByFilter(dtn, "id_ASC", null);
        //        return orderRows;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}

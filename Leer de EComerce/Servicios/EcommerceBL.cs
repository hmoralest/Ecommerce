using System;
using System.Collections.Generic;

using Bukimedia.PrestaSharp.Entities;

using Servicios.DAL.EcommerceDAL;

namespace Servicios.Ecommerce
{
    public class EcommerceBL
    {
        CustomerDAL oCustomer = new CustomerDAL();
        OrderDAL oOrder = new OrderDAL();
        ProductDAL oProducto = new ProductDAL();
        StockDAL oStock = new StockDAL();
        AddressDAL oAddress = new AddressDAL();

        public List<customer> ListaClientes()
        {
            return oCustomer.ListaClientes();
        }

        /*public List<customer> ListaClientesNuevos()
        {
            return oCustomer.ListaClientesNuevos();
        }*/

        /*public List<customer> ListaClientesRegistrados()
        {
            return oCustomer.ListaClientesRegistrados();
        }*/

        public customer GetCliente(int id)
        {
            return oCustomer.GetCliente(id);
        }

        public List<order> ListaPedidosPagados()
        {
            return oOrder.ListaPedidosPagados();
        }

        public List<address> ListaDirecciones()
        {
            return oAddress.ListaDirecciones();
        }

        public address GetDireccion(int id)
        {
            return oAddress.GetDireccion(id);
        }

        public order_payment GetPayment(string id)
        {
            return oOrder.GetPayment(id);
        }

        public List<order> ListaPedidos()
        {
            return oOrder.ListaPedidos();
        }

        public List<order_payment> ListaPagos()
        {
            return oOrder.ListaPagos();
        }

        public List<product> ListaProductos()
        {
            return oProducto.ListaProductos();
        }

        public List<stock_available> ListaStocks()
        {
            return oStock.ListaStocks();
        }

        //public List<order_row> ListaProductosOrden()
        //{
        //    return oOrder.ListaProductosOrden();
        //}
    }
}

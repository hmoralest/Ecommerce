using System;
using System.Collections.Generic;

using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;

using Servicios.Configuracion;

namespace Servicios.DAL.EcommerceDAL
{
    public class ProductDAL
    {
        ProductFactory productFactory = new ProductFactory(Configuracion.Ecommerce.BaseURL, Configuracion.Ecommerce.Account, Configuracion.Ecommerce.Password);

        public List<product> ListaProductos()
        {
            List<product> products = new List<product>();
            try
            {
                products = productFactory.GetAll();
                return products;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}

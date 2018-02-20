using System.Collections.Generic;

using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;

using Servicios.Configuracion;

namespace Servicios.DAL.EcommerceDAL
{
    public class StockDAL
    {
        StockAvailableFactory stockFactory = new StockAvailableFactory(Configuracion.Ecommerce.BaseURL, Configuracion.Ecommerce.Account, Configuracion.Ecommerce.Password);

        public List<stock_available> ListaStocks()
        {
            List<stock_available> stocks = new List<stock_available>();
            try
            {
                stocks = stockFactory.GetAll();
                return stocks;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}

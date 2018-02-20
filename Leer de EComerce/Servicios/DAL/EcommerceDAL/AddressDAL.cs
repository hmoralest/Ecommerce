using System.Collections.Generic;

using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;

using Servicios.Configuracion;

namespace Servicios.DAL.EcommerceDAL
{
    public class AddressDAL
    {
        AddressFactory addressFactory = new AddressFactory(Configuracion.Ecommerce.BaseURL, Configuracion.Ecommerce.Account, Configuracion.Ecommerce.Password);

        public List<address> ListaDirecciones()
        {
            List<address> addresses = new List<address>();
            try
            {
                addresses = addressFactory.GetAll();
                return addresses;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public address GetDireccion(int id)
        {
            address address = new address();
            try
            {
                address = addressFactory.Get(id);
                return address;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}

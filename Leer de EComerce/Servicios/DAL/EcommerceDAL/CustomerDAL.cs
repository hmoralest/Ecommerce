using System.Collections.Generic;

using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;

using Servicios.Configuracion;

using System.Linq;

namespace Servicios.DAL.EcommerceDAL
{
    public class CustomerDAL
    {
        CustomerFactory customerFactory = new CustomerFactory(Configuracion.Ecommerce.BaseURL, Configuracion.Ecommerce.Account, Configuracion.Ecommerce.Password);


        public List<customer> ListaClientes()
        {
            List<customer> customers = new List<customer>();
            try
            {
                customers = customerFactory.GetAll();
                return customers;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public customer GetCliente(int id)
        {
            customer customer = new customer();
            try
            {
                customer = customerFactory.Get(id);
                return customer;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /*public List<customer> ListaClientesNuevos()
        {
            List<customer> customers = new List<customer>();

            List<ClienteEL> clientes = new List<ClienteEL>();
            ClienteDAL oCliente = new ClienteDAL();

            try
            {
                customers = customerFactory.GetAll();
                clientes = oCliente.ListaCliente();
                
                var email = clientes.Select(x => (x.Email)).ToList();

                List<customer> customerMod = new List<customer>(customers);

                foreach (customer customer in customerMod)
                {
                    if (email.Contains(customer.email))
                        customers.Remove(customer);
                }

                return customers;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }*/

        /*public List<customer> ListaClientesRegistrados()
        {
            List<customer> customers = new List<customer>();

            List<ClienteEL> clientes = new List<ClienteEL>();
            ClienteDAL oCliente = new ClienteDAL();

            try
            {
                customers = customerFactory.GetAll();
                clientes = oCliente.ListaCliente();

                var email = clientes.Select(x => (x.Email)).ToList();

                List<customer> customerMod = new List<customer>(customers);

                foreach (customer customer in customerMod)
                {
                    if (!email.Contains(customer.email))
                        customers.Remove(customer);
                }

                return customers;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }*/
    }
}

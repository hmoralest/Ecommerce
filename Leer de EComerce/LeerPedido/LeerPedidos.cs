using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Servicios.Ecommerce;
using Bukimedia.PrestaSharp.Entities;
using System.Data;

namespace LeerPedidos
{
    class LeerPedido
    {
        EcommerceBL oEcommerce = new EcommerceBL();

        List<order> pedidosEcommerce = new List<order>();

        public  List<order> ListaPedidosPagados()
        {
                return oEcommerce.ListaPedidosPagados();
        }

        public customer GetCliente(int id)
        {
                return oEcommerce.GetCliente(id);
        }

        public address GetDireccion(int id)
        {
                return oEcommerce.GetDireccion(id);
        }

        public order_payment GetPayment(string reference)
        {
                return oEcommerce.GetPayment(reference);
        }

        public DataTable PrepararPedidos()
        {
            DataTable Prestashop = new DataTable("Ordenes");
            DataColumn column;
            DataRow row;
            pedidosEcommerce = ListaPedidosPagados();

            customer cliente = new customer();
            address direccion_cli = new address();
            address direccion_ent = new address();
            order_payment pago = new order_payment();

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ped_id";
            column.Caption = "ped_id";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ped_ref";
            column.Caption = "ped_ref";
            Prestashop.Columns.Add(column);
            
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ped_fecha";
            column.Caption = "ped_fecha";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ped_ubigeo_ent";
            column.Caption = "ped_ubigeo_ent";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ped_dir_ent";
            column.Caption = "ped_dir_ent";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "ped_total_sigv";
            column.Caption = "ped_total_sigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "ped_total_cigv";
            column.Caption = "ped_total_cigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "ped_dcto_sigv";
            column.Caption = "ped_dcto_sigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "ped_dcto_cigv";
            column.Caption = "ped_dcto_cigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "ped_ship_sigv";
            column.Caption = "ped_ship_sigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Decimal");
            column.ColumnName = "ped_ship_cigv";
            column.Caption = "ped_ship_cigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "cli_id";
            column.Caption = "cli_id";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_nombres";
            column.Caption = "cli_nombres";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_apellidos";
            column.Caption = "cli_apellidos";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_fec_nac";
            column.Caption = "cli_fec_nac";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_email";
            column.Caption = "cli_email";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_ubigeo";
            column.Caption = "cli_ubigeo";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_direc";
            column.Caption = "cli_direc";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_telf";
            column.Caption = "cli_telf";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "cli_dni";
            column.Caption = "cli_dni";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "det_artic";
            column.Caption = "det_artic";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "det_artic_ref";
            column.Caption = "det_artic_ref";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "det_desc_artic";
            column.Caption = "det_desc_artic";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "det_cant";
            column.Caption = "det_cant";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "det_prec_sigv";
            column.ColumnName = "det_prec_sigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "det_dcto_sigv";
            column.ColumnName = "det_dcto_sigv";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "pag_metodo";
            column.ColumnName = "pag_metodo";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "pag_nro_trans";
            column.ColumnName = "pag_nro_trans";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "pag_nro_tarj";
            column.ColumnName = "pag_nro_tarj";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "pag_fecha";
            column.ColumnName = "pag_fecha";
            Prestashop.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "pag_monto";
            column.ColumnName = "pag_monto";
            Prestashop.Columns.Add(column);

            foreach (order orden in pedidosEcommerce)
            {
                int idCliente = Convert.ToInt32((orden).id_customer);
                cliente = GetCliente(idCliente);

                int idDireccion = Convert.ToInt32((orden).id_address_delivery);
                direccion_ent = GetDireccion(idDireccion);

                int idDireccion2 = Convert.ToInt32((orden).id_address_invoice);
                direccion_cli = GetDireccion(idDireccion);

                string idPago = Convert.ToString((orden).reference);
                pago = GetPayment(idPago);

                foreach (order_row detalle in orden.associations.order_rows)
                {
                    row = Prestashop.NewRow();
                    row["ped_id"] = orden.id;
                    row["ped_ref"] = orden.reference;
                    row["ped_fecha"] = orden.date_add;
                    row["ped_ubigeo_ent"] = direccion_ent.id_state;
                    row["ped_dir_ent"] = direccion_ent.address1;
                    row["ped_total_sigv"] = orden.total_paid_tax_excl;
                    row["ped_total_cigv"] = orden.total_paid_real;
                    row["ped_dcto_sigv"] = orden.total_discounts_tax_excl;
                    row["ped_dcto_cigv"] = orden.total_discounts;
                    row["ped_ship_sigv"] = orden.total_shipping_tax_excl;
                    row["ped_ship_cigv"] = orden.total_shipping;
                    row["cli_id"] = orden.id_customer;
                    row["cli_nombres"] = cliente.firstname;
                    row["cli_apellidos"] = cliente.lastname;
                    row["cli_fec_nac"] = cliente.birthday;
                    row["cli_email"] = cliente.email;
                    row["cli_ubigeo"] = direccion_cli.id_state;
                    row["cli_direc"] = direccion_cli.address1;
                    row["cli_telf"] = direccion_cli.phone;
                    row["cli_dni"] = direccion_cli.dni;
                    row["det_artic"] = detalle.product_id;
                    row["det_artic_ref"] = detalle.product_reference;
                    row["det_desc_artic"] = detalle.product_name;
                    row["det_cant"] = detalle.product_quantity;
                    row["det_prec_sigv"] = detalle.product_price;
                    row["det_dcto_sigv"] = detalle.unit_price_tax_excl;
                    row["pag_metodo"] = pago.payment_method;
                    row["pag_nro_trans"] = pago.transaction_id;
                    row["pag_nro_tarj"] = pago.card_number;
                    row["pag_fecha"] = pago.date_add;
                    row["pag_monto"] = pago.amount;
                    Prestashop.Rows.Add(row);
                }
            }
            return Prestashop;

        }

        /*public static void Main(string[] args)
        {
            LeerPedido ped = new LeerPedido();
            DataTable imprimir = ped.PrepararPedidos();
            Console.ReadKey();
        }*/
    }
}

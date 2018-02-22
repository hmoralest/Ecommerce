using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using MySql.Data.MySqlClient;

namespace LeerPedido
{
    class TraerPedidos
    {
        MySqlConnection mysql;
        Conexion oConexionMySql = new Conexion();

        public DataTable PrepararPedidos()
        {
            mysql = oConexionMySql.getConexionMySQL();
            mysql.Open();

            DataTable Prestashop = new DataTable();

            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = mysql;
            cmd.CommandText = "USP_LISTA_PEDIDOS";

            //asignar paramentros
            cmd.Parameters.AddWithValue("estado", 2);

            MySqlDataAdapter MySqlData = new MySqlDataAdapter(cmd);
            MySqlData.Fill(Prestashop);

            return Prestashop;

        }

        public static void Main(string[] args)
        {
            TraerPedidos ped = new TraerPedidos();
            DataTable imprimir = ped.PrepararPedidos();
            Console.ReadKey();
        }
    }
}

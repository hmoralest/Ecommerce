using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using ZXing;
using System.Diagnostics;

namespace WinImpresionEtiqueta
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Objeto de Web Service de Guia Electronica - Urbano
        /// </summary>
        public static GuiaUrbano guiaUrbano = new GuiaUrbano
        {
            linea = "3",
            id_contrato = "7207",
            cod_rastreo = "1000145107001",
            cod_barra = "1000145107001",
            fech_emi_vent = "15/12/2017",
            nro_o_compra = "1024319",
            nro_guia_trans = "1254",
            nro_factura = "999999",
            cod_empresa = "20123456781",
            nom_empresa = "BATA",
            cod_cliente = "45678912",
            nom_cliente = "Jorge Solis Mc Lellan",
            nro_telf = "",
            nro_telf_mobil = "987654321",
            correo_elec = "jorge.solis@tawa.com.pe",
            dir_entrega = "Av. Lima 944",
            nro_via = "1111",
            nro_int = "2121",
            nom_urb = "",
            ubi_direc = "150120",
            ref_direc = "Alt. cdra 10 Av. San Miguel",
            peso_total = "1",
            pieza_total = "3",
            productos = new List<Producto> {
                    new Producto { cod_sku = "8451230-30", descr_sku = "Zapato de cuero negro talla 30", modelo_sku = "8451230", marca_sku = "BATA", peso_sku = "0.3", cantidad_sku = "1" },
                    new Producto { cod_sku = "8451240-42", descr_sku = "Zapato marrón talla 42", modelo_sku = "8451240", marca_sku ="BATA", peso_sku = "0.3", cantidad_sku = "1" },
                    new Producto { cod_sku = "8451250-45", descr_sku = "Zapato de gamuza azul talla 45", modelo_sku = "8451250", marca_sku = "BATA", peso_sku = "0.3", cantidad_sku = "1" },
                }
        };

        /// <summary>
        /// Cadena de Dirección IP de Servidor de Impresora
        /// </summary>
        public static string Ip = "127.0.0.1";
        /// <summary>
        /// Puerto de Comunicación a Impresora IP
        /// </summary>
        public static int Port = 9101;



        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que imprime la Etiqueta 4cm x 4cm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            //Generando código de barra
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options =
                {
                    Width = 270,
                    Height = 45,
                    Margin = 0
                }
            };

            ImprimirEtiqueta4Cmx4Cm("WYB17868551", guiaUrbano);
        }

        /// <summary>
        /// Evento que imprime la Etiqueta 70mm x 80mm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            //Generando código de barra
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options =
                {
                    Width = 270,
                    Height = 45,
                    Margin = 0
                }
            };

            ImprimirEtiqueta70Mmx38Mm("WYB17868551", guiaUrbano);
        }




        /// <summary>
        /// Metodo que  genera etiquetaen Formato 4cm x 4cm
        /// </summary>
        /// <param name="strNroGuia">Número de Guía</param>
        /// <param name="oGuia">Objeto Guía Eletronica</param>
        public void ImprimirEtiqueta4Cmx4Cm(string strNroGuia, GuiaUrbano oGuia)
        {

            // Generar Formato de Información
            string cliente = oGuia.nom_cliente;
            string empresa = oGuia.nom_empresa + " - " + oGuia.nro_o_compra;
            string direccion = oGuia.dir_entrega + " " + oGuia.nro_via + " " + oGuia.nro_int;
            string referencia = oGuia.ref_direc;
            string ubigeo = oGuia.ubi_direc;
            string texto = cliente + "\r\n" + empresa + "\r\n" + direccion + "\r\n" + referencia + "\r\n" + ubigeo;

            // Generar Código ZPL
            StringBuilder strb = new StringBuilder();
            strb.Append("^XA\n");
            strb.Append("^JMA\n");
            strb.Append("^PRC\n");
            strb.Append("^FWN\n");
            strb.Append("^BY2,,65^FS\n");
            strb.Append("^LH 15,0\n");
            strb.Append("^FO 10, 20^BCN,60,Y,N,N^FD" + strNroGuia + "^FS\n");
            strb.Append("^FO 10,120^A0,20,16^FWN^FD" + cliente + "^FS\n");
            strb.Append("^FO 10,140^A0,20,15^FWN^FD" + empresa + "^FS\n");
            strb.Append("^FO 10,160^A0,20,15^FWN^FD" + direccion + "^FS\n");
            strb.Append("^FO 10,180^A0,20,15^FWN^FD" + referencia + "^FS\n");
            strb.Append("^FO 10,200^A0,20,15^FWN^FD" + ubigeo + "^FS\n");
            strb.Append("^PQ3^FS\n");
            strb.Append("^XZ\n");

            // Imprimir ZPL
            ImprimirZPL(Ip, Port, strb.ToString());
        }


        /// <summary>
        /// Metodo que genera etiqueta en Formato 70mm x 38mm
        /// </summary>
        /// <param name="strNroGuia">Número de Guía</param>
        /// <param name="oGuia">Objeto Guía Eletronica</param>
        public void ImprimirEtiqueta70Mmx38Mm(string strNroGuia, GuiaUrbano oGuia)
        {

            // Generar Formato de Información
            string cliente = oGuia.nom_cliente;
            string empresa = oGuia.nom_empresa + " - " + oGuia.nro_o_compra;
            string direccion = oGuia.dir_entrega + " " + oGuia.nro_via + " " + oGuia.nro_int;
            string referencia = oGuia.ref_direc;
            string ubigeo = oGuia.ubi_direc;
            string texto = cliente + "\r\n" + empresa + "\r\n" + direccion + "\r\n" + referencia + "\r\n" + ubigeo;

            // Generar Código ZPL
            StringBuilder strb = new StringBuilder();
            strb.Append("^XA\n");
            strb.Append("^JMA\n");
            strb.Append("^PRC\n");
            strb.Append("^FWN\n");
            strb.Append("^BY4,,70^FS\n");
            strb.Append("^LH 25,0\n");
            strb.Append("^FO 10, 30^BCN,60,Y,N,N^FD" + strNroGuia + "^FS\n");
            strb.Append("^FO 10,160^A\"Calibri\",16,10^FWN^FD" + cliente + "^FS\n");
            strb.Append("^FO 10,185^A\"Calibri\",16,10^FWN^FD" + empresa + "^FS\n");
            strb.Append("^FO 10,210^A\"Calibri\",16,10^FWN^FD" + direccion + "^FS\n");
            strb.Append("^FO 10,235^A\"Calibri\",16,10^FWN^FD" + referencia + "^FS\n");
            strb.Append("^FO 10,260^A\"Calibri\",16,10^FWN^FD" + ubigeo + "^FS\n");
            strb.Append("^PQ3^FS\n");
            strb.Append("^XZ\n");

            // Imprimir ZPL
            ImprimirZPL(Ip, Port, strb.ToString());
        }


        /// <summary>
        /// Metodo para imprimir el código ZPL
        /// </summary>
        /// <param name="strIp">Número de IP</param>
        /// <param name="strPort">Número de Puerto</param>
        /// <param name="strZPL">Cadena en código ZPL</param>
        private void ImprimirZPL(string strIp, int strPort, string strZPL)
        {
            try
            {
                // Open connection
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(strIp, strPort);
                // Write ZPL String to connection
                System.IO.StreamWriter writer =
                new System.IO.StreamWriter(client.GetStream());
                writer.Write(strZPL);
                Debug.WriteLine(strZPL);
                writer.Flush();
                // Close Connection
                writer.Close();
                client.Close();
            }
            catch (Exception)
            {
            }
        }

    }
}

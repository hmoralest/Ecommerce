using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ZXing;
using System;

namespace ImpresionEtiquetas1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            GuiaUrbano guiaUrbano = new GuiaUrbano
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

                //Añadiendo productos
                productos = new List<Producto> {
                    new Producto { cod_sku = "8451230-30", descr_sku = "Zapato de cuero negro talla 30", modelo_sku = "8451230", marca_sku = "BATA", peso_sku = "0.3", cantidad_sku = "1" },
                    new Producto { cod_sku = "8451240-42", descr_sku = "Zapato marrón talla 42", modelo_sku = "8451240", marca_sku ="BATA", peso_sku = "0.3", cantidad_sku = "1" },
                    new Producto { cod_sku = "8451250-45", descr_sku = "Zapato de gamuza azul talla 45", modelo_sku = "8451250", marca_sku = "BATA", peso_sku = "0.3", cantidad_sku = "1" },
                }
            };

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

            //Parámetro número de Guia Urbano devuelto por webservice
            Bitmap barCode = writer.Write("WYB17868551");
            //barCode.Save("barcode.png", ImageFormat.Png);

            //Generando texto para código de barra
            string empresa = guiaUrbano.nom_empresa + " - " + guiaUrbano.nro_o_compra;
            string cliente = guiaUrbano.nom_cliente;
            string direccion = guiaUrbano.dir_entrega + " " + guiaUrbano.nro_via + " " + guiaUrbano.nro_int;
            string referencia = guiaUrbano.ref_direc;
            string ubigeo = guiaUrbano.ubi_direc;
            string texto = cliente + "\r\n" + empresa + "\r\n" + direccion + "\r\n" + referencia + "\r\n" + ubigeo;

            Bitmap textos = ConvertTextToImage(texto, "Calibri", 9, Color.White, Color.Black, 270, 80);

            //Generando etiqueta
            Bitmap etiqueta = new Bitmap(270, 140);
            using (Graphics g = Graphics.FromImage(etiqueta))
            {
                g.Clear(Color.White);
                g.DrawImage(barCode, 0, 10);
                g.DrawImage(textos, 10, 65);
            }
            //etiqueta.Save("etiqueta.jpg", ImageFormat.Jpeg);

            //
            List<Bitmap> etiquetas = new List<Bitmap>();
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);
            etiquetas.Add(etiqueta);

            Bitmap pagina = new Bitmap(810, 1120);
            using (Graphics g2 = Graphics.FromImage(pagina))
            {
                g2.Clear(Color.White);
                g2.DrawImage(etiquetas.ElementAt(0), 0, 0);
                g2.DrawImage(etiquetas.ElementAt(1), 270, 0);
                g2.DrawImage(etiquetas.ElementAt(2), 540, 0);
                g2.DrawImage(etiquetas.ElementAt(3), 0, 140);
                g2.DrawImage(etiquetas.ElementAt(4), 270, 140);
                g2.DrawImage(etiquetas.ElementAt(5), 540, 140);
                g2.DrawImage(etiquetas.ElementAt(6), 0, 280);
                g2.DrawImage(etiquetas.ElementAt(7), 270, 280);
                g2.DrawImage(etiquetas.ElementAt(8), 540, 280);
                g2.DrawImage(etiquetas.ElementAt(9), 0, 420);
                g2.DrawImage(etiquetas.ElementAt(10), 270, 420);
                g2.DrawImage(etiquetas.ElementAt(11), 540, 420);
                g2.DrawImage(etiquetas.ElementAt(12), 0, 560);
                g2.DrawImage(etiquetas.ElementAt(13), 270, 560);
                g2.DrawImage(etiquetas.ElementAt(14), 540, 560);
                g2.DrawImage(etiquetas.ElementAt(15), 0, 700);
                g2.DrawImage(etiquetas.ElementAt(16), 270, 700);
                g2.DrawImage(etiquetas.ElementAt(17), 540, 700);
                g2.DrawImage(etiquetas.ElementAt(18), 0, 840);
                g2.DrawImage(etiquetas.ElementAt(19), 270, 840);
                g2.DrawImage(etiquetas.ElementAt(20), 540, 840);
                g2.DrawImage(etiquetas.ElementAt(21), 0, 980);
                g2.DrawImage(etiquetas.ElementAt(22), 270, 980);
                g2.DrawImage(etiquetas.ElementAt(23), 540, 980);
            }
            pagina.Save("pagina.png", ImageFormat.Png);
        }

        public static Bitmap ConvertTextToImage(string txt, string fontname, int fontsize, Color bgcolor, Color fcolor, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                Font font = new Font(fontname, fontsize);
                graphics.FillRectangle(new SolidBrush(bgcolor), 0, 0, bmp.Width, bmp.Height);
                graphics.DrawString(txt, font, new SolidBrush(fcolor), 0, 0);
                graphics.Flush();
                font.Dispose();
                graphics.Dispose();
            }
            return bmp;
        }


        /*public static void ImprimirEtiqueta(string strNroGuia, GuiaUrbano oGuia)
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
            strb.Append("^BY4,,70^FS\n");
            strb.Append("^LH 25,0\n");
            strb.Append("^FO 10, 30^BCN,60,Y,N,N^FD" + strNroGuia + "^FS\n");
            strb.Append("^FO 10,160^A0,16,10^FWN^FD" + cliente + "^FS\n");
            strb.Append("^FO 10,185^A0,16,10^FWN^FD" + empresa + "^FS\n");
            strb.Append("^FO 10,210^A0,16,10^FWN^FD" + direccion + "^FS\n");
            strb.Append("^FO 10,235^A0,16,10^FWN^FD" + referencia + "^FS\n");
            strb.Append("^FO 10,260^A0,16,10^FWN^FD" + ubigeo + "^FS\n");
            strb.Append("^PQ3^FS\n");
            strb.Append("^XZ\n");

            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = new PrinterSettings();
            //pd.Document = 
            if (DialogResult.OK == pd.ShowDialog(this))
            {
                RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, s);
            }


        }*/


    }
}

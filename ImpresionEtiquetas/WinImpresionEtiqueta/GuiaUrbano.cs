using System.Collections.Generic;

namespace WinImpresionEtiqueta
{
    /// <summary>
    /// Clase para generación de la Guía Electrónica de Urbano
    /// </summary>
    public class GuiaUrbano
    {
        //Grupo 1: Identificador del servicio
        public string linea { get; set; } //Linea, "3", dato de proveedor logístico
        public string id_contrato { get; set; } //ID de contrato, 7207, dato de proveedor logístico

        //Grupo 2: Identificador del envio
        public string cod_rastreo { get; set; } //Codigo de rastreo, # de orden de Prestashop
        public string cod_barra { get; set; } //Codigo de barra, # de orden de Prestashop
        public string fech_emi_vent { get; set; } //Fecha de emisión de venta
        public string nro_o_compra { get; set; } //# de orden de compra
        public string nro_guia_trans { get; set; } //# de guía de transporte
        public string nro_factura { get; set; } //# de factura


        //Grupo 3: Datos del vendedor --> BATA
        public string cod_empresa { get; set; }  //RUC BATA
        public string nom_empresa { get; set; } //BATA - Emcomer S.A.
        
        //Grupo 4: Datos del receptor
        public string cod_cliente { get; set; } //DNI - RUC cliente
        public string nom_cliente { get; set; } //Nombre de cliente
        public string nro_telf { get; set; } //# de teléfono de cliente
        public string nro_telf_mobil { get; set; } //# de celular de cliente
        public string correo_elec { get; set; } //Email de cliente

        //Grupo 5: Direccion para entrega
        public string dir_entrega { get; set; } // Dirección de entrega
        public string nro_via { get; set; } //# de vía
        public string nro_int { get; set; } //# de interior
        public string nom_urb { get; set; } //Nombre de urbanización
        public string ubi_direc { get; set; } //Ubigeo dirección entrega
        public string ref_direc { get; set; } //Referencia dirección entrega

        //Grupo 6: Datos del producto a entregar
        public List<Producto> productos { get; set; } //Lista de productos

        //Grupo 7: Datos para despachos
        public string peso_total { get; set; } //Peso total, 0.3g por defecto para cada par
        public string pieza_total { get; set; } //# de bultos
    }
}

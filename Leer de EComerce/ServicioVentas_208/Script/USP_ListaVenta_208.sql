If Exists(Select * from sysobjects Where name = 'USP_ListaVenta_208' And type = 'P')
	Drop Procedure USP_ListaVenta_208
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 12-03-2018
-- Description	: Lista Ventas Realizadas en PS para envío a BD 208
-- =====================================================================
/*
	Exec USP_ListaVenta_208 
*/

CREATE Procedure USP_ListaVenta_208
AS 
BEGIN

	Declare	@entidad	Varchar(10),
			@almacen	Varchar(4);

	Set	@entidad = '50005'
	Set	@almacen = '0000'
	
	Select top 1000
			@entidad As entidad,
			vta.Ven_Id As vta_id,
			CASE SUBSTRING(vta.Ven_Id,1,1)
				WHEN 'B'	THEN '03'
				WHEN 'F'	THEN '01'
							ELSE '00'
			END		As tipo_doc,
			CASE SUBSTRING(vta.Ven_Id,1,1)
				WHEN 'B'	THEN '03'
				WHEN 'F'	THEN '01'
							ELSE '00'
			END		As tipo_doc_sunat,
			SUBSTRING(vta.Ven_Id,1,4) As serie_doc,
			SUBSTRING(vta.Ven_Id,5,8) As numero_doc,
			Replace(Convert(Varchar,vta.Ven_Fecha,102),'.','') As ven_fecha,
			CONVERT(varchar,vta.Ven_Fecha,108) As ven_hora,
			isnull(cli.Bas_Primer_Nombre, '') As pri_nom,
			isnull(cli.bas_segundo_nombre, '') As seg_nom,
			isnull(cli.Bas_Primer_Apellido, '') As pri_ape,
			isnull(cli.Bas_Segundo_Apellido, '') As seg_ape,
			isnull(cli.Bas_Direccion,'') As dir_cli,
			isnull(cli.Bas_Telefono,'') As telf_cli,
			isnull(cli.Bas_Documento,'') As nro_doc_cli,
			@almacen	As almacen,
			'01' As moneda,
			'1' As tipo_cambio,
			'1' As form_pag,
			'usu'	As usu_vta,
			'vend' As vendedor,
			vta.Ven_Cod_Hash	As codigo,
			vta.Ven_Igv	As tot_igv,
			vta.Ven_ComisionP As dcto_artic,	--dcto
			det.Ven_Det_Item	As item_artic,
			det.Ven_Det_Cantidad	As cant_artic,
			'5'	As seccion,
			det.Ven_Det_ArtId	As artic,
			det.Ven_Det_TalId	As talla,
			det.Ven_Det_Precio	As prec_artic,
			det.Ven_Det_ComisionM As dcto_artic_tot, -- dcto
			doc_pag.Doc_Tra_ConId	As forma_pago,	/*Falta equivalencia en Forma de Pago*************************/
			CASE WHEN (LTRIM(RTRIM(ISNULL(doc_pag.Doc_Bin_NumTar,''))))= '' THEN doc_pag.trans2
				ELSE LTRIM(RTRIM(ISNULL(doc_pag.Doc_Bin_NumTar,'')))
				END		As doc_pago,
			doc_pag.Doc_Tra_SubTotal	As total_pago, 
			Replace(Convert(Varchar,doc_pag.Doc_Tra_Fecha,102),'.','')	As fec_pago,
			doc_pag.Doc_Tra_GruId	As gru_trans
	From Venta vta
		Inner Join Venta_Detalle det
			On vta.Ven_Id = det.Ven_Det_Id
		Inner Join Basico_Dato cli
			On Ven_BasId = cli.Bas_Id
		Inner Join (Select	d1.Doc_Tra_Doc, d2.Doc_Tra_ConId, d2.Doc_Tra_SubTotal, d2.Doc_Tra_Fecha,
							d1.Doc_Tra_GruId, d2.Doc_Tra_Doc As trans2, d2.Doc_Bin_NumTar
					From Documento_Transaccion d1
					Inner Join Documento_Transaccion  d2
						On d1.Doc_Tra_GruId = d2.Doc_Tra_GruId
					Where d1.Doc_Tra_Doc <> d2.Doc_Tra_Doc) doc_pag
			On vta.Ven_Id = doc_pag.Doc_Tra_Doc
	Where 1=1
	  And isnull(vta.Ven_EstAct_Vta,'') NOT IN ('P')
	Order by vta.Ven_Fecha desc

			/*select * from concepto where Con_Id = '9F'
Select * from [dbo].[Documento_Transaccion]
Select * from Documento_Transaccion where Doc_Tra_GruId = '32093'*/

/*Select d1.Doc_Tra_Doc, d2.Doc_Tra_ConId,d2.Doc_Tra_SubTotal, d2.Doc_Tra_Fecha ,d1.Doc_Tra_GruId
from Documento_Transaccion d1
Inner Join Documento_Transaccion  d2
	On d1.Doc_Tra_GruId = d2.Doc_Tra_GruId
Where d1.Doc_Tra_Doc = 'B03000018802' And d1.Doc_Tra_Doc <> d2.Doc_Tra_Doc*/


END
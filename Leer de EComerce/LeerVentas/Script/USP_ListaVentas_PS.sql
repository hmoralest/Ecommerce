If Exists(Select * from sysobjects Where name = 'USP_ListaVentas_PS' And type = 'P')
	Drop Procedure USP_ListaVentas_PS
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 02-03-2018
-- Description	: Lista Ventas Realizadas en PS para envío a Almacén
-- =====================================================================
/*
	Exec USP_ListaVentas_PS '11'
*/

CREATE Procedure USP_ListaVentas_PS(
	/*@moneda		Varchar(3),
	@seccion	Varchar(1),	/*retail (5); no retail (6)*/*/
	@tienda		Varchar(1)
)
AS 
BEGIN

	/*Select  
			vta.Ven_Id									As id_venta,
			CASE WHEN LEFT(vta.Ven_Id) = 'F' THEN 'FAC'
				 WHEN LEFT(vta.Ven_Id) = 'B' THEN 'BOL'
			END										As tpdoc,
			vta.Ven_Id									As nro_doc,
			'PED'										As tp_ped,
			'SQE1E21'									As cod_ped,
			Convert(varchar,vta.Ven_Fecha,103)			As fecha_vta,
			replace(substring(convert(varchar,vta.Ven_Fecha,108),1,5),':','')			As hora_vta,
			convert(varchar,getdate(),103)			As fecha_reg,
			replace(substring(convert(varchar,getdate(),108),1,5),':','')			As hora_reg,
			det.Ven_Det_ArtId							As product_id,
			'1'											As calidad,
			CASE left(det.Ven_Det_ArtId,1) WHEN '9' THEN det.Ven_Det_Cantidad
			ELSE 0 END					As cant_no_calzado,
			CASE left(det.Ven_Det_ArtId,1) WHEN '9' THEN 0
			ELSE det.Ven_Det_Cantidad END					As cant_calzado,
			det.Ven_Det_Precio					As precio,
			det.Ven_Det_Costo					As costo,
			det.Ven_Det_ComisionM				As comision,
			art.Art_Gru_Talla				As tipo_med,
			gru.Gru_Tal_Col				As col_med,
			''						As estandar_consig,
			''						As linea,
			art.Art_Mar_Id						As marca,
			scat.Sca_CodCat						As categ,
			scat.Sca_CodSubCat					As subcat,
			''						As rims_linea,
			''						As rims_categ,
			''						As rims_subcat,
			''						As rims_marca
	From Venta vta
		Inner Join Venta_Detalle det
			On vta.Ven_Id = det.Ven_Det_Id
		Inner Join Articulo art
			On det.Ven_Det_ArtId = art.Art_Id
		Inner Join Grupo_Talla gru
			On gru.Gru_Tal_Id = art.Art_Gru_Talla And gru.Gru_Tal_TalId = det.Ven_Det_TalId
		Inner Join SubCategoria scat
			On art.Art_SubCat_Id = scat.Sca_Id
	Where vta.Ven_Alm_Id = @tienda
	  And isnull(vta.Ven_EstAct_Alm,'')<>'P'*/

	Select
		'66800003794'									As id_Venta,
			'FAC'										As tpdoc,
			'00000001'									As nro_doc,
			'PED'										As tp_ped,
			'SQE1E21'									As cod_ped,
		'17/07/2014'			As fecha_vta,
		'1539'			As hora_vta,
		'05/03/2018'			As fecha_reg,
		'1500'			As hora_reg,
		'5814925'							As product_id,
			'1'											As calidad,
			2					As cant_no_calzado,
			0					As cant_calzado,
			10					As costo,
			20					As precio,
			2				As comision,
			'D'				As tipo_med,
			'4'				As col_med,
			''						As estandar_consig,
			''						As linea,
			'23'						As marca,
			'04'						As categ,
			8					As subcat,
			''						As rims_linea,
			''						As rims_categ,
			''						As rims_subcat,
			''						As rims_marca
	UNION
	Select
		'66800003794'									As id_Venta,
			'FAC'										As tpdoc,
			'00000001'									As nro_doc,
			'PED'										As tp_ped,
			'SQE1E21'									As cod_ped,
		'17/07/2014'			As fecha_vta,
		'1539'			As hora_vta,
		'05/03/2018'			As fecha_reg,
		'1500'			As hora_reg,
		'8892976'							As product_id,
			'1'											As calidad,
			0					As cant_no_calzado,
			1					As cant_calzado,
			22.5					As costo,
			94.5					As precio,
			15.5				As comision,
			'D'				As tipo_med,
			'4'				As col_med,
			''						As estandar_consig,
			''						As linea,
			'06'						As marca,
			'38'						As categ,
			'21'					As subcat,
			''						As rims_linea,
			''						As rims_categ,
			''						As rims_subcat,
			''						As rims_marca
	UNION
	Select
		'66800003794'									As id_Venta,
			'FAC'										As tpdoc,
			'00000001'									As nro_doc,
			'PED'										As tp_ped,
			'SQE1E21'									As cod_ped,
		'17/07/2014'			As fecha_vta,
		'1539'			As hora_vta,
		'05/03/2018'			As fecha_reg,
		'1500'			As hora_reg,
		'8899976'							As product_id,
			'1'											As calidad,
			1					As cant_no_calzado,
			0					As cant_calzado,
			58.07					As costo,
			97.89					As precio,
			11.61				As comision,
			'A'				As tipo_med,
			'1'				As col_med,
			''						As estandar_consig,
			''						As linea,
			'02'						As marca,
			'20'						As categ,
			'89'					As subcat,
			''						As rims_linea,
			''						As rims_categ,
			''						As rims_subcat,
			''						As rims_marca
	UNION
	Select
		'66800003795'									As id_Venta,
			'FAC'										As tpdoc,
			'00000001'									As nro_doc,
			'PED'										As tp_ped,
			'SQE1E21'									As cod_ped,
		'17/07/2014'			As fecha_vta,
		'1539'			As hora_vta,
		'05/03/2018'			As fecha_reg,
		'1500'			As hora_reg,
		'5538812'							As product_id,
			'1'											As calidad,
			1					As cant_no_calzado,
			0					As cant_calzado,
			15.22					As costo,
		39.90					As precio,
		15.96				As comision,
			'A'				As tipo_med,
			'1'				As col_med,
			''						As estandar_consig,
			''						As linea,
			'02'						As marca,
			'20'						As categ,
			'89'					As subcat,
			''						As rims_linea,
			''						As rims_categ,
			''						As rims_subcat,
			''						As rims_marca
	UNION
	Select
		'66800003795'									As id_Venta,
			'FAC'										As tpdoc,
			'00000001'									As nro_doc,
			'PED'										As tp_ped,
			'SQE1E21'									As cod_ped,
		'17/07/2014'			As fecha_vta,
		'1539'			As hora_vta,
		'05/03/2018'			As fecha_reg,
		'1500'			As hora_reg,
		'2098924'							As product_id,
			'1'											As calidad,
			0					As cant_no_calzado,
			2					As cant_calzado,
			15.22					As costo,
		41.46					As precio,
		16.58				As comision,
			'D'				As tipo_med,
			'3'				As col_med,
			''						As estandar_consig,
			''						As linea,
			'02'						As marca,
			'20'						As categ,
			'89'					As subcat,
			''						As rims_linea,
			''						As rims_categ,
			''						As rims_subcat,
			''						As rims_marca

END


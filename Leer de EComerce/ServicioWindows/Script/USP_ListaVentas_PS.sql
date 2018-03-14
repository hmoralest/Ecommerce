If Exists(Select * from sysobjects Where name = 'USP_ListaVentas_PS' And type = 'P')
	Drop Procedure USP_ListaVentas_PS
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 02-03-2018
-- Description	: Lista Ventas Realizadas en PS para envío a Almacén
-- =====================================================================
-- Ven_EstAct_Alm	-> NULL	= Creado
--					-> ''	= RollBack
--					-> 'P'	= Procesado
--					-> 'E'	= Error en Validacion
-- =====================================================================
-- Modificado por	: Henry Morales
-- Fch Modifica		: 07/03/2018
-- Asunto			: Se agregó filtro para no tomar los que no pasaron validación
-- =====================================================================
/*
	Exec USP_ListaVentas_PS '11'
*/

CREATE Procedure USP_ListaVentas_PS(
	@tienda		Varchar(5)
)
AS 
BEGIN

	Select  
			vta.Ven_Id										As id_venta,
			CASE WHEN LEFT(vta.Ven_Id,1) = 'F' THEN 'FAC'
				 WHEN LEFT(vta.Ven_Id,1) = 'B' THEN 'BOL'
			END												As tpdoc,
			vta.Ven_Id										As nro_doc,
			'PED'											As tp_ped,
			vta.Ven_Pst_Ref									As cod_ped,
			Convert(varchar,vta.Ven_Fecha,103)				As fecha_vta,
			replace(substring(convert(varchar,vta.Ven_Fecha,108),1,5),':','')			As hora_vta,
			convert(varchar,getdate(),103)					As fecha_reg,
			replace(substring(convert(varchar,getdate(),108),1,5),':','')				As hora_reg,
			det.Ven_Det_ArtId								As product_id,
			'1'												As calidad,
			CASE left(det.Ven_Det_ArtId,1) WHEN '9' THEN det.Ven_Det_Cantidad
			ELSE 0 END										As cant_no_calzado,
			CASE left(det.Ven_Det_ArtId,1) WHEN '9' THEN 0
			ELSE det.Ven_Det_Cantidad END					As cant_calzado,
			det.Ven_Det_Precio								As precio,
			det.Ven_Det_Costo								As costo,
			det.Ven_Det_ComisionM							As comision,
			art.Art_Gru_Talla								As tipo_med,
			gru.Gru_Tal_Col									As col_med,
			art.Art_Flagc									As estandar_consig,
			art.Art_Merc									As linea,
			art.Art_Mar_Id									As marca,
			scat.Sca_CodCat									As categ,
			scat.Sca_CodSubCat								As subcat,
			art.Art_Merc3									As rims_linea,
			art.Art_Cate3									As rims_categ,
			art.Art_SubC3									As rims_subcat,
			art.Art_Marc3									As rims_marca
	From Venta vta
		Inner Join Venta_Detalle det
			On vta.Ven_Id = det.Ven_Det_Id
		Inner Join Articulo art
			On det.Ven_Det_ArtId = art.Art_Id And art.Art_Sin_Stk = 0
		Inner Join Grupo_Talla gru
			On gru.Gru_Tal_Id = art.Art_Gru_Talla And gru.Gru_Tal_TalId = det.Ven_Det_TalId
		Inner Join SubCategoria scat
			On art.Art_SubCat_Id = scat.Sca_Id
	Where vta.Ven_Alm_Id = @tienda
	  And isnull(vta.Ven_EstAct_Alm,'') NOT IN ('P','E')
	  And isnull(vta.Ven_Est_Id,'') <> 'FANUL';

END
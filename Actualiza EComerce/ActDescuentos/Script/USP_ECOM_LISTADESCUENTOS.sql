/*If Exists(Select * from sysobjects Where name = 'USP_ECOM_LISTADESCUENTOS' And type = 'P')
	Drop Procedure USP_ECOM_LISTADESCUENTOS
GO*/

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 12-02-2018
-- Description	: Lista Descuentos para enviarlos a PrestaShop
-- =====================================================================
/*
	Exec USP_ECOM_LISTADESCUENTOS 'e-com'	
*/

ALTER Procedure USP_ECOM_LISTADESCUENTOS(
	@tienda		Varchar(50)
)
AS 
BEGIN

	Select	codart											As product_id,
			Replace(convert(varchar,fecini,102),'.','')		As Fecha_Ini,
			Replace(convert(varchar,fecfin,102),'.','')		As Fecha_Fin,
			montodcto										As Monto
	From Scaprom
	Where codtda = @tienda
	  And estado NOT IN ('D')
	  And calidad = 1
	  And fecfin >= GETDATE()
	  
	  /*Select Distinct 'ecom',Cast(codigo As Int) As product_id,'20180201' As Fecha_Ini, '20180228' As Fecha_Fin, 20 As Monto
	  From [BD_BataCommerce_Test].dbo.producto;*/
END
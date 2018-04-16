/*If Exists(Select * from sysobjects Where name = 'USP_ECOM_LISTADESCUENTOS' And type = 'P')
	Drop Procedure USP_ECOM_LISTADESCUENTOS
GO*/

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 12-02-2018
-- Description	: Lista Descuentos para enviarlos a PrestaShop
-- =====================================================================
/*
	Exec USP_ECOM_LISTAPRECIOS 'PEN',5,'e-com'
*/

ALTER Procedure USP_ECOM_LISTAPRECIOS(
	@moneda		Varchar(3),
	@seccion	Varchar(1),	/*retail (5); no retail (6)*/
	@tienda		Varchar(1)
)
AS 
BEGIN

	Select	b.cod_artic							As product_id,
	--		Cast((b.val_pvent1/1.18) As Decimal(18,2))	As precio1,
			Cast(b.val_pvent1 As Varchar) 		As precio1,
			Cast(b.val_pvent2 As Varchar)		As precio2,
			b.fec_vigen
	From thistprecios b
	Where b.cod_moneda = @moneda
	  And b.cod_secci = @seccion
	  And b.tip_estad = 'A'		/*Activo (A); Desactivado (D)*/
	  And b.fec_vigen = (	Select max(a.fec_vigen)
							From thistprecios a
							Where a.cod_moneda = @moneda
							  And a.cod_secci = @seccion
							  And a.tip_estad = 'A'	
							  And a.cod_artic = b.cod_artic
							  And a.fec_vigen <= GETDATE());


	  /*Select distinct Cast(Codigo As Int) As product_id,100 As precio1, 200 precio2
	  From [BD_BataCommerce_Test].dbo.[Producto]
	  UNION 
	  Select 11111111,22,33*/
END


If Exists(Select * from sysobjects Where name = 'USP_ECOM_ACTSTOCK' And type = 'P')
	Drop Procedure USP_ECOM_ACTSTOCK
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 12-02-2018
-- Description	: Lista Movimientos de Stock para enviarlos a PrestaShop
-- =====================================================================
/*
	Exec USP_ECOM_ACTSTOCK '11','16/02/2018 11:07:12','879-2903-43',1	
*/
CREATE Procedure USP_ECOM_ACTSTOCK(
	@tienda		Varchar(50),
	@mov_id		Varchar(12),
	@det_mov_id	Varchar(12)
	/*@fecha		Varchar(50),
	@producto	Varchar(32),
	@cantidad	Int*/
)
AS 
BEGIN

	Update det
	Set det.Mov_Det_EstId = 'P'
	From Movimiento mov	
		Inner Join Movimiento_Detalle det
			On mov.Mov_Id = det.Mov_Det_Id
	Where mov.Mov_AlmId = @tienda
	  /*And convert(varchar,mov.Mov_Fecha,103) +' ' + convert(varchar,mov.Mov_Fecha,108) = @fecha
	  And Substring(det.Mov_Det_ArtId,1,3)+'-'+Substring(det.Mov_Det_ArtId,4,9)+'-'+det.Mov_Det_TalId = @producto
	  And det.Mov_Det_Cantidad = @cantidad*/
	  And mov.Mov_Id = @mov_id
	  And det.Mov_Det_Items = @det_mov_id
	  And isnull(det.Mov_Det_EstId,'') <> 'P'
	  And mov.Mov_EstId = 'A';

END
GO
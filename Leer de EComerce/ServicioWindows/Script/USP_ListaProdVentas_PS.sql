If Exists(Select * from sysobjects Where name = 'USP_ListaProdVentas_PS' And type = 'P')
	Drop Procedure USP_ListaProdVentas_PS
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 07-03-2018
-- Description	: Lista Productos de Ventas realizadas en PS para validar
--				  Adicionalmente actualiza estados Errados a Normal
-- =====================================================================
/*
	Exec USP_ListaProdVentas_PS '11'
*/

CREATE Procedure USP_ListaProdVentas_PS(
	@tienda		Varchar(5)
)
AS 
BEGIN

	BEGIN TRY
		BEGIN TRAN act_Venta

		Update Venta Set Ven_EstAct_Alm = '' Where isnull(Ven_EstAct_Alm,'') IN ('E') ;
				
		COMMIT TRAN act_Venta
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN act_Venta

		DECLARE		@ErrorMessage	NVARCHAR(4000),
					@ErrorSeverity	INT,
					@ErrorState		INT; 

		SELECT		@ErrorMessage = ERROR_MESSAGE(),
					@ErrorSeverity = ERROR_SEVERITY(),
					@ErrorState = ERROR_STATE(); 

		RAISERROR (	@ErrorMessage, @ErrorSeverity, @ErrorState ); 

	END CATCH
	
	SElect 
		vta.Ven_Id								As id,
		det.Ven_Det_ArtId						As producto,
		Convert(varchar,vta.Ven_Fecha,103)		As fecha
	From Venta vta
		Inner Join Venta_Detalle det
			On vta.Ven_Id = det.Ven_Det_Id
		Inner Join Articulo art
			On det.Ven_Det_ArtId = art.Art_Id And art.Art_Sin_Stk = 0
		Inner Join Grupo_Talla gru
			On gru.Gru_Tal_Id = art.Art_Gru_Talla And gru.Gru_Tal_TalId = det.Ven_Det_TalId
		Inner Join SubCategoria scat
			On art.Art_SubCat_Id = scat.Sca_Id
	Where ltrim(rtrim(vta.Ven_Alm_Id)) = @tienda
	  And isnull(vta.Ven_EstAct_Alm,'') NOT IN ('P')
	  And isnull(vta.Ven_Est_Id,'') <> 'FANUL';

END
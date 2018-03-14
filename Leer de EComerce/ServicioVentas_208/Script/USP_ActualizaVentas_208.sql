If Exists(Select * from sysobjects Where name = 'USP_ActualizaVentas_208' And type = 'P')
	Drop Procedure USP_ActualizaVentas_208
GO

-- ==========================================================================================
-- Author		: Henry Morales
-- Create date	: 13/03/2018
-- Description	: Se Actualiza estado de las Ventas que han sido enviadas al servidor 208
-- ==========================================================================================
/*
	Exec USP_ActualizaVentas_208
*/

CREATE Procedure USP_ActualizaVentas_208(
	@id			Varchar(12),
	@estado		Varchar(1)
)
AS 
BEGIN

	BEGIN TRY
		BEGIN TRAN act_Venta

		Update Venta Set Ven_EstAct_Vta = @estado Where Ven_Id = @Id;
				
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
END
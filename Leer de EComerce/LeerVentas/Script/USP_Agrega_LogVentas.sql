If Exists(Select * from sysobjects Where name = 'USP_Agrega_LogVentas' And type = 'P')
	Drop Procedure USP_Agrega_LogVentas
GO

-- ==========================================================================================
-- Author		: Henry Morales
-- Create date	: 07/03/2018
-- Description	: Se utiliza para agregar al log los errores encontrados en la migracion de Ventas a DBF
-- ==========================================================================================
/*
	Exec USP_Agrega_LogVentas
*/

CREATE Procedure USP_Agrega_LogVentas(
	@id_venta			Varchar(12),
	@mensaje			Varchar(max),
	@sistema			Varchar(max)
)
AS 
BEGIN

	BEGIN TRY
		BEGIN TRAN act_Venta

		Insert into Log_ProcesoVentas values (getdate(), @id_venta, @mensaje, @sistema);
				
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
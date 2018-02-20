If Exists(Select * from sysobjects Where name = 'ECOM_ACTLOGPROCESO' And type = 'P')
	Drop Procedure ECOM_ACTLOGPROCESO
GO

-- ==================================================================
-- Author		: Henry Morales
-- Create date	: 08-02-2018
-- Description	: Actualiza Registro en ECOM_Control_Proceso
-- ==================================================================

CREATE Procedure ECOM_ACTLOGPROCESO(
	@estado		Int,	/*Error (0), Éxito(1)*/
	@proceso	Varchar(50)
)
AS 
BEGIN
	BEGIN TRANSACTION;
		SAVE TRANSACTION SavePoint;
		BEGIN TRY

			Update ECOM_Control_Proceso
				Set	Fecha = GETDATE(),
					Flag_Proceso = @estado
			Where Proceso = @proceso
			  And Cast(Fecha As Date) = Cast(getdate() As Date);
			  
		END TRY
	    BEGIN CATCH
	        IF @@TRANCOUNT > 0
	        BEGIN
		        SELECT ERROR_MESSAGE() AS ErrorMessage;
	            ROLLBACK TRANSACTION SavePoint;
	        END
	    END CATCH
	COMMIT TRANSACTION
END


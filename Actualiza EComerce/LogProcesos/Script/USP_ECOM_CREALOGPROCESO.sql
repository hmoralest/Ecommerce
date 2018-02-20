If Exists(Select * from sysobjects Where name = 'ECOM_CREALOGPROCESO' And type = 'P')
	Drop Procedure ECOM_CREALOGPROCESO
GO

-- ==================================================================
-- Author		: Henry Morales
-- Create date	: 08-02-2018
-- Description	: Crea Registro en ECOM_Control_Proceso
-- ==================================================================

CREATE Procedure ECOM_CREALOGPROCESO(
	@Proceso	Varchar(50)
)
AS 
BEGIN
	BEGIN TRANSACTION;
		SAVE TRANSACTION SavePoint;
		BEGIN TRY

			If not Exists(Select 1 From	ECOM_Control_Proceso Where	Proceso = @Proceso And	Cast(Fecha As Date) = Cast(getdate() As Date))
			BEGIN
				Insert Into ECOM_Control_Proceso (Fecha, Flag_Proceso, Proceso)
				values (getdate(), 0, @Proceso);
			END

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


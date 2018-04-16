If Exists(Select * from sysobjects Where name = 'USP_ActualizaVentas_PS' And type = 'P')
	Drop Procedure USP_ActualizaVentas_PS
GO

-- ==========================================================================================
-- Author			: Henry Morales
-- Create date		: 21/02/2018
-- Description		: Se Actualiza estado de las Ventas que han sido enviadas a los DBF
--					  Se agerga @estado para poder hacer RollBack Manual
-- ==========================================================================================
-- Modificado por	: Henry Morales
-- Fch. Modifica	: 06/04/2018
-- Asunto			: Se agregó el parámetro @tipo, para agregar también Notas de Crédito
--						VT = Ventas
--						NC = Notas de Crédito
-- ==========================================================================================
/*
	Exec USP_ActualizaVentas_PS 'B03000024519','P', 'VT'
*/

CREATE Procedure USP_ActualizaVentas_PS(
	@id			Varchar(12),
	@estado		Varchar(1),
	@tipo		Varchar(2)
)
AS 
BEGIN

	BEGIN TRY
		BEGIN TRAN act_Venta

		IF(@tipo = 'VT')
		BEGIN
			Update Venta Set Ven_EstAct_Alm = @estado Where Ven_Id = @Id;
		END
		IF (@tipo = 'NC')
		BEGIN
			Update Nota_Credito Set Not_EstAct_Alm = @estado Where Not_Id = @Id;
		END
				
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
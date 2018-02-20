If Exists(Select * from sysobjects Where name = 'ECOM_VALIDAPROCESO' And type = 'P')
	Drop Procedure ECOM_VALIDAPROCESO
GO

-- ==================================================================
-- Author		: Henry Morales
-- Create date	: 08-02-2018
-- Description	: Valida Registro en ECOM_Control_Proceso
-- ==================================================================

CREATE Procedure ECOM_VALIDAPROCESO(
	@Proceso	Varchar(50)	/*Error (0), Éxito(1)*/
)
AS 
BEGIN

	Declare @estado Int;
	
	Select @estado = 0;

	If Exists(Select 1 From	ECOM_Control_Proceso Where	Proceso = @Proceso And	Cast(Fecha As Date) = Cast(getdate() As Date))
	BEGIN
			Select	@estado = Cast(Flag_Proceso As Int)
			From	ECOM_Control_Proceso
			Where	Proceso = @Proceso
			  And	Cast(Fecha As Date) = Cast(getdate() As Date);
	END

	Select @estado AS estado;

END


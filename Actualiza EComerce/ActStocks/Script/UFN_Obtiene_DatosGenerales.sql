If Exists(Select * from sysobjects Where name = 'UFN_Obtiene_DatosGenerales' And type = 'FN')
	Drop Function UFN_Obtiene_DatosGenerales
GO

-- ========================================================================
-- Author		: Henry Morales
-- Create date	: 20/04/2018
-- Description	: Obtiene el dato almacenado en la tabla de Datos Generales
-- Estado		-> A : Activo
--				-> B : Baja
-- ========================================================================
/*
	Select dbo.UFN_Obtiene_DatosGenerales('1') AS dato;
*/

CREATE FUNCTION UFN_Obtiene_DatosGenerales(
	@codigo varchar(10)
)  
RETURNS varchar(50) 
AS 
BEGIN
	DECLARE	@result	varchar(50);
 
	Set @result = (	Select Top 1 dato
					From datos_generales
					Where codigo = @codigo
					  And Estado = 'A'
					Order by fecha_crea DESC);

	return @result;

END
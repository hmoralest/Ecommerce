If Exists(Select * from sysobjects Where name = 'USP_Obtiene_DatosConexion' And type = 'P')
	Drop Procedure USP_Obtiene_DatosConexion
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 18/04/2018
-- Description	: Obtiene los datos de conexión
-- Estado		-> A : Activo
--				-> B : Baja
-- =====================================================================
/*
	Exec USP_Obtiene_DatosConexion '01'
*/

CREATE Procedure USP_Obtiene_DatosConexion(
	@id		Varchar(2)
)
AS 
BEGIN

		Select	Top 1
				Id,
				Descripcion,
				Tipo,
				Url,
				BaseDatos,
				Usuario,
				Contrasena,
				CASE Trusted_Connection 
					WHEN 0 THEN 'False'
					WHEN 1 THEN 'True'
					ELSE		NULL
				END			As Trusted_Connection,
				Estado,
				Fecha_Crea,
				Fecha_Modif
		From Acceso_Conexiones
		Where Id = @id
		  And Estado = 'A'
		Order by fecha_Crea DESC;

END

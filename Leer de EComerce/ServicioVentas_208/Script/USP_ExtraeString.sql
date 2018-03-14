If Exists(Select * from sysobjects Where name = 'USP_ExtraeString' And type = 'P')
	Drop Procedure USP_ExtraeString
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 12-03-2018
-- Description	: Realiza la separación de datos en diferentes tablas
-- =====================================================================
/*
	Declare @text varchar(max);
	exec USP_ExtraeString '123-158-489-493','-',2, @text output;
	Select @text
*/

CREATE Procedure USP_ExtraeString
(    
      @campo		VARCHAR(MAX),
	  @separador	VARCHAR(MAX),
      @posicion		INT,
	  @texto		VARCHAR(MAX) OUTPUT
)
AS
BEGIN

	/*Declaración de variables*/
	Declare @consulta		NVarchar(max),
			@consulta2		NVarchar(max),
			@fin			NVarchar(max),
			@fin2			NVarchar(max),
			@cnt			Int,
			@ParmDefinition	NVarchar(max);

	Declare @temporal table (inicio int, fin int);

	/*Inicializa variables*/
	Set @cnt = 1;
	Set @consulta = '';
	Set @fin = ')';
	Set @fin2 = ')';
	Set @ParmDefinition = '@text varchar(max)';
	
	SET @consulta	= 'SELECT CHARINDEX('+char(39)+@separador+char(39)+',@text'
	SET @consulta2	= 'SELECT CHARINDEX('+char(39)+@separador+char(39)+',@text'

	/*Llenamos la consulta*/
	WHILE @cnt < @posicion
	BEGIN
	
		SET @consulta += ', CHARINDEX('+char(39)+@separador+char(39)+',@text'
		SET @fin += '+1)'
		/*Para la segunda consulta 1 vuelta menos (inicio)*/
		IF(@cnt <> 1)
		Begin
		   SET @consulta2 += ', CHARINDEX('+char(39)+@separador+char(39)+',@text'
		   SET @fin2 += '+1)'
		End

		SET @cnt = @cnt + 1;
	END;
	/*Concatena el final de las consultas*/
	Set @consulta+=@fin
	Set @consulta2+=@fin2

	/*Cuando es posición 1, no tenemos el inicio*/
	IF(@posicion =1)
	Begin
		set @consulta2 = 'SELECT 0'
	End

	/*Insertamos Fin*/
	Insert into @temporal (fin)
	EXECUTE sp_executesql @consulta, @ParmDefinition,  
						  @text = @campo;  
						  
	/*Insertamos Inicio*/
	Insert into @temporal (inicio)
	EXECUTE sp_executesql @consulta2, @ParmDefinition,  
						  @text = @campo;  
						  
	/*Se llena el valor que devolveremos*/
	Select @texto = SUBSTRING(	@campo,	-- cadena
								(Select inicio From @temporal Where fin is null)+1, -- inicio
								CASE	WHEN (Select fin From @temporal Where inicio is null) = 0 THEN len(@campo)  -- fin
										ELSE (Select fin From @temporal Where inicio is null)-(Select inicio From @temporal Where fin is null)-1 END
								) 

	RETURN 
END
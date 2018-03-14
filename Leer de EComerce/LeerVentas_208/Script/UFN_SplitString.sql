If Exists(Select * from sysobjects Where name = 'UFN_SplitString' And type = 'TF')
	Drop Function UFN_SplitString
GO

-- =====================================================================
-- Author		: Henry Morales
-- Create date	: 12-03-2018
-- Description	: Realiza la separación de datos en diferentes tablas
-- =====================================================================
/*
	Select Item From UFN_SplitString('xdx/dxd/xdx/d','/');
*/

CREATE FUNCTION UFN_SplitString
(    
      @Input NVARCHAR(MAX),
      @Character CHAR(1)
)
RETURNS @Output TABLE (
      Item NVARCHAR(1000)
)
AS
BEGIN
      DECLARE @StartIndex INT, @EndIndex INT
 
      SET @StartIndex = 1
      IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
      BEGIN
            SET @Input = @Input + @Character
      END
 
      WHILE CHARINDEX(@Character, @Input) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Character, @Input)
           
            INSERT INTO @Output(Item)
            SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
           
            SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
      END
 
      RETURN
END
GO
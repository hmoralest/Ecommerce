USE [BD_ECOMMERCE]
GO

/****** Object:  Table [dbo].[Datos_Generales]    Script Date: 20/04/2018 09:18:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Datos_Generales](
	[Codigo] [varchar](10) NOT NULL,
	[Proceso] [varchar](25) NULL,
	[Descripcion] [varchar](100) NULL,
	[Dato] [varchar](50) NULL,
	[Estado] [varchar](1) NULL,
	[Fecha_Crea] [datetime] NULL,
	[Fecha_Modif] [datetime] NULL,
) 
GO


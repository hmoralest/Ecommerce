USE [BD_ECOMMERCE]
GO

/****** Object:  Table [dbo].[Acceso_Conexiones]    Script Date: 18/04/2018 09:13:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Acceso_Conexiones](
	[Id] [varchar](2) NOT NULL,
	[Descripcion] [varchar](50) NULL,
	[Tipo] [varchar](15) NULL,
	[Url] [varchar](max) NOT NULL,
	[BaseDatos] [varchar](max) NOT NULL,
	[Usuario] [varchar](max) NOT NULL,
	[Contrasena] [varchar](max) NOT NULL,
	[Trusted_Connection] [bit] NULL,
	[Estado] [varchar](1) NOT NULL,
	[Fecha_Crea] [datetime] NULL,
	[Fecha_Modif] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



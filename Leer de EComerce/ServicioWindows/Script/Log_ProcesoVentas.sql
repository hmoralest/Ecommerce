

CREATE TABLE [dbo].[Log_ProcesoVentas](
	[Fecha] [datetime] NULL,
	[id_venta] [varchar](12) NULL,
	[mensaje_error] [varchar](max) NULL,
	[mensaje_sistem] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



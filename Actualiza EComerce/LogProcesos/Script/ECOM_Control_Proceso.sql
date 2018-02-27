/****** Object:  Table [dbo].[ECOM_Control_Proceso]    Script Date: 26/02/2018 12:27:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ECOM_Control_Proceso](
	[Fecha] [datetime] NULL,
	[Flag_Proceso] [int] NOT NULL,
	[Proceso] [varchar](50) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ECOM_Control_Proceso] ADD  CONSTRAINT [DF_ECOM_Control_Proceso_Flag_Proceso]  DEFAULT ((0)) FOR [Flag_Proceso]
GO

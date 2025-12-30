USE [Rento_DB]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 30-Dec-25 12:09:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[ID] [int] NULL,
	[Username] [nvarchar](50) NULL,
	[PasswordHash] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[FullName] [nvarchar](50) NULL,
	[UserType] [nvarchar](50) NULL,
	[PhoneNumber] [nchar](50) NULL
) ON [PRIMARY]
GO



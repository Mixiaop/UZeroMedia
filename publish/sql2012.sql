
/****** Object:  Table [dbo].[UZeroMedia_Pictures]     ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UZeroMedia_Pictures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MimeType] [nvarchar](max) NULL,
	[SeoFilename] [nvarchar](max) NULL,
	[IsNew] [bit] NOT NULL,
	[CreationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.UZeroMedia_Pictures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


/****** Object:  Table [dbo].[UZeroMedia_Files]     ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UZeroMedia_Files](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MimeType] [nvarchar](max) NULL,
	[Extension] [nvarchar](max) NULL,
	[SeoFilename] [nvarchar](max) NULL,
	[IsNew] [bit] NOT NULL,
	[CreationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.UZeroMedia_Files] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO



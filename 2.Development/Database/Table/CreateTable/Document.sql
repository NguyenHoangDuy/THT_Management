create TABLE [dbo].[Document](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [varchar](15) NOT NULL,
	[CategoryID] [nvarchar](50) NOT NULL,
	[DocumentName] [nvarchar](250) NOT NULL,
	[int] [nvarchar](50) NOT NULL,
	[DateSave] [datetime] NULL,
	[Format] [varchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[SideboardID] [nvarchar](50) NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[FileNameLocal] [nvarchar](250) NULL,
	[FileNameServer] [nvarchar](250) NULL,
	[Path] [nvarchar](250) NULL,
	[TimeSave] int NULL,
	[IsSaved] [varchar](20) NULL,
	[IsExpirated] [varchar](20) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [varchar](32) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](32) NULL,
	[Status] [bit] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



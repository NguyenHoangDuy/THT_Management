
create TABLE [dbo].[CategoryConfig](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [varchar](15) NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
	[IsSaved] [varchar](20) NOT NULL,
	[IsExpirated] [varchar](20) NOT NULL,
	[SideboardID] [nvarchar](50) NOT NULL,
	[Format] [nvarchar](250) NULL,
	[TimeSave] int NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [varchar](32) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](32) NULL,
	[Status] [bit] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_CategoryConfig] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



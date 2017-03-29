

CREATE TABLE [dbo].[GroupDocument](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [varchar](15) NOT NULL,
	[GroupName] [nvarchar](250) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [varchar](32) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](32) NULL,
	[Status] [bit] NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_GroupDocument] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



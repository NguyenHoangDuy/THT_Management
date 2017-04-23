USE [THTDB]
GO

/****** Object:  Table [dbo].[Check_In]    Script Date: 04/22/2017 11:21:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].TimeScheduled(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	TimeSheetName [nvarchar](200) NOT NULL,
	HousedHours int NOT NULL,
	Descr [nvarchar](200) NULL,
	StartDate [datetime] NULL,
    EndDate [datetime] NULL,
	Active BIT, 
	CreatedAt [datetime] NULL,
	CreatedBy [nvarchar](32) NULL,
	UpdatedAt [datetime] NULL,
	UpdatedBy [nvarchar](32) NULL,
 CONSTRAINT [PK_TimeScheduled] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



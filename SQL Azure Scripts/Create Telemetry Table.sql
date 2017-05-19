USE [waste-management]
GO

/****** Object:  Table [dbo].[Telemetry]    Script Date: 19/05/2017 12:00:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Telemetry](
	[deviceid] [nvarchar](50) NOT NULL,
	[timeutc] [datetime] NOT NULL,
	[timelocal] [datetime] NOT NULL,
	[level] [float] NOT NULL,
	[temperature] [float] NOT NULL,
	[pressure] [float] NOT NULL,
	[humidity] [float] NOT NULL,
	[Precipitation] [float] NOT NULL,
	[Wind] [float] NOT NULL,
	[Cloud] [float] NOT NULL,
	[Weather] [nvarchar](50) NOT NULL,
	[GeopointId] [bigint] NOT NULL,
	[Location] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Telemetry] PRIMARY KEY CLUSTERED 
(
	[deviceid] ASC,
	[timeutc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[Telemetry] ADD  CONSTRAINT [DF_Telemetry_GeopointId]  DEFAULT ((1)) FOR [GeopointId]
GO



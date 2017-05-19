USE [waste-management]
GO

/****** Object:  View [dbo].[Telemetry12HourView]    Script Date: 19/05/2017 12:01:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[Telemetry12HourView] AS
SELECT TOP(2000) Telemetry.*, Geopoint.[Description], Geopoint.[Latitude], Geopoint.Longitude
FROM dbo.Telemetry
INNER JOIN Geopoint ON Geopoint.GeopointId = Telemetry.GeopointId
WHERE timeutc >= DATEADD(hour, -12, GETDATE())
ORDER BY timelocal DESC










GO



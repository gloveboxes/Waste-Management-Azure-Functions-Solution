USE [waste-management]
GO

/****** Object:  View [dbo].[Telemetry24HourView]    Script Date: 19/05/2017 12:02:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[Telemetry24HourView] AS
SELECT TOP(2000) Telemetry.*, Geopoint.[Description], Geopoint.[Latitude], Geopoint.Longitude
FROM dbo.Telemetry
INNER JOIN Geopoint ON Geopoint.GeopointId = Telemetry.GeopointId
WHERE timeutc >= DATEADD(hour, -24, GETDATE())
ORDER BY timelocal DESC


GO



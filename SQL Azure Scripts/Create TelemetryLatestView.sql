USE [waste-management]
GO

/****** Object:  View [dbo].[TelemetryLatestView]    Script Date: 19/05/2017 12:02:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




















CREATE VIEW [dbo].[TelemetryLatestView] AS
SELECT DISTINCT X.*, Geopoint.[Description], Geopoint.[Latitude], Geopoint.Longitude
FROM Telemetry f
INNER JOIN Geopoint ON Geopoint.GeopointId = f.GeopointId
CROSS APPLY(SELECT TOP 1 * FROM Telemetry WHERE DeviceId = f.DeviceId AND timeutc >= DATEADD(hour, -1, GETDATE())  ORDER BY timeutc desc) AS X

















GO



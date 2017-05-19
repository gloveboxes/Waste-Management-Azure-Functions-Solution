This solutions was designed to be used in conjunction with The Things Network To Azure IoT Hub Gateway.

[The Things Network to Azure IoT Hub Gateway](https://github.com/gloveboxes/The-Things-Network-Azure-IoT-Hub-Gateway)


![Architecture](https://raw.githubusercontent.com/gloveboxes/Waste-Management-Azure-Functions-Solution/master/Images/Waste%20Manage%20System.jpg)




## Stream Analytics

![Stream Analytics](https://raw.githubusercontent.com/gloveboxes/Waste-Management-Azure-Functions-Solution/master/Images/Stream%20Analytics.JPG)

    WITH Telemetry AS (
        SELECT
            T.iothub.connectiondeviceid AS DeviceId,
            Max(T.EventEnqueuedUtcTime) AS TimeUTC,
            Max(DateAdd(Hour, 10, T.EventEnqueuedUtcTime)) AS TimeLocal, -- AU EST UTC + 10
            MAX(T.Level) AS Level,
            L.Location AS Location,
            L.GeopointId AS GeopointId,
            W.Temperature AS Temperature,
            W.Pressure AS Pressure,
            W.Humidity AS Humidity,
            W.Precipitation AS Precipitation,
            W.Wind AS Wind,
            W.Cloud AS Cloud,
            W.Weather AS Weather
        FROM [IoTHubSFM] T TIMESTAMP BY EventEnqueuedUtcTime
        INNER JOIN LocationSFM L ON T.iothub.connectiondeviceid = L.DeviceId
        INNER JOIN WeatherSFM W ON W.WeatherID = 'Sydney, AU'
        GROUP BY
            T.iothub.connectiondeviceid, TumblingWindow(minute, 3), L.Location, L.GeopointId, 
            W.Temperature, W.Pressure, W.Humidity, W.Precipitation, W.Wind, W.Cloud, W.Weather
    )

    SELECT * INTO TelemetryPBI FROM Telemetry
    SELECT * INTO TelemetryBlob FROM Telemetry
    SELECT * INTO TelemetrySQL FROM Telemetry
    SELECT * INTO AlertQueue FROM Telemetry WHERE level > 80




# Azure Function Apps

## The Things Network to Azure IoT Hub Gateway

![The Things Network and Azure IoT Hub Integration](https://raw.githubusercontent.com/gloveboxes/Waste-Management-Azure-Functions-Solution/master/Images/Slide4.JPG)


Technology: Azure Function HTTP Trigger App


[The Things Network To Azure IoT Hub Bridge Azure Function Code](https://github.com/gloveboxes/Waste-Management/tree/master/Functions/Functions/TheThingsNetworkBridge)



![The Things Network Bridge](https://raw.githubusercontent.com/gloveboxes/Waste-Management-Azure-Functions-Solution/master/Images/ThingsNetworkBridgeFunctionAppHttpTrigger.JPG)


## The Things Network Http Integration


![The Things Network HTTP Integration](https://raw.githubusercontent.com/gloveboxes/Waste-Management-Azure-Functions-Solution/master/Images/TheThingsNetworkHttpIntegration.JPG)


## Open Weather Map Azure Function App






## SQL Azure Views for Power BI

```SQL

CREATE VIEW [dbo].[TelemetryLatestView] AS
SELECT DISTINCT X.*, Geopoint.[Latitude], Geopoint.Longitude
FROM Telemetry f
INNER JOIN Geopoint ON Geopoint.GeopointId = f.GeopointId
CROSS APPLY(SELECT TOP 1 * FROM Telemetry WHERE DeviceId = f.DeviceId AND timeutc >= DATEADD(hour, -1, GETDATE())  ORDER BY timeutc desc) AS X

```


using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using OpenWeatherMap;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;

public class OWM
{
    static OpenWeatherMapClient client;
    static string owmKey = ConfigurationManager.AppSettings["OpenWeatherMapKey"];
    static string storageAcct = ConfigurationManager.AppSettings["StorageAccount"];
    static string container = ConfigurationManager.AppSettings["OwmContainer"];

    public static async void Run(TimerInfo myTimer, TraceWriter log)
    {
        client = new OpenWeatherMapClient(owmKey);

        log.Info($"Open Weather Map Timer trigger function executed at: {DateTime.Now}");

        var result = await client.CurrentWeather.GetByName("Sydney, AU", MetricSystem.Metric);
        await WriteToBlob(result);
    }

    static async Task<string> WriteToBlob(OpenWeatherMap.CurrentWeatherResponse result)
    {
        string status = string.Empty;

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAcct);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
        blobContainer.CreateIfNotExists();

        string createddate = Convert.ToDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd/HH-mm/");
        string blobName = "weather/" + createddate + "weather.csv";
        CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
        blob.Properties.ContentType = "text/csv";

        if (await blob.ExistsAsync()) { return "Exists"; }

        try
        {
            string h = $"WeatherId;Temperature;Pressure;Humidity;Precipitation;Wind;Cloud;Weather{Environment.NewLine}Sydney, AU;{result.Temperature.Value.ToString()};{result.Pressure.Value.ToString()};{result.Humidity.Value.ToString()};{result.Precipitation.Value.ToString()};{result.Wind.Speed.Value.ToString()};{result.Clouds.Value.ToString()};{result.Weather.Value}";
            await blob.UploadTextAsync(string.Format(h, blobName));
        }
        catch (StorageException e)
        {
            return $"Error {e.Message}";
        }
        return "Success";
    }

}
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Twilio;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System.Configuration;
using System.Net;
using System.Text;


public class AlertQueueEntity
{
    public string DeviceId { get; set; }
    public float Level { get; set; }
    public string Location { get; set; }
}

public class SmsAlertEntity : TableEntity
{
    public SmsAlertEntity(string deviceId, string phoneNumber)
    {
        this.PartitionKey = "Bin";
        this.RowKey = deviceId;
        this.PhoneNumber = phoneNumber;
    }
    public SmsAlertEntity() {}
    public DateTime LastAlertUTC { get; set; }
    public string PhoneNumber { get; set; }
}

static string accountSid = ConfigurationManager.AppSettings["AlertTwilioSid"];
static string authToken = ConfigurationManager.AppSettings["AlertTwilioToken"];
static string alertDefaultPhoneNumber = ConfigurationManager.AppSettings["AlertDefaultPhoneNumber"];
static string twilioPhoneNumber = ConfigurationManager.AppSettings["AlertTwilioPhoneNumber"];
static int threshholdMiniutes = int.Parse(ConfigurationManager.AppSettings["AlertThresholdSeconds"]);
static string storageAcct = ConfigurationManager.AppSettings["StorageAccount"];
static string telstraSmsApiConsumerKey = ConfigurationManager.AppSettings["TelstraSMSApiConsumerKey"];
static string telstraSmsConsumerSecret = ConfigurationManager.AppSettings["TelstraSmsConsumerSecret"];


public static void Run(string alertQueueItem, TraceWriter log)
{
    TimeSpan ts = TimeSpan.MaxValue;
    SmsAlertEntity updateEntity;   
    var telstraToken = GetAccessToken(); 


    var alert = JsonConvert.DeserializeObject<AlertQueueEntity>(alertQueueItem, new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()});

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAcct);            
    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();     // Create the table client.
    CloudTable table = tableClient.GetTableReference("SMSAlerts");              // Retrieve a reference to the table.

    if (alert.DeviceId.ToLower() != "wm-bin-07") {return;}

    TableOperation retrieveOperation = TableOperation.Retrieve<SmsAlertEntity>("Bin", alert.DeviceId);     //retrieve Bin SMS Alter data
    TableResult retrievedResult = table.Execute(retrieveOperation);

    if (retrievedResult.Result == null)
    {
        updateEntity = new SmsAlertEntity(alert.DeviceId, alertDefaultPhoneNumber);
    }
    else {
        updateEntity = (SmsAlertEntity)retrievedResult.Result;
        ts = DateTime.UtcNow - updateEntity.LastAlertUTC;
    }

    if (ts.TotalMinutes > threshholdMiniutes)
    {
        log.Info($"Sending SMS Alert for {alert.Location}");    

        var smsMessage = $"Please empty bin located at {alert.Location}"; 

        SendSms(telstraToken, updateEntity.PhoneNumber, smsMessage);

        //var client = new TwilioRestClient(accountSid, authToken);   
        //var result = client.SendMessage(twilioPhoneNumber, updateEntity.PhoneNumber, $"Please empty bin located at {alert.Location}");

        updateEntity.LastAlertUTC = DateTime.UtcNow;
        TableOperation updateOperation = TableOperation.InsertOrReplace(updateEntity);
        table.Execute(updateOperation);     // Execute the operation.
    }
    else {
        log.Info($"SMS alert for {alert.Location} was already sent {ts.Minutes} ago.");
    }   
}

// http://blog.wenstor.com/2015/06/using-telstra-api-to-send-sms-in-aspnet.html

static string GetAccessToken()
{
    string consumerKey = telstraSmsApiConsumerKey;
    string consumerSecret = telstraSmsConsumerSecret;
    string url = string.Format("https://api.telstra.com/v1/oauth/token?client_id={0}&client_secret={1}&grant_type=client_credentials&scope=SMS", consumerKey, consumerSecret);

    using (var webClient = new System.Net.WebClient())
    {
        var json = webClient.DownloadString(url);
        var obj = JObject.Parse(json);
        return obj.GetValue("access_token").ToString();
    }
}

static void SendSms(string token, string recipientNumber, string message)
{
    using (var webClient = new System.Net.WebClient())
    {
        webClient.Headers.Clear();
        webClient.Headers.Add(HttpRequestHeader.ContentType, @"application/json");
        webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);

        string data = "{\"to\":\"" + recipientNumber + "\", \"body\":\"" + message + "\"}";
        var response = webClient.UploadData("https://api.telstra.com/v1/sms/messages", "POST", Encoding.Default.GetBytes(data));
        var responseString = Encoding.Default.GetString(response);
        var obj = JObject.Parse(responseString);
        //return obj.GetValue("messageId").ToString();
        // Now parse with JSON.Net
    }
}

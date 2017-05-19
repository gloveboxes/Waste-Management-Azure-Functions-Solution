#r "Newtonsoft.Json"

using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography;
using System.Globalization;
using System.Configuration;
using System.Net.Http.Headers;


public class TTNEntity
{
    public string app_id { get; set; }
    public string dev_id { get; set; }
    //public string hardware_serial { get; set; }
    //public string counter { get; set; }
    public string payload_raw { get; set; }
    //public string downlink_url { get; set; }
}

public class Telemetry
{
    public UInt32 Level { get; set; }
    public string Schema { get; set; } = "1";

    public string ToJson(UInt32 level)
    {
        this.Level = level;
        return JsonConvert.SerializeObject(this);
    }
}

static Telemetry telemetry = new Telemetry();
static string api = "2016-11-14";
static string host = ConfigurationManager.AppSettings["IoTHubHost"];
static string IoTHubRegistryCS = ConfigurationManager.AppSettings["IoTHubRegistry"];
static string iotHubKey = "pg3xZ7jiDBHt7jF21Vjh8lTm61f1naV+JnW5ph/1//E=";
static string iotHubKeyName = "iothubowner";


public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    DateTime start = DateTime.Now;

    dynamic data = await req.Content.ReadAsAsync<object>(); // Get request body

    var ttn = JsonConvert.DeserializeObject<TTNEntity>(data.ToString(), new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

    var result = DecodeRawData(ttn.payload_raw);
    
    string restUri = $"https://{host}/devices/{ttn.dev_id}/messages/events?api-version={api}";

    string key = await GetDeviceKeyFromRegistry(ttn.dev_id);

    if (key == null) { return req.CreateResponse(HttpStatusCode.BadRequest); }

    var sasToken = getDeviceSaSToken(host, ttn.dev_id, key);

    HttpClient client = new HttpClient();
    
    client.DefaultRequestHeaders.Add("Authorization", sasToken);

    var content = new StringContent(telemetry.ToJson(result));
    var response = await client.PostAsync(restUri, content);

    DateTime finish = DateTime.Now;
    TimeSpan total = finish - start;
    log.Info($"DeviceId: {ttn.dev_id}, ms:{total.Milliseconds}");

    return req.CreateResponse(HttpStatusCode.OK);
}

public static UInt32 DecodeRawData(string base64EncodedData)
{
    UInt32 result = 0;
    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
    var d = System.Text.Encoding.UTF7.GetString(base64EncodedBytes);

    for (int i = 0; i < d.Length; i++)
    {
        result = result << (8);
        result += (byte)d[i];
    }
    return result;
}


public static async Task<string> GetDeviceKeyFromRegistry(string deviceId)
{
    string sasToken = getRegistrySaSToken(host, iotHubKey, iotHubKeyName);

    var url = $"https://{host}/devices/{deviceId}?api-version={"2016-02-03"}"; //%s' % (self.iotHost, deviceId, self.API_VERSION)
    HttpClient client = new HttpClient();

    client.DefaultRequestHeaders.Add("Authorization", sasToken);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

    var response = await client.GetAsync(url);

    if (response.StatusCode != HttpStatusCode.OK) { return null; }
    
    var responseBody = await response.Content.ReadAsStringAsync();

    return JObject.Parse(responseBody)["authentication"]["symmetricKey"]["primaryKey"].ToString();
}

static string getRegistrySaSToken(string host, string key, string keyName)
{
    return $"{genSaSToken(host, key)}&skn={keyName}";
}

public static string getDeviceSaSToken(string host, string device, string key)
{
    return genSaSToken($"{host}/devices/{device}", key);
}

public static string genSaSToken(string url, string key, int expirySeconds = 3600)
{
    TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
    string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + expirySeconds);

    string baseAddress = WebUtility.UrlEncode(url.ToLower());
    string stringToSign = $"{baseAddress}\n{expiry}";

    byte[] data = Convert.FromBase64String(key);
    HMACSHA256 hmac = new HMACSHA256(data);
    string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

    string token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}",
                    baseAddress, WebUtility.UrlEncode(signature), expiry);
    return token;
}
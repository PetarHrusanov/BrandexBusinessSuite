namespace BrandexBusinessSuite.Requests;

using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Common;

public static class RequestsMethods
{
    
    private static readonly string NewStateSerialized = JsonConvert.SerializeObject( new { newState = "Released" });
    private static readonly StringContent StateContent =  new (NewStateSerialized, Encoding.UTF8, Constants.RequestConstants.ApplicationJson);
    private const string GeneralRequest = "https://brandexbg.my.erp.net/api/domain/odata/";
    
    public static async Task<JObject> JObjectByUriPostRequest(HttpClient client, string newUri, string jsonPostString)
    {
        var uri = new Uri(newUri);
        var content = new StringContent(jsonPostString, Encoding.UTF8, Constants.RequestConstants.ApplicationJson);
        var response = await client.PostAsync(uri, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JObject.Parse(responseContent);
    }

    public static async Task<JObject> JObjectByUriGetRequest(HttpClient client, string newUri)
    {
        int retryCount = 3; // Maximum number of retries
        int retryDelayMilliseconds = 1000; // Delay between retries in milliseconds

        for (int retry = 0; retry < retryCount; retry++)
        {
            try
            {
                var uri = new Uri(newUri);
                var response = await client.GetAsync(uri);
                var responseContent = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseContent);

                if (jObject != null) // Check if the JObject is not null
                {
                    return jObject;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // You can log or handle the exception here if needed
            }

            await Task.Delay(retryDelayMilliseconds);
        }

        return null; // Return null if retries are unsuccessful
    }
    
    public static async Task ChangeStateToRelease(HttpClient client, string document)
    {
        var uriChangeState = new Uri($"{GeneralRequest}{document}/ChangeState");
        await client.PostAsync(uriChangeState, StateContent);
    }
    
    public static void AuthenticateUserBasicHeader(HttpClient client, string user, string password)
    {
        var byteArray = Encoding.ASCII.GetBytes($"{user}:{password}");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }

}
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
        var uri = new Uri(newUri);
        var response = await client.GetAsync(uri);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JObject.Parse(responseContent);
    }
    public static async Task ChangeStateToRelease(HttpClient client, string document)
    {
        var uriChangeState = new Uri($"{GeneralRequest}{document}/ChangeState");
        await client.PostAsync(uriChangeState, StateContent);
    }

}
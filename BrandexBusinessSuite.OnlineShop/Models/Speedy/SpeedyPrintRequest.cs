namespace BrandexBusinessSuite.OnlineShop.Models.Speedy;

using Newtonsoft.Json;

public class SpeedyPrintRequest
{

    public SpeedyPrintRequest(string userName, string password)
    {
        UserName = userName;
        Password = password;
        Parcels = new List<SpeedyParcelId>();
    }
    
    [JsonProperty("userName")]
    public string UserName { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("paperSize")] public string PaperSize { get; set; } = "A6";
    
    [JsonProperty("parcels")]
    public List<SpeedyParcelId> Parcels { get; set; }

}
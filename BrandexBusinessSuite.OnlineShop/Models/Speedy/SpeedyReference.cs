namespace BrandexBusinessSuite.OnlineShop.Models.Speedy;

using Newtonsoft.Json;

public class SpeedyReference
{
    public SpeedyReference()
    {
        
    }

    public SpeedyReference(string reference)
    {
        Reference = reference;
    }
    [JsonProperty("ref")]
    public string Reference { get; set; }
}
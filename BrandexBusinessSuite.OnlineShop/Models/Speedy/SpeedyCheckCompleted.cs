namespace BrandexBusinessSuite.OnlineShop.Models.Speedy;

using Newtonsoft.Json;

public class SpeedyCheckCompleted
{
    public SpeedyCheckCompleted()
    {
        Parcels = new List<SpeedyReference>();
    }
    
    public SpeedyCheckCompleted(string userName, string password)
    {
        UserName = userName;
        Password = password;
        Parcels = new List<SpeedyReference>();
    }
    
    public SpeedyCheckCompleted(string userName, string password, List<SpeedyReference> references)
    {
        UserName = userName;
        Password = password;
        Parcels = references;
    }

    [JsonProperty("userName")]
    public string UserName { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }
    
    [JsonProperty("parcels")]
    public List<SpeedyReference> Parcels { get; set; }
}
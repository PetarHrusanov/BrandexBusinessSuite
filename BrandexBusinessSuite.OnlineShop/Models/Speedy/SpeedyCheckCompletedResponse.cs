namespace BrandexBusinessSuite.OnlineShop.Models.Speedy;

using Newtonsoft.Json;

public class SpeedyCheckCompletedResponse
{

    public SpeedyCheckCompletedResponse()
    {
        Operations = new List<_Operations>();
    }
    
    [JsonProperty("parcelId")]
    public string ParcelId { get; set; }
    
    [JsonProperty("ref")]
    public string Reference { get; set; }
    
    [JsonProperty("operations")]
    public List<_Operations> Operations { get; set; }

    public class _Operations
    {
        [JsonProperty("operationCode")]
        public int OperationCode { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
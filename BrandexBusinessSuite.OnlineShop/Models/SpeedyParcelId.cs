using Newtonsoft.Json;

namespace BrandexBusinessSuite.OnlineShop.Models;

public class SpeedyParcelId
{

    public SpeedyParcelId(string id)
    {
        Parcel = new _Parcel(id);
    }
    
    [JsonProperty("parcel")]
    public _Parcel Parcel { get; set; }

    public class _Parcel
    {

        public _Parcel(string id)
        {
            Id = id;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
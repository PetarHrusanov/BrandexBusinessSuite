using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpSalesOrderAnalysis
{
    // [JsonProperty("@odata.id")]
    public string Id { get; set; }

    public string DocumentDate { get; set; }

    public ICollection<ErpSalesLineAnalysis> Lines { get; set; } = new HashSet<ErpSalesLineAnalysis>();
    
    public CrmCustomer ShipToCustomer { get; set; }

    public ErpShipToPartyContactMechanism ShipToPartyContactMechanism { get; set; }
    
}
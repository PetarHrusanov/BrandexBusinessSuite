namespace BrandexSalesAdapter.Models.ErpDocuments;

using Newtonsoft.Json;

public abstract class ErpDocument
{

    public ErpDocument()
    {
        DocumentType = new _DocumentType();
        EnterpriseCompany = new _EnterpriseCompany();
        EnterpriseCompanyLocation = new _EnterpriseCompanyLocation();
    }
    
    public _DocumentType DocumentType { get; set; }
    public _EnterpriseCompany EnterpriseCompany { get; set; }
    public _EnterpriseCompanyLocation EnterpriseCompanyLocation { get; set; }
    

    public class _DocumentType
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }

    public class _EnterpriseCompany
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }
    
    public class _EnterpriseCompanyLocation
    {
        [JsonProperty("@odata.id")]
        public string Id { get; set; }
    }
    
}
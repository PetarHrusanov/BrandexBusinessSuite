namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

public class PharmacyCheckErpModel
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    public string ErpId { get; set; }
    
    public string CompanyId { get; set; }

    public bool? IsActive { get; set; }
    
    public string Address { get; set; }
    
    public int? PhoenixId { get; set; }
    public int? PharmnetId { get; set; }
    public int? SopharmaId { get; set; }
    public int? StingId { get; set; }
    
    public int? Region { get; set; }
    public int? PharmacyChain { get; set; }
    public int? Class { get; set; }

}
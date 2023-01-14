namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

public class PharmacyCheckErpModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string ErpId { get; set; }
    
    public int CompanyId { get; set; }
    public string CompanyIdErp { get; set; }
    public int RegionId { get; set; }
    public string RegionErp { get; set; }
    public int PharmacyChainId { get; set; }
    public string PharmacyChainErp { get; set; }
    public bool? IsActive { get; set; }
    public string Address { get; set; }
    public int? PhoenixId { get; set; }
    public int? PharmnetId { get; set; }
    public int? SopharmaId { get; set; }
    public int? StingId { get; set; }
    
    // public int? Class { get; set; }

}
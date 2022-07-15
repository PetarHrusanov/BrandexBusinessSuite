namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

using Data.Enums;

public class PharmacyDbInputModel
{
    public int BrandexId { get; set; }
    public string Name { get; set; }
    public PharmacyClass PharmacyClass { get; set; }
    public bool Active { get; set; }
    public int CompanyId { get; set; }
    public int PharmacyChainId { get; set; }
    public string Address { get; set; }
    public int CityId { get; set; }
    public int? PharmnetId { get; set; }
    public int? PhoenixId { get; set; }
    public int? SopharmaId { get; set; }
    public int? StingId { get; set; }
    public int RegionId { get; set; }
}
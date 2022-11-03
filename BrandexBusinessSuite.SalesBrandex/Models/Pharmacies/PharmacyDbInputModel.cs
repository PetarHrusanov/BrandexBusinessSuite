namespace BrandexBusinessSuite.SalesBrandex.Models.Pharmacies;

using Data.Enums;

public class PharmacyDbInputModel
{
    public int? BrandexId { get; set; }= null;
    public string ErpId { get; set; }
    public string Name { get; set; }
    public string? PartyCode { get; set; }
    public PharmacyClass? PharmacyClass { get; set; } = null;
    public int? CompanyId { get; set; }= null;
    public int? PharmacyChainId { get; set; }= null;
    public string Address { get; set; }
    public int? CityId { get; set; }= null;
    public int? RegionId { get; set; }= null;

}
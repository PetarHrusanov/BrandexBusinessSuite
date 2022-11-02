namespace BrandexBusinessSuite.SalesBrandex.Models.Pharmacies;

using Data.Enums;

public class PharmacyDbInputModel
{
    public int Id { get; set; }
    public int BrandexId { get; set; }
    public string ErpId { get; set; }
    public string Name { get; set; }
    public string PartyCode { get; set; }
    public PharmacyClass? PharmacyClass{ get;set; }
    public int? CompanyId { get; set; }
    public int? PharmacyChainId { get; set; }
    public string Address { get; set; }
    public int? CityId { get; set; }
    public int? RegionId { get; set; }

}
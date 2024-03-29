using System;

namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

public class PharmacyDbUpdateModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CompanyId { get; set; }
    public int PharmacyChainId { get; set; }
    public string Address { get; set; }
    public int? PharmnetId { get; set; }
    public int? PhoenixId { get; set; }
    public int? SopharmaId { get; set; }
    public int? StingId { get; set; }
    public int RegionId { get; set; }
    public DateTime ModifiedOn { get; set; }
}
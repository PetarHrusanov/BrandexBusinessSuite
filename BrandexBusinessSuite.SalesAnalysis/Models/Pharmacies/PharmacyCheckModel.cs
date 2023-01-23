namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

using AutoMapper;

using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;

public class PharmacyCheckModel : IMapFrom<Pharmacy>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpId { get; set; }
    public int BrandexId { get; set; }
    public int? PharmnetId { get; set; }
    public int? PhoenixId { get; set; }
    public int? SopharmaId { get; set; }
    public int? StingId { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Pharmacy, PharmacyCheckModel>();

}
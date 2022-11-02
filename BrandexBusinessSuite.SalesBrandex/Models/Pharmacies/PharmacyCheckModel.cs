namespace BrandexBusinessSuite.SalesBrandex.Models.Pharmacies;

using AutoMapper;

using BrandexBusinessSuite.SalesBrandex.Data.Models;
using BrandexBusinessSuite.Models;
public class PharmacyCheckModel : IMapFrom<Pharmacy>
{
    public int Id { get; set; }

    public int BrandexId { get; set; }
    
    public string ErpId { get; set; }

    public string Name { get; set; }
    
    public string PartyCode { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Pharmacy, PharmacyCheckModel>();
}
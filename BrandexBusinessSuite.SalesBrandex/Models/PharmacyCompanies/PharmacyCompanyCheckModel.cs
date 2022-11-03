namespace BrandexBusinessSuite.SalesBrandex.Models.PharmacyCompanies;

using AutoMapper;

using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.SalesBrandex.Data.Models;

public class PharmacyCompanyCheckModel : IMapFrom<Company>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpId { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Company, PharmacyCompanyCheckModel>();
}
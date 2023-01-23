using AutoMapper;
using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;

namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

public class PharmacyCheckErpModel : IMapFrom<Pharmacy>
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
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Pharmacy, PharmacyCheckErpModel>()
            .ForMember(o => o.CompanyIdErp, cfg => cfg
                .MapFrom(o => o.Company.ErpId))
            .ForMember(o => o.RegionErp, cfg => cfg
                .MapFrom(o => o.Region.ErpId))
            .ForMember(o => o.PharmacyChainErp, cfg => cfg
                .MapFrom(o => o.PharmacyChain.ErpId))
            .ForMember(o => o.PharmacyChainErp, cfg => cfg
                .MapFrom(o => o.PharmacyChain.ErpId));

}
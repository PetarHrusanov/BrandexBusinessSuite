using AutoMapper;
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models;

namespace BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;

public class AdMediaDisplayModel : IMapFrom<AdMedia>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<AdMedia, AdMediaDisplayModel>()
            .ForMember(o => o.CompanyName, cfg => cfg
                .MapFrom(o => o.Company.Name));
}
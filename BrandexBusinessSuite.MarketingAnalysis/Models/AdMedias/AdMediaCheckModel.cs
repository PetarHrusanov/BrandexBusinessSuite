using AutoMapper;
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models;

namespace BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;

public class AdMediaCheckModel : IMapFrom<AdMedia>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CompanyId { get; set; }
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<AdMedia, AdMediaCheckModel>();
}
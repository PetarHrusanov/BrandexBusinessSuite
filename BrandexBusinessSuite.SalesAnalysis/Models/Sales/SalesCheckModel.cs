using AutoMapper;
using BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

namespace BrandexBusinessSuite.SalesAnalysis.Models.Sales;

using System;

using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;

public class SalesCheckModel : IMapFrom<Sale>
{
    public int Id { get; set; }
    public int PharmacyId { get; set; }
    public int ProductId { get; set; }
    public int DistributorId { get; set; }
    public int RegionId { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }

    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Sale, SalesCheckModel>()
            .ForMember(o => o.RegionId, cfg => cfg
                .MapFrom(o => o.Pharmacy.RegionId));
}
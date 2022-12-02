namespace BrandexBusinessSuite.SalesAnalysis.Models.Products;

using AutoMapper;

using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;

public class ProductCheckModel : IMapFrom<Product>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BrandexId { get; set; }
    public int? PharmnetId { get; set; }
    public int? PhoenixId { get; set; }
    public string SopharmaId { get; set; }
    public int? StingId { get; set; }
    public string ErpId { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Product, ProductCheckModel>();
}
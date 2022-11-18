namespace BrandexBusinessSuite.Inventory.Models.Materials;

using AutoMapper;

using Data.Enums;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Models;

public class MaterialOutputModel : IMapFrom<Material>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpId { get; set; }
    public string PartNumber { get; set; }

    public MaterialType Type { get; set; }
    public MaterialMeasurement Measurement { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Material, MaterialOutputModel>();
}
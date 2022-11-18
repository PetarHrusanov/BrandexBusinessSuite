namespace BrandexBusinessSuite.Inventory.Models.Materials;

using System.ComponentModel.DataAnnotations;
using BrandexBusinessSuite.Inventory.Data.Enums;

public class RawMaterialInputModel
{
    public string MaterialsValue { get; set; }
    
    [EnumDataType(typeof(MaterialType))]
    public MaterialType MaterialsType { get; set; }
    
    [EnumDataType(typeof(MaterialMeasurement))]
    public MaterialMeasurement MaterialsMeasure { get; set; }
}
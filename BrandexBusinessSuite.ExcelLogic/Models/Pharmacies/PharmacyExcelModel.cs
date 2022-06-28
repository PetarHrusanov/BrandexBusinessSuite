namespace BrandexBusinessSuite.ExcelLogic.Models.Pharmacies;

using System.Collections.Generic;
using BrandexBusinessSuite.ExcelLogic.Data.Enums;
using BrandexBusinessSuite.ExcelLogic.Models.Sales;

public class PharmacyExcelModel
{
    public string Name { get; set; }

    public string Address { get; set; }

    public PharmacyClass PharmacyClass { get; set; }

    public string Region { get; set; }

    public ICollection <SaleExcelOutputModel> Sales { get; set; }

}
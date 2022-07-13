using BrandexBusinessSuite.SalesAnalysis.Data.Enums;
using BrandexBusinessSuite.SalesAnalysis.Models.Sales;

namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

using System.Collections.Generic;
using SalesAnalysis.Data.Enums;
using SalesAnalysis.Models.Sales;

public class PharmacyExcelModel
{
    public string Name { get; set; }

    public string Address { get; set; }

    public PharmacyClass PharmacyClass { get; set; }

    public string Region { get; set; }

    public ICollection <SaleExcelOutputModel> Sales { get; set; }

}
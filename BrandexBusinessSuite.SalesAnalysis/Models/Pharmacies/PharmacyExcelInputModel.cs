using Microsoft.AspNetCore.Http;

namespace BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

public class PharmacyExcelInputModel
{
    public string Distributor { get; set;}
    public IFormFile ImageFile { get; set;}
}
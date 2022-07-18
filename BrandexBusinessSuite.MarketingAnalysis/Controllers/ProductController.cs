namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;

using Infrastructure;
using Models.Products;
using Services.Products;

using static Methods.ExcelMethods;

using static Common.Constants;

public class ProductController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IProductsService _productsService;

    public ProductController(IWebHostEnvironment hostEnvironment, IProductsService productsService)
    {
        _hostEnvironment = hostEnvironment;
        _productsService = productsService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Import([FromForm] IFormFile file)
    {

        var errorDictionary = new List<string>();

        var productsCheck = await _productsService.GetCheckModels();

        var uniqueProducts = new List<ProductInputModel>();

        if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);


        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);
        
        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var newProduct = new ProductInputModel();

            var nameRow = row.GetCell(0);

            if (nameRow != null)
            {
                newProduct.Name = nameRow.ToString()?.TrimEnd().ToUpper() ?? throw new InvalidOperationException();
            }

            var shortNameRow = row.GetCell(1);

            if (shortNameRow != null)
            {
                newProduct.ShortName = shortNameRow.ToString()?.TrimEnd().ToUpper() ??
                                       throw new InvalidOperationException();
            }


            if (productsCheck.All(c =>
                    !string.Equals(c.Name, newProduct.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                uniqueProducts.Add(newProduct);
            }

            else
            {
                errorDictionary.Add($"{i} Line: Error");
            }
        }

        await _productsService.UploadBulk(uniqueProducts);

        return JsonConvert.SerializeObject(errorDictionary.ToArray());
    }
}
namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using Infrastructure;
using Models.Products;
using Services.Products;

using static Methods.ExcelMethods;

public class ProductsController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IProductsService _productsService;
    
    public ProductsController(
        IWebHostEnvironment hostEnvironment,
        IProductsService productsService)

    {
        _hostEnvironment = hostEnvironment;
        _productsService = productsService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {
        var errorDictionary = new List<string>();

        if (!CheckXlsx(file, errorDictionary)) return JsonConvert.SerializeObject(errorDictionary.ToArray());
        
        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var newProduct = new ProductInputModel
            {
                Name = row.GetCell(0)?.ToString()?.TrimEnd(),
                ShortName = row.GetCell(1)?.ToString()?.TrimEnd()
            };

            var brandexId = row.GetCell(2)?.ToString()?.TrimEnd();
            if (int.TryParse(brandexId, out var brandexIdInt)) newProduct.BrandexId = brandexIdInt;

            var priceRow = row.GetCell(7)?.ToString()?.TrimEnd();
            if (double.TryParse(priceRow, out var price)) newProduct.Price = price;
            
            if (newProduct.BrandexId == 0 || newProduct.Name == null || newProduct.ShortName == null ||
                newProduct.Price == 0)
            {
                errorDictionary.Add($"{i} Line: Brandex Id, Name, Short Name or Price are incorrect.");
            }
            
            if (ConvertRowToInt(row, 3) != 0) newProduct.PhoenixId = ConvertRowToInt(row, 3);
            if (ConvertRowToInt(row, 4) != 0) newProduct.PharmnetId = ConvertRowToInt(row, 4);
            if (ConvertRowToInt(row, 5) != 0) newProduct.StingId = ConvertRowToInt(row, 5);

            var sopharmaId = row.GetCell(6)?.ToString()?.TrimEnd();
            if (!string.IsNullOrEmpty(sopharmaId)) newProduct.SopharmaId = sopharmaId;

            await _productsService.CreateProduct(newProduct);
        }

        return JsonConvert.SerializeObject(errorDictionary);
    }

    [HttpPost]
    public async Task<string> Upload([FromBody] ProductInputModel productInputModel)
    {
        var outputProduct = new ProductOutputModel();

        if (string.IsNullOrEmpty(productInputModel.Name) || string.IsNullOrEmpty(productInputModel.ShortName) ||
            productInputModel.Price == 0 ||
            productInputModel.BrandexId == 0) return JsonConvert.SerializeObject(outputProduct);

        await _productsService.CreateProduct(productInputModel);

        return JsonConvert.SerializeObject(productInputModel);
    }
}
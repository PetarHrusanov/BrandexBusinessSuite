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

using static Common.ExcelDataConstants.ExcelLineErrors;
using static Common.Constants;

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

        if (file.Length <= 0 || Path.GetExtension(file.FileName)?.ToLower() != ".xlsx")
        {
            errorDictionary.Add(Errors.IncorrectFileFormat);
            return JsonConvert.SerializeObject(errorDictionary.ToArray());
        }
        
        var newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);
        var fullPath = Path.Combine(newPath, file.FileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var newProduct = new ProductInputModel();

            var nameRow = row.GetCell(0).ToString()?.TrimEnd();

            if (!string.IsNullOrEmpty(nameRow))
            {
                newProduct.Name = nameRow;
            }

            var nameShortRow = row.GetCell(1)?.ToString()?.TrimEnd();

            if (!string.IsNullOrEmpty(nameShortRow))
            {
                newProduct.ShortName = nameShortRow;
            }

            else
            {
                errorDictionary.Add($"{i} Line: {IncorrectProductId}");
            }

            var brandexId = row.GetCell(2)?.ToString()?.TrimEnd();

            if (int.TryParse(brandexId, out var brandexIdInt))
            {
                newProduct.BrandexId = brandexIdInt;
            }
                
            var phoenixId = row.GetCell(3)?.ToString()?.TrimEnd();

            if (!string.IsNullOrEmpty(phoenixId) && int.TryParse(phoenixId, out var phoenixIdInt))
            {
                newProduct.PhoenixId = phoenixIdInt;
            }

            var pharmnetId = row.GetCell(4)?.ToString()?.TrimEnd();

            if (!string.IsNullOrEmpty(pharmnetId)  && int.TryParse(pharmnetId, out var pharmnetIdInt))
            {
                newProduct.PharmnetId = pharmnetIdInt;
            }

            var stingId = row.GetCell(5)?.ToString()?.TrimEnd();

            if (!string.IsNullOrEmpty(stingId) && int.TryParse(stingId, out var stingIdInt))
            {
                newProduct.StingId = stingIdInt;
            }

            var sopharmaId = row.GetCell(6)?.ToString()?.TrimEnd();

            if (!string.IsNullOrEmpty(sopharmaId))
            {
                newProduct.SopharmaId = sopharmaId;
            }

            double.TryParse(row.GetCell(7)?.ToString()?.TrimEnd(), out var price);
            newProduct.Price = price;

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
        
        var newProduct = new ProductInputModel
        {
            Name = productInputModel.Name,
            ShortName = productInputModel.ShortName,
            Price = productInputModel.Price,
            BrandexId = productInputModel.BrandexId,
            PharmnetId = productInputModel.PharmnetId,
            PhoenixId = productInputModel.PhoenixId,
            SopharmaId = productInputModel.SopharmaId,
            StingId = productInputModel.StingId
        };

        if (await _productsService.CreateProduct(newProduct) != "")
        {
            outputProduct = new ProductOutputModel
            {
                Name = productInputModel.Name,
                ShortName = productInputModel.ShortName,
                Price = productInputModel.Price,
                BrandexId = productInputModel.BrandexId,
                SopharmaId = productInputModel.SopharmaId,
                PharmnetId = productInputModel.PharmnetId,
                PhoenixId = productInputModel.PhoenixId,
                StingId = productInputModel.StingId,
            };
        }

        return JsonConvert.SerializeObject(outputProduct);
    }
}
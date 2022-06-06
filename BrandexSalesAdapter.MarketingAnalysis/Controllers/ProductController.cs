namespace BrandexSalesAdapter.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Mvc;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexSalesAdapter.Controllers;
using BrandexSalesAdapter.Models;

using Infrastructure;
using Models.Products;
using Services.Products;

public class ProductController :AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IProductsService _productsService;

    public ProductController(
        IWebHostEnvironment hostEnvironment,
        IProductsService productsService
    )

    {
        _hostEnvironment = hostEnvironment;
        _productsService = productsService;
    }
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {

        string newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

        var productsCheck = await _productsService.GetCheckModels();

        var uniqueProducts = new List<ProductInputModel>();

        if (file.Length > 0)
        {

            var sFileExtension = Path.GetExtension(file.FileName)?.ToLower();

            if (file.FileName != null)
            {
                var fullPath = Path.Combine(newPath, file.FileName);

                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                stream.Position = 0;

                ISheet sheet;
                if (sFileExtension == ".xls")
                {

                    var hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  

                }

                else
                {

                    var hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

                }

                var headerRow = sheet.GetRow(0); //Get Header Row

                int cellCount = headerRow.LastCellNum;

                for (var j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);

                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                }

                for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                {

                    IRow row = sheet.GetRow(i);

                    if (row == null) continue;

                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    var newProduct = new ProductInputModel();

                    var nameRow = row.GetCell(0);

                    if (nameRow!=null)
                    {
                        newProduct.Name = nameRow.ToString()?.TrimEnd().ToUpper() ?? throw new InvalidOperationException();
                    }
                    
                    var shortNameRow = row.GetCell(1);

                    if (shortNameRow!=null)
                    {
                        newProduct.ShortName = shortNameRow.ToString()?.TrimEnd().ToUpper() ?? throw new InvalidOperationException();
                    }
                    
                    
                    if (productsCheck.All(c =>
                            !string.Equals(c.Name, newProduct.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        uniqueProducts.Add(newProduct);
                    }

                    else
                    { 
                        errorDictionary[i + 1] = "Incorrect Ad Media";
                    }

                }

                await _productsService.UploadBulk(uniqueProducts);

            }
        }

        var errorModel = new CustomErrorDictionaryOutputModel
        {
            Errors = errorDictionary
        };

        var outputSerialized = JsonConvert.SerializeObject(errorModel);

        return outputSerialized;

    }
    
}
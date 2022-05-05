using BrandexSalesAdapter.Infrastructure;

namespace BrandexSalesAdapter.ExcelLogic.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Models.Products;
using Services;
using Services.Products;
    
using Newtonsoft.Json;
    
using static Common.DataConstants.ExcelLineErrors;
    
using BrandexSalesAdapter.Controllers;
using BrandexSalesAdapter.Models;

public class ProductsController :AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IProductsService _productsService;

    private readonly INumbersChecker _numbersChecker;


    public ProductsController(
        IWebHostEnvironment hostEnvironment,
        IProductsService productsService,
        INumbersChecker numbersChecker)

    {

        _hostEnvironment = hostEnvironment;
        _productsService = productsService;
        _numbersChecker = numbersChecker;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm]IFormFile file)
    {

        string newPath = CreateExcelFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

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

                for (int j = 0; j < cellCount; j++)
                {
                    var cell = headerRow.GetCell(j);

                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;


                }

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                {

                    var row = sheet.GetRow(i);

                    if (row == null) continue;

                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    var newProduct = new ProductInputModel();
                    
                    var nameRow = row.GetCell(0).ToString()?.TrimEnd();

                    if (!string.IsNullOrEmpty(nameRow))
                    {
                        newProduct.Name = nameRow;
                    }
                    
                    var nameShortRow = row.GetCell(1).ToString()?.TrimEnd();

                    if (!string.IsNullOrEmpty(nameShortRow))
                    {
                        newProduct.ShortName = nameShortRow;
                    }
                    
                    else
                    {
                        errorDictionary[i+1] = IncorrectProductId;
                    }
                    
                    var brandexId = row.GetCell(2).ToString()?.TrimEnd();

                    if (_numbersChecker.WholeNumberCheck(brandexId))
                    {
                        newProduct.BrandexId = int.Parse(brandexId);
                    }
                    
                    else
                    {
                        errorDictionary[i+1] = IncorrectProductBrandexId;
                    }
                    
                    var phoenixId = row.GetCell(3).ToString()?.TrimEnd();

                    if (!string.IsNullOrEmpty(phoenixId))
                    {
                        if (_numbersChecker.WholeNumberCheck(phoenixId))
                        {
                            newProduct.PhoenixId = int.Parse(phoenixId);
                        }
                    
                        else
                        {
                            errorDictionary[i+1] = IncorrectProductPhoenixId;
                        }
                    }
                    
                    var pharmnetId = row.GetCell(4).ToString()?.TrimEnd();

                    if (!string.IsNullOrEmpty(pharmnetId))
                    {
                        if (_numbersChecker.WholeNumberCheck(pharmnetId))
                        {
                            newProduct.PharmnetId = int.Parse(pharmnetId);
                        }
                    
                        else
                        {
                            errorDictionary[i+1] = IncorrectProductPharmnetId;
                        }
                    }
                    
                    var stingId = row.GetCell(5).ToString()?.TrimEnd();

                    if (!string.IsNullOrEmpty(stingId))
                    {
                        if (_numbersChecker.WholeNumberCheck(stingId))
                        {
                            newProduct.StingId = int.Parse(stingId);
                        }
                    
                        else
                        {
                            errorDictionary[i+1] = IncorrectProductStingId;
                        }
                    }

                    var sopharmaId = row.GetCell(6).ToString()?.TrimEnd();

                    if (!string.IsNullOrEmpty(sopharmaId))
                    {
                        newProduct.SopharmaId = sopharmaId;
                    }
                    
                    var priceRow = row.GetCell(7).ToString()?.TrimEnd();

                    double.TryParse(priceRow, out var price); 

                    if (price!=0)
                    {
                        newProduct.Price = price;
                    }
                    
                    else
                    {
                        errorDictionary[i+1] = IncorrectPrice;
                    }

                    await _productsService.CreateProduct(newProduct);

                }
            }
        }

        var errorModel = new CustomErrorDictionaryOutputModel
        {
            Errors = errorDictionary
        };

        string outputSerialized = JsonConvert.SerializeObject(errorModel);

        return outputSerialized;

    }

    // [Authorize]
    [HttpPost]
    public async Task<string> Upload([FromBody]ProductInputModel productInputModel)
    {

        var outputProduct = new ProductOutputModel();
            
        if (!string.IsNullOrEmpty(productInputModel.Name) &&
            !string.IsNullOrEmpty(productInputModel.ShortName) && 
            productInputModel.Price != 0 && 
            productInputModel.BrandexId != 0)
        {
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

            if(await this._productsService.CreateProduct(newProduct)!= "")
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
                
        }
            
        var outputSerialized = JsonConvert.SerializeObject(outputProduct);

        return outputSerialized;
            
    }
}
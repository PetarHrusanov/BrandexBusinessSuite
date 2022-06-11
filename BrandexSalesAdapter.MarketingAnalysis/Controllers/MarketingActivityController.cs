using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace BrandexSalesAdapter.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Mvc;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexSalesAdapter.Controllers;
using BrandexSalesAdapter.Models;

using Models.MarketingActivities;
using Services.MarketingActivities;
using Services.Products;
using Infrastructure;
using Services.AdMedias;

using static Common.Constants;

public class MarketingActivityController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IMarketingActivitesService _marketingActivitesService;
    private readonly IProductsService _productsService;
    private readonly IAdMediasService _adMediasService;

    public MarketingActivityController(
        IWebHostEnvironment hostEnvironment,
        IMarketingActivitesService marketingActivitesService,
        IProductsService productsService,
        IAdMediasService adMediasService
    )

    {
        _hostEnvironment = hostEnvironment;
        _marketingActivitesService = marketingActivitesService;
        _productsService = productsService;
        _adMediasService = adMediasService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] MarketingBulkInputModel marketingBulkInputModel)
    {
        
        string dateFromClient = marketingBulkInputModel.Date;

        DateTime dateForDb = DateTime.ParseExact(dateFromClient, "dd-MM-yyyy", null);

        string sheetName = marketingBulkInputModel.Sheet;
            
        IFormFile file = marketingBulkInputModel.ImageFile;
        

        string newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

        // var adMediasCheck = await _marketingActivitesService.GetCheckModels();

        var marketingActivities = new List<MarketingActivityInputModel>();

        var products = await _productsService.GetCheckModels();
        var adMedias = await _adMediasService.GetCheckModels();
        

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

                    sheet = hssfwb.GetSheet(sheetName); //get first sheet from workbook   

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

                    var sumRow = row.GetCell(6);

                    if (sumRow == null) continue;
                    
                    var sumString = sumRow.ToString()!.TrimEnd();
                    
                    var productRow = row.GetCell(0);
                    if (productRow == null) continue;

                    var productString = productRow.ToString()!.TrimEnd().ToUpper();

                    if (!string.IsNullOrEmpty(sumString) & decimal.TryParse(sumString, out var sum) & !string.IsNullOrEmpty(productString))
                    {
                        var marketingActivityInput = new MarketingActivityInputModel();

                        marketingActivityInput.Price = sum;

                        marketingActivityInput.ProductId = products
                            .Where(p => p.Name == productString)
                            .Select(p => p.Id)
                            .FirstOrDefault();
                        
                        var adMediaRow = row.GetCell(1);

                        if (adMediaRow == null) throw new ArgumentException("GRESHNO ID BRAT");
                        
                        marketingActivityInput.AdMediaId = adMedias
                            .Where(p => p.Name == adMediaRow.ToString()!.TrimEnd().ToUpper())
                            .Select(p => p.Id)
                            .FirstOrDefault();
                            
                        var descriptionRow = row.GetCell(5);

                        if (descriptionRow == null) throw new ArgumentException("GRESHNO ID BRAT");

                        marketingActivityInput.Description = descriptionRow.ToString()!.TrimEnd();

                        marketingActivityInput.Date = dateForDb;

                        marketingActivities.Add(marketingActivityInput);

                    }

                    else
                    { 
                        errorDictionary[i + 1] = "Incorrect Ad Media";
                    }

                }

                await _marketingActivitesService.UploadBulk(marketingActivities);

            }
        }

        var errorModel = new CustomErrorDictionaryOutputModel
        {
            Errors = errorDictionary
        };

        var outputSerialized = JsonConvert.SerializeObject(errorModel);

        return outputSerialized;

    }

    [HttpPost]
    [Authorize(Roles = AdministratorRoleName)]
    public async Task<MarketingActivityModel[]> GetMarketingActivitiesByDate([FromBody] SingleStringInputModel singleStringInputModel)
    {
        var date = DateTime.ParseExact(singleStringInputModel.SingleStringValue, "MM-yyyy", CultureInfo.InvariantCulture);
        
        var marketingActivitiesArray = await _marketingActivitesService.GetMarketingActivitiesByDate(date);

        return marketingActivitiesArray;

    }
    
    

    // [HttpPost]
    // public async Task<string> Upload([FromBody] SingleStringInputModel singleStringInputModel)
    // {
    //     if (singleStringInputModel.SingleStringValue != null)
    //     {
    //         await _citiesService.UploadCity(singleStringInputModel.SingleStringValue);
    //     }
    //
    //     var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);
    //
    //     outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);
    //
    //     return outputSerialized;
    //
    // }
}
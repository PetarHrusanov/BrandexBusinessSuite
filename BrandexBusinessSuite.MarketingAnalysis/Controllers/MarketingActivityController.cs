namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using System.Globalization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models;

using Models.MarketingActivities;
using Services.MarketingActivities;
using Services.Products;
using Infrastructure;
using Services.AdMedias;

using static Methods.ExcelMethods;
using static Common.Constants;

public class MarketingActivityController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    private readonly IMarketingActivitesService _marketingActivitiesService;
    private readonly IProductsService _productsService;
    private readonly IAdMediasService _adMediasService;

    public MarketingActivityController(IWebHostEnvironment hostEnvironment,
        IMarketingActivitesService marketingActivitesService, IProductsService productsService,
        IAdMediasService adMediasService)

    {
        _hostEnvironment = hostEnvironment;
        _marketingActivitiesService = marketingActivitesService;
        _productsService = productsService;
        _adMediasService = adMediasService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Import([FromForm] MarketingBulkInputModel marketingBulkInputModel)
    {
        var dateForDb = DateTime.ParseExact(marketingBulkInputModel.Date, "dd-MM-yyyy", null);

        var sheetName = marketingBulkInputModel.Sheet;

        var file = marketingBulkInputModel.ImageFile;

        var errorDictionary = new List<string>();

        var marketingActivities = new List<MarketingActivityInputModel>();

        if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        var products = await _productsService.GetCheckModels();
        var adMedias = await _adMediasService.GetCheckModels();

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheet(sheetName);

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var sumRow = row.GetCell(6);

            if (sumRow == null) continue;

            var sumString = sumRow.ToString()!.TrimEnd();

            var productRow = row.GetCell(0);
            if (productRow == null) continue;

            var productString = productRow.ToString()!.TrimEnd().ToUpper();

            if (!string.IsNullOrEmpty(sumString) & decimal.TryParse(sumString, out var sum) &
                !string.IsNullOrEmpty(productString))
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
                errorDictionary.Add($"{i} Line: Error");
            }
        }

        await _marketingActivitiesService.UploadBulk(marketingActivities);

        return JsonConvert.SerializeObject(errorDictionary.ToArray());
    }

    [HttpPost]
    [Authorize(Roles = AdministratorRoleName)]
    public async Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(
        [FromBody] SingleStringInputModel singleStringInputModel)
    {
        var date = DateTime.ParseExact(singleStringInputModel.SingleStringValue, "MM-yyyy",
            CultureInfo.InvariantCulture);

        var marketingActivitiesArray = await _marketingActivitiesService.GetMarketingActivitiesByDate(date);

        return marketingActivitiesArray;
    }
}
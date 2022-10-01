namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using System.Globalization;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.ErpDocuments;
using Models.MediaTypes;
using BrandexBusinessSuite.Services;

using Models.MarketingActivities;
using Services.MarketingActivities;
using Services.MediaTypes;
using Services.Products;
using Infrastructure;
using Services.AdMedias;

using static Common.Constants;
using static Common.ErpConstants;

using static Methods.ExcelMethods;
using static Requests.RequestsMethods;
using static Methods.FieldsValuesMethods;

public class MarketingActivityController : ApiController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    private readonly IMarketingActivitesService _marketingActivitiesService;
    private readonly IProductsService _productsService;
    private readonly IAdMediasService _adMediasService;
    private readonly IMediaTypesService _mediaTypesService;
    
    private readonly ErpUserSettings _userSettings;

    private static readonly HttpClient Client = new();

    public MarketingActivityController(IWebHostEnvironment hostEnvironment,
        IOptions<ErpUserSettings> userSettings,
        IMarketingActivitesService marketingActivitesService, IProductsService productsService,
        IAdMediasService adMediasService,
        IMediaTypesService mediaTypesService)

    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _marketingActivitiesService = marketingActivitesService;
        _productsService = productsService;
        _adMediasService = adMediasService;
        _mediaTypesService = mediaTypesService;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<List<MediaTypesCheckModel>> GetAdMediaTypes()
    {
        return await _mediaTypesService.GetCheckModels();
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [Route(Id)]
    public async Task<ActionResult<MarketingActivityEditModel>> Details(int id)
        => await _marketingActivitiesService.GetDetails(id) ?? throw new InvalidOperationException();
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadMarketingActivity(MarketingActivityInputModel inputModel)
    {
        await _marketingActivitiesService.UploadMarketingActivity(inputModel);
        return Result.Success;
    }
    
    [HttpPut]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult<MarketingActivityEditModel>> Edit(MarketingActivityEditModel input)
        => await _marketingActivitiesService.Edit(input) ?? throw new InvalidOperationException();

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
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
        var mediaTypes = await _mediaTypesService.GetCheckModels();

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
                    .Where(p => string.Equals(p.Name, productString, StringComparison.CurrentCultureIgnoreCase))
                    .Select(p => p.Id)
                    .FirstOrDefault();

                var adMediaRow = row.GetCell(1);

                if (adMediaRow == null) throw new ArgumentException("GRESHNO ID BRAT");

                var adMediaString = adMediaRow.ToString()!.TrimEnd().ToUpper();
                
                marketingActivityInput.AdMediaId = adMedias
                    .Where(p => string.Equals(p.Name, adMediaString, StringComparison.CurrentCultureIgnoreCase))
                    .Select(p => p.Id)
                    .FirstOrDefault();

                var activityType = row.GetCell(2);
                if (activityType == null) throw new ArgumentException("GRESHNO ID BRAT");

                var activityTypeString = activityType.ToString()!.TrimEnd().ToUpper();
                
                marketingActivityInput.MediaTypeId = mediaTypes
                    .Where(p => string.Equals(p.Name, activityTypeString, StringComparison.CurrentCultureIgnoreCase))
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

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(string dateFormatted)
    {
        var date = DateTime.ParseExact(dateFormatted, "MM-yyyy",
            CultureInfo.InvariantCulture);

        var marketingActivitiesArray = await _marketingActivitiesService.GetMarketingActivitiesByDate(date);

        return marketingActivitiesArray;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task PostMarketingActivitiesToErp(int id)
    {

        var marketingActivity = await _marketingActivitiesService.GetDetailsErp(id);
        
        var monthErp = ReturnValueByClassAndName(typeof(ErpMonths), marketingActivity.Date.ToString("MMMM"));
        var yearErp = marketingActivity.Date.ToString("yyyy");

        var activityObject = new MarketingActivityCm(
            $"Задача / {marketingActivity.CompanyName}",
            marketingActivity.Date,
            marketingActivity.CompanyErpId,
            monthErp,
            yearErp,
            "",
            marketingActivity.Description,
            marketingActivity.MediaType,
            marketingActivity.AdMedia,
            decimal.ToDouble(marketingActivity.Price),
            marketingActivity.ProductName);

        var jsonPostString = JsonConvert.SerializeObject(activityObject, Formatting.Indented);

        var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.User}:{_userSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await  JObjectByUriPostRequest(Client, $"{ErpRequests.BaseUrl}General_Contacts_Activities/", jsonPostString);

        var documentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();

        await ChangeStateToRelease(Client, documentId);

        await _marketingActivitiesService.ErpPublishMarketingActivity(id);

    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> PayMarketingActivity(int id)
    {

        await _marketingActivitiesService.PayMarketingActivity(id);
        return Result.Success;

    }
}
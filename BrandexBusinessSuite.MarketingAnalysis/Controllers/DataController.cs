namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using BrandexBusinessSuite.Controllers;
using Models.MarketingActivities;
using Models.MediaTypes;
using Services.AdMedias;
using Services.MarketingActivities;
using Services.MediaTypes;
using Services.Products;
using BrandexBusinessSuite.Services;

using static Methods.ExcelMethods;
using static Common.Constants;

public class DataController : ApiController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    private readonly IMarketingActivitesService _marketingActivitiesService;
    private readonly IProductsService _productsService;
    private readonly IAdMediasService _adMediasService;
    private readonly IMediaTypesService _mediaTypesService;

    private readonly ErpUserSettings _userSettings;

    private static readonly HttpClient Client = new();

    public DataController(IWebHostEnvironment hostEnvironment,
        IOptions<ErpUserSettings> userSettings,
        IMarketingActivitesService marketingActivitiesService, IProductsService productsService,
        IAdMediasService adMediasService,
        IMediaTypesService mediaTypesService
    )

    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _marketingActivitiesService = marketingActivitiesService;
        _productsService = productsService;
        _adMediasService = adMediasService;
        _mediaTypesService = mediaTypesService;
    }
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadMarketingMedia(MarketingActivityInputModel inputModel)
    {
        await _marketingActivitiesService.UploadMarketingActivity(inputModel);
        return Result.Success;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<MediaTypesCheckModel>> GetAdMediaTypes() 
        => await _mediaTypesService.GetCheckModels();
    
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    [Route(Id)]
    public async Task<ActionResult<MarketingActivityEditModel>> Details(int id)
        => await _marketingActivitiesService.GetDetails(id) ?? throw new InvalidOperationException();
    
}
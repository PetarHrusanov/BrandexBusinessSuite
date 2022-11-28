using BrandexBusinessSuite.MarketingAnalysis.Models.Products;

namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.DataModels;
using Models.MarketingActivities;

using Services.Companies;
using Services.AdMedias;
using Services.MarketingActivities;
using Services.MediaTypes;
using Services.Products;
using BrandexBusinessSuite.Services;

using static Common.Constants;

public class DataController : ApiController
{

    private readonly IMarketingActivitesService _marketingActivitiesService;
    private readonly IProductsService _productsService;
    private readonly IAdMediasService _adMediasService;
    private readonly IMediaTypesService _mediaTypesService;
    private readonly ICompaniesService _companiesService;

    private readonly ErpUserSettings _userSettings;

    private static readonly HttpClient Client = new();

    public DataController(IOptions<ErpUserSettings> userSettings,
        IMarketingActivitesService marketingActivitiesService, IProductsService productsService,
        IAdMediasService adMediasService,
        IMediaTypesService mediaTypesService,
        ICompaniesService companiesService
    )

    {
        _userSettings = userSettings.Value;
        _marketingActivitiesService = marketingActivitiesService;
        _productsService = productsService;
        _adMediasService = adMediasService;
        _mediaTypesService = mediaTypesService;
        _companiesService = companiesService;
    }
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadCompany(BasicErpInputModel inputModel)
    {
        await _companiesService.Upload(inputModel);
        return Result.Success;
    }
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadAdMedia(BasicCheckModel inputModel)
    {
        await _adMediasService.Upload(inputModel);
        return Result.Success;
    }
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadProduct(ProductInputModel inputModel)
    {
        await _productsService.Upload(inputModel);
        return Result.Success;
    }

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    [Route(Id)]
    public async Task<ActionResult<MarketingActivityEditModel>> Details(int id)
        => await _marketingActivitiesService.GetDetails(id) ?? throw new InvalidOperationException();

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<BasicCheckModel>> GetCompanies()
    {
        var companiesGet = await _companiesService.GetCheckModels();
        return companiesGet.Select(company => new BasicCheckModel() { Name = company.Name, Id = company.Id }).ToList();
    }
    
}
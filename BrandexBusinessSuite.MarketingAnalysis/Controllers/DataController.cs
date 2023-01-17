namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Services;
using Models.AdMedias;
using Models.Products;
using Services.Companies;
using Services.AdMedias;
using Services.Products;

using static Common.Constants;

public class DataController : ApiController
{
    
    private readonly IProductsService _productsService;
    private readonly IAdMediasService _adMediasService;
    private readonly ICompaniesService _companiesService;

    public DataController(IProductsService productsService, IAdMediasService adMediasService, ICompaniesService companiesService)

    {
        _productsService = productsService;
        _adMediasService = adMediasService;
        _companiesService = companiesService;
    }

    // COMPANY LOGIC
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadCompany(BasicErpInputModel inputModel)
    {
        await _companiesService.Upload(inputModel);
        return Result.Success;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<BasicCheckModel>> GetCompanies() 
        =>  await _companiesService.GetCheckModels();
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    [Route(Id)]
    public async Task<ActionResult<BasicCheckErpModel>> CompanyDetails(int id)
        => await _companiesService.Details(id) ?? throw new InvalidOperationException();
    
    [HttpPut]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult<BasicCheckErpModel>> CompanyEdit(BasicCheckErpModel input)
        => await _companiesService.Edit(input) ?? throw new InvalidOperationException();
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> CompanyDelete([FromForm] int id)
    {
        await _companiesService.Delete(id);
        return Result.Success;
    }
    

    // AD MEDIA LOGIC
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadAdMedia(BasicCheckModel inputModel)
    {
        await _adMediasService.Upload(inputModel);
        return Result.Success;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<List<AdMediaDisplayModel>> GetAdMedias() 
        => await _adMediasService.GetDisplayModels();

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    [Route(Id)]
    public async Task<ActionResult<AdMediaCheckModel>> AdMediaDetails(int id)
        => await _adMediasService.GetDetails(id) ?? throw new InvalidOperationException();
    
    [HttpPut]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult<AdMediaCheckModel>> AdMediaEdit(AdMediaCheckModel input)
        => await _adMediasService.Edit(input) ?? throw new InvalidOperationException();
    
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> AdMediaDelete([FromForm] int id)
    {
        await _adMediasService.Delete(id);
        return Result.Success;
    }
    

    // PRODUCT LOGIC
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult> UploadProduct(ProductInputModel inputModel)
    {
        await _productsService.Upload(inputModel);
        return Result.Success;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<List<ProductCheckModel>> GetProducts() 
        => await _productsService.GetCheckModels();
    
}
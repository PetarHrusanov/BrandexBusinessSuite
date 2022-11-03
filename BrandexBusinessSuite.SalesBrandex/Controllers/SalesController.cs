namespace BrandexBusinessSuite.SalesBrandex.Controllers;

using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using BrandexBusinessSuite.Services;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.ErpDocuments;
using Models.PharmacyCompanies;
using Services.Pharmacies;
using Services.PharmacyCompanies;
using BrandexBusinessSuite.Models.DataModels;
using Services.Cities;
using Services.PharmacyChains;

using static  Common.Constants;
using static Common.ErpConstants;
using static Requests.RequestsMethods;

public class SalesController : ApiController
{
    private readonly ErpUserSettings _erpUserSettings;
    private static readonly HttpClient Client = new();
    
    private readonly IPharmaciesService _pharmaciesService;
    private readonly IPharmacyCompaniesService _companiesService;
    private readonly ICitiesService _citiesService;
    private readonly IPharmacyChainsService _pharmacyChainsService;

    public SalesController(IOptions<ErpUserSettings> erpUserSettings,
        IPharmaciesService pharmaciesService,
        IPharmacyCompaniesService companiesService,
        ICitiesService citiesService,
        IPharmacyChainsService pharmacyChainsService
        )
    {
        _erpUserSettings = erpUserSettings.Value;
        _pharmaciesService = pharmaciesService;
        _companiesService = companiesService;
        _citiesService = citiesService;
        _pharmacyChainsService = pharmacyChainsService;
    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> GetSales()
    {
        var citiesCheck = await _citiesService.GetCitiesCheck();
        var companiesCheck = await _companiesService.GetPharmacyCompaniesCheck();
        var pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();

        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        string query =
            "Crm_Sales_SalesOrders?$top=10000&$filter=CreationTime%20ge%202022-10-01T00:00:00.000Z%20and%20CreationTime%20le%202022-10-31T00:00:00.000Z&$select=DocumentDate,Id,ShipToCustomer&$expand=Lines($expand=Product($select=Id,Name);$select=Id,LineAmount,Product,Quantity),ShipToCustomer($expand=Party($select=CustomProperty_KN,CustomProperty_RETREG,PartyCode,PartyName);$select=CustomProperty_GRAD_u002DKLIENT,CustomProperty_Klas_u0020Klient,CustomProperty_STOR3,Id),ShipToPartyContactMechanism($expand=ContactMechanism;$select=ContactMechanism),ToParty($select=PartyId,PartyName)";
        
        var responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}{query}");
        var orderAnalyses = JsonConvert.DeserializeObject<List<ErpSalesOrderAnalysis>>(responseContentJObj["value"].ToString());

        var citiesSales = orderAnalyses
            .Where(c=>c.ShipToCustomer?.City?.Value != null)
            .DistinctBy(c=>c.ShipToCustomer.City.ValueId)
            .Select(c => new BasicErpInputModel()
            {
                Name = c.ShipToCustomer.City.Value,
                ErpId = c.ShipToCustomer.City.ValueId,
            }).ToList();

        var citiesNew = (from city in citiesSales 
            where citiesCheck.All(c => !string.Equals(c.ErpId, city.ErpId, StringComparison.CurrentCultureIgnoreCase)) 
            select city).ToList();
        await _citiesService.UploadBulk(citiesNew);
        citiesCheck = await _citiesService.GetCitiesCheck();

        var pharmacyChainsSales = orderAnalyses
            .Where(c=>c.ShipToCustomer?.PharmacyChain?.Value!=null)
            .DistinctBy(c=>c.ShipToCustomer.PharmacyChain.ValueId)
            .Select(c => new BasicErpInputModel()
            {
                Name = c.ShipToCustomer.PharmacyChain.Value,
                ErpId = c.ShipToCustomer.PharmacyChain.ValueId
            }).ToList();
        var pharmacyChainsNew = (from pharmacyChain in pharmacyChainsSales 
            where pharmacyChainsCheck.All(c => !string.Equals(c.ErpId, pharmacyChain.ErpId, StringComparison.CurrentCultureIgnoreCase)) 
            select pharmacyChain).ToList();
        await _pharmacyChainsService.UploadBulk(pharmacyChainsNew);
        pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();

        var companiesSales = orderAnalyses.Select(c => new PharmacyCompanyInputModel()
        {
            Name = c.ToParty.PartyName.BG,
            ErpId = c.ToParty.PartyId
        }).DistinctBy(c=>c.ErpId).ToList();
        var companiesNew = (from company in companiesSales 
            where companiesCheck.All(c => !string.Equals(c.ErpId, company.ErpId, StringComparison.CurrentCultureIgnoreCase)) 
            select company).ToList();
        await _companiesService.UploadBulk(companiesNew);
        companiesCheck = await _companiesService.GetPharmacyCompaniesCheck();

        return Result.Success;

    }
    
}
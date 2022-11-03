using BrandexBusinessSuite.Models.Dates;

namespace BrandexBusinessSuite.SalesBrandex.Controllers;

using System.Text;
using System.Globalization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using BrandexBusinessSuite.Services;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Models.DataModels;
using Models.Sales;
using Data.Enums;
using Models.Pharmacies;
using Models.PharmacyCompanies;
using Services.Regions;
using Services.Pharmacies;
using Services.PharmacyCompanies;
using Services.Cities;
using Services.PharmacyChains;
using Services.Products;
using Services.Sales;

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
    private readonly IRegionsService _regionsService;
    private readonly IProductsService _productsService;
    private readonly ISalesService _salesService;

    private const string PersonalClient = "Personal Client";

    public SalesController(IOptions<ErpUserSettings> erpUserSettings,
        IPharmaciesService pharmaciesService,
        IPharmacyCompaniesService companiesService,
        ICitiesService citiesService,
        IPharmacyChainsService pharmacyChainsService,
        IRegionsService regionsService,
        IProductsService productsService,
        ISalesService salesService
        )
    {
        _erpUserSettings = erpUserSettings.Value;
        _pharmaciesService = pharmaciesService;
        _companiesService = companiesService;
        _citiesService = citiesService;
        _pharmacyChainsService = pharmacyChainsService;
        _regionsService = regionsService;
        _productsService = productsService;
        _salesService = salesService;
    }
    
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> GetSales([FromBody] DateStartEndInputModel dateStartEndInputModel)
    {
        var citiesCheck = await _citiesService.GetCitiesCheck();
        var companiesCheck = await _companiesService.GetPharmacyCompaniesCheck();
        var pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        var regionsCheck = await _regionsService.GetRegionsCheck();
        var pharmaciesCheck = await _pharmaciesService.GetPharmaciesCheck();
        var productsCheck = await _productsService.GetProductsCheck();

        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var dateStart = dateStartEndInputModel.DateStart.ToString("yyyy-MM-ddThh:mm:ss'Z'", CultureInfo.InvariantCulture);
        var dateEnd = dateStartEndInputModel.DateEnd.ToString("yyyy-MM-ddThh:mm:ss'Z'", CultureInfo.InvariantCulture);

        string queryDate =
            $"Crm_Sales_SalesOrders?$top=10000&$filter=CreationTime%20ge%20{dateStart}%20and%20CreationTime%20le%20{dateEnd}&$select=DocumentDate,Id,ShipToCustomer&$expand=Lines($expand=Product($select=Id,Name);$select=Id,LineAmount,Product,Quantity),ShipToCustomer($expand=Party($select=CustomProperty_RETREG,PartyCode,PartyName);$select=CustomProperty_GRAD_u002DKLIENT,CustomProperty_Klas_u0020Klient,CustomProperty_STOR3,Id),ShipToPartyContactMechanism($expand=ContactMechanism;$select=ContactMechanism),ToParty($select=PartyId,PartyName)";
        
        var responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}{queryDate}");
        
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
        
        var regionsSales = orderAnalyses
            .Where(c=>c.ShipToCustomer?.Party?.Region.Value!=null)
            .DistinctBy(c=>c.ShipToCustomer?.Party?.Region.ValueId)
            .Select(c => new BasicErpInputModel()
            {
                Name = c.ShipToCustomer?.Party?.Region.Value,
                ErpId = c.ShipToCustomer?.Party?.Region.ValueId
            }).ToList();
        var regionsNew = (from region in regionsSales 
            where regionsCheck.All(c => !string.Equals(c.ErpId, region.ErpId, StringComparison.CurrentCultureIgnoreCase)) 
            select region).ToList();
        await _regionsService.UploadBulk(regionsNew);
        regionsCheck = await _regionsService.GetRegionsCheck();
        
        var queryCompanyLocations=
            "General_Contacts_CompanyLocations?$top=600000&$select=Id,LocationName,PartyCode";
        
        responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}{queryCompanyLocations}");
        var companyLocations = JsonConvert.DeserializeObject<List<ErpGeneralContactsCompanyLocations>>(responseContentJObj["value"].ToString());

        var pharmaciesSales = new List<PharmacyDbInputModel>();

        var ordersDistinctClients = orderAnalyses.DistinctBy(c => c.ShipToCustomer?.Id).ToList();

        foreach (var order in ordersDistinctClients)
        {
            var newPharmacy = new PharmacyDbInputModel()
            {
                BrandexId = 0,
                ErpId = PersonalClient,
                Name = PersonalClient,
                PartyCode = PersonalClient,
                Address = "",
                PharmacyClass = PharmacyClass.Other,
            };

            if (order.ShipToCustomer!=null)
            {
                newPharmacy.ErpId = order.ShipToCustomer.Id;
            }

            if (order.ShipToCustomer?.Party.PartyCode!=null)
            {
                newPharmacy.BrandexId = int.Parse(order.ShipToCustomer?.Party?.PartyCode ?? string.Empty);
                newPharmacy.Name = companyLocations
                    .Where(c => c.PartyCode == order.ShipToCustomer?.Party?.PartyCode)
                    .Select(c => c.LocationName.BG).FirstOrDefault();
            }
            if (order.ShipToCustomer?.Party?.Region.ValueId!=null)
            {
                newPharmacy.RegionId = regionsCheck.Where(c => c.ErpId == order.ShipToCustomer?.Party?.Region.ValueId)
                    .Select(c => c.Id).FirstOrDefault();
            }
            
            
            if (order.ShipToCustomer?.Class?.Value!=null)
            {
                newPharmacy.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass),
                    order.ShipToCustomer.Class?.Value.TrimEnd() ?? string.Empty, true);
            }

            if (order.ToParty?.PartyId!=null)
            {
                newPharmacy.CompanyId = companiesCheck.Where(c => c.ErpId == order.ToParty.PartyId)
                    .Select(c => c.Id).FirstOrDefault();
            }
            
            if (order.ShipToCustomer?.PharmacyChain?.ValueId!=null)
            {
                newPharmacy.PharmacyChainId = pharmacyChainsCheck.Where(c => c.ErpId == order.ShipToCustomer.PharmacyChain?.ValueId)
                    .Select(c => c.Id).FirstOrDefault();
            }
            
            if (order.ShipToCustomer?.City?.ValueId!=null)
            {
                newPharmacy.CityId = citiesCheck.Where(c => c.ErpId == order.ShipToCustomer.City?.ValueId)
                    .Select(c => c.Id).FirstOrDefault();
            }

            if (order.ShipToPartyContactMechanism?.ContactMechanism?.Name!=null)
            {
                newPharmacy.Address = order.ShipToPartyContactMechanism.ContactMechanism.Name;
            }
            
            pharmaciesSales.Add(newPharmacy);
        }

        var pharmaciesNew = (from pharmacy in pharmaciesSales 
            where pharmaciesCheck.All(c => !string.Equals(c.ErpId, pharmacy.ErpId, StringComparison.CurrentCultureIgnoreCase)) 
            select pharmacy).ToList();
        await _pharmaciesService.UploadBulk(pharmaciesNew);
        
        pharmaciesCheck = await _pharmaciesService.GetPharmaciesCheck();

        var salesNew = new List<SaleDbInputModel>();

        foreach (var sale in orderAnalyses)
        {
            Console.WriteLine(sale.Id);
            foreach (var line in sale.Lines)
            {
                var newSale = new SaleDbInputModel()
                {
                    ErpId = sale.Id,
                    ProductId = productsCheck.Where(p=>p.ErpId==line.Product.Id).Select(p=>p.Id).FirstOrDefault(),
                    Date = DateTime.ParseExact(sale.DocumentDate, "yyyy-MM-ddThh:mm:ss'Z'", CultureInfo.InvariantCulture),
                    Count = Convert.ToInt32(line.Quantity.Value)
                };

                if (sale.ShipToCustomer!=null)
                {
                    newSale.PharmacyId = pharmaciesCheck
                        .Where(p => p.ErpId == sale.ShipToCustomer.Id)
                        .Select(p => p.Id)
                        .FirstOrDefault();
                }
                else
                {
                    newSale.PharmacyId = pharmaciesCheck
                        .Where(p => p.ErpId == PersonalClient)
                        .Select(p => p.Id)
                        .FirstOrDefault();
                }
                salesNew.Add(newSale);
            }
        }

        await _salesService.UploadBulk(salesNew);

        return Result.Success;

    }
    
}
namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Services;
using Common;
using Data.Enums;
using Infrastructure;

using Models.Pharmacies;

using Services.Cities;
using Services.Pharmacies;
using Services.PharmacyChains;
using Services.Regions;
using Services.PharmacyCompanies;

using static Methods.ExcelMethods;
using static Requests.RequestsMethods;
using static Common.ExcelDataConstants.Ditributors;

public class PharmaciesController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    
    private readonly ErpUserSettings _erpUserSettings;
    private static readonly HttpClient Client = new();
    
    private readonly IPharmaciesService _pharmaciesService;
    private readonly IPharmacyCompaniesService _pharmacyCompaniesService;
    private readonly IRegionsService _regionsService;
    private readonly IPharmacyChainsService _pharmacyChainsService;
    private readonly ICitiesService _citiesService;

    // private const int BrandexIdColumn = 19;
    // private const int NameColumn = 4;
    // private const int PharmacyClassColumn = 2;
    // private const int ActiveColumn = 1;
    // private const int PharmacyCompanyColumn = 3;
    // private const int PharmacyChainColumn = 5;
    // private const int AddressColumn = 6;
    // private const int CityColumn = 20;
    // private const int PharmnetIdColumn = 14;
    // private const int PhoenixIdColumn = 15;
    // private const int SopharmaIdColumn = 16;
    // private const int StingIdColumn = 17;
    // private const int RegionColumn = 8;

    public PharmaciesController(IWebHostEnvironment hostEnvironment, IOptions<ErpUserSettings> erpUserSettings, IPharmaciesService pharmaciesService,
        IPharmacyCompaniesService pharmacyCompaniesService, IPharmacyChainsService pharmacyChainsService,
        IRegionsService regionsService, ICitiesService citiesService)

    {
        _hostEnvironment = hostEnvironment;
        _erpUserSettings = erpUserSettings.Value;
        _pharmaciesService = pharmaciesService;
        _pharmacyCompaniesService = pharmacyCompaniesService;
        _pharmacyChainsService = pharmacyChainsService;
        _regionsService = regionsService;
        _citiesService = citiesService;
    }
    
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<List<PharmacyDisplayModel>> Check([FromForm] PharmacyExcelInputModel inputModel)
    {

        if (!CheckXlsx(inputModel.ImageFile)) throw new ArgumentNullException("No file.");

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, inputModel.ImageFile);
        
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await inputModel.ImageFile.CopyToAsync(stream);
        
        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);
        
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var pharmaciesErp = await GetPharmaciesErp(false);

        var pharmaciesFile = new List<PharmacyDisplayModel>();

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var pharmacyDisplay = inputModel.Distributor switch
            {
                Phoenix => CreatePharmacyDisplayModel(row, 2, 3, 9, 10, 5, 7),
                Sting => CreatePharmacyDisplayModel(row, 6, 7, 9, 5, 8, null),
                Pharmnet => CreatePharmacyDisplayModel(row, 4, 5, 7, 6, 8, null),
                Sopharma => CreatePharmacyDisplayModel(row, 5, 6, 8, 4, 7, 9),
                _ => new PharmacyDisplayModel()
            };

            if (pharmaciesFile.All(x => x.Code != pharmacyDisplay.Code))
            {
                pharmaciesFile.Add(pharmacyDisplay);
            }
        }

        var pharmaciesErpSelected = new List<string>();
        
        switch(inputModel.Distributor)
        {
            case Phoenix:
                pharmaciesErpSelected = pharmaciesErp.Where(p => p.PhoenixId is { Value: { } }).Select(p=>p.PhoenixId.Value).ToList();
                break;
            case Sting:
                pharmaciesErpSelected = pharmaciesErp.Where(p => p.StingId is { Value: { } }).Select(p=>p.StingId.Value).ToList();
                break;
            case Pharmnet:
                pharmaciesErpSelected = pharmaciesErp.Where(p => p.PharmnetId is { Value: { } }).Select(p=>p.PharmnetId.Value).ToList();
                break;
            case Sopharma:
                pharmaciesErpSelected = pharmaciesErp.Where(p => p.SopharmaId is { Value: { } }).Select(p=>p.SopharmaId.Value).ToList();
                break;
        }

        return pharmaciesFile.Where(pharmacy => !pharmaciesErpSelected.Contains(pharmacy.Code)).ToList();
    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> AddErpIds()
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        
        var pharmaciesErp = await GetPharmaciesErp(true);

        var regionsCheck = await _regionsService.AllRegions();
        var regionsErpDistinct = pharmaciesErp!.Where(c=>c.Region!=null).DistinctBy(c => c.Region!.ValueId).ToList();
        var regionsForUpdate = regionsCheck.Select(region => new BasicCheckErpModel()
            {
                Id = region.Id,
                Name = region.Name,
                ErpId = regionsErpDistinct.Where(r => r.Region!.Value!.ToUpper().TrimEnd()
                        .Equals(region.Name, StringComparison.InvariantCultureIgnoreCase))
                    .Select(r => r.Region!.ValueId)
                    .FirstOrDefault()
            })
            .ToList();
        await _regionsService.BulkUpdateData(regionsForUpdate);

        var citiesCheck = await _citiesService.GetCitiesCheck();

        var citiesErpDistinct = await GetCitiesErp(false);
        var citiesForUpdate = (from city in citiesCheck
            let erpCity = citiesErpDistinct.FirstOrDefault(c => c.City!.Value!.TrimEnd().Equals(city!.Name!.TrimEnd(), StringComparison.InvariantCultureIgnoreCase)) 
            where erpCity != null
            select new BasicCheckErpModel { Id = city.Id, Name = city.Name!.ToUpper().TrimEnd(), ErpId = erpCity!.City!.ValueId! }).ToList();
        await _citiesService.BulkUpdateData(citiesForUpdate);

        var pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        var pharmacyChainsErpDistinct = pharmaciesErp.Where(c => c.PharmacyChain != null).DistinctBy(c => c.PharmacyChain.ValueId).ToList();

        var pharmacyChainsForUpdate = (from pharmacyChain in pharmacyChainsCheck
            let pharmacyChainErp = pharmacyChainsErpDistinct.FirstOrDefault(c => string.Equals(c.PharmacyChain!.Value!.TrimEnd().ToUpper(), pharmacyChain.Name, StringComparison.InvariantCultureIgnoreCase)) 
            where pharmacyChainErp != null
            select new BasicCheckErpModel { Id = pharmacyChain.Id, Name = pharmacyChain.Name!.ToUpper().TrimEnd(), ErpId = pharmacyChainErp!.PharmacyChain!.ValueId! }).ToList();
        await _pharmacyChainsService.BulkUpdateData(pharmacyChainsForUpdate);
        
        var pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesErpCheck();
        var pharmacyCompaniesErpDistinct = pharmaciesErp!.Where(c => c.ParentParty != null).DistinctBy(c => c.ParentParty.PartyId).ToList();
        var pharmacyCompaniesForUpdate = (from pharmacyCompany in pharmacyCompaniesErpCheck
            let pharmacyCompanyErp = pharmacyCompaniesErpDistinct.FirstOrDefault(c => string.Equals(c.ParentParty!.PartyName!.BG.TrimEnd(), pharmacyCompany.Name!.TrimEnd(), StringComparison.InvariantCultureIgnoreCase)) 
            where pharmacyCompanyErp != null
            select new BasicCheckErpModel { Id = pharmacyCompany.Id, Name = pharmacyCompany.Name!.ToUpper().TrimEnd(), ErpId = pharmacyCompanyErp!.ParentParty!.PartyId! }).ToList();
        await _pharmacyCompaniesService.BulkUpdateData(pharmacyCompaniesForUpdate);
        
        var pharmaciesErpCheck = await _pharmaciesService.GetPharmaciesCheck();
        var pharmaciesErpDistinct = pharmaciesErp!
            .Where(c => c.PartyId != null)
            .Where(p=>p.Address!=null && (bool)p.IsActive!).DistinctBy(c => c.PartyId).ToList();
        var pharmaciesForUpdate = (from pharmacy in pharmaciesErpCheck
            let pharmacyErp = pharmaciesErpDistinct.FirstOrDefault(c =>int.Parse(c.PartyCode!)== pharmacy.BrandexId) 
            where pharmacyErp != null
            select new BasicCheckErpModel { Id = pharmacy.Id, Name = pharmacy.Name!.ToUpper().TrimEnd(), ErpId = pharmacyErp!.PartyId }).ToList();
        await _pharmaciesService.BulkUpdateData(pharmaciesForUpdate);
        
        return Result.Success;
    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> AddNewErpData()
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var pharmaciesErp = await GetPharmaciesErp(true);
        
        var citiesCheck = await _citiesService.GetCitiesCheck();
        var citiesErpDistinct = await GetCitiesErp(false);
        var citiesNew = citiesErpDistinct.Where(c => citiesCheck.All(i => i.ErpId != c.City!.ValueId)).ToList();
        await _citiesService.UploadBulkFromErp(citiesNew);
        citiesCheck = await _citiesService.GetCitiesCheck();

        var pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        var pharmacyChainsErpDistinct = pharmaciesErp.Where(c => c.PharmacyChain != null).DistinctBy(c => c.PharmacyChain.ValueId).ToList();
        var pharmacyChainsNew = pharmacyChainsErpDistinct
            .Where(p => pharmacyChainsCheck.All(i => i.ErpId != p.PharmacyChain!.ValueId)).ToList();
        await _pharmacyChainsService.UploadBulkFromErp(pharmacyChainsNew);
        pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        
        var pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesErpCheck();
        var pharmacyCompaniesErpDistinct = pharmaciesErp!.Where(c => c.ParentParty != null).DistinctBy(c => c.ParentParty.PartyId).ToList();
        var pharmacyCompaniesNew = pharmacyCompaniesErpDistinct
            .Where(p => pharmacyCompaniesErpCheck.All(i => i.ErpId != p.ParentParty!.PartyId)).ToList();
        await _pharmacyCompaniesService.UploadBulkFromErp(pharmacyCompaniesNew);
        pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesErpCheck();
        
        var pharmaciesErpCheck = await _pharmaciesService.GetPharmaciesCheck();
        var pharmaciesErpDistinct = pharmaciesErp!.Where(c => c.PartyId != null)
            .Where(p=>p.Address!=null && (bool)p.IsActive!).DistinctBy(c => c.PartyId).ToList();
        var pharmaciesNew = pharmaciesErpDistinct
            .Where(p=>p.Address!=null && p.Address.Value!="#N/A"
                                      && !p.LocationName!.BG.Contains("дублиран")
                                      && p.ParentParty!.PartyId!="2f1d45bf-4fdc-40a9-ba80-d1f32d3a2dd5" 
                                      && p.ParentParty!.PartyId!="3690661b-6cb7-4c9a-87ed-0325112c5647"
                                      && p.ParentParty!.PartyId!="6984be81-6ff4-4575-96a6-239acf35e898"
                                      && p.ParentParty!.PartyId!="964765e7-793a-4b42-b5eb-222bfcd3c00f"
                                      && p.ParentParty!.PartyId!="e7468f59-febd-4ec8-9d25-62b0f498d4d5"
                                      && p.ParentParty!.PartyId!="69b4999d-4f8a-46f0-9fb3-0c3808fa8ed7"
                                      && p.ParentParty!.PartyId!="cccb03e5-3396-43d7-8439-ce28f61f874b"
                                      && p.ParentParty!.PartyId!="26db4076-155b-4d79-bc73-2514e1fa3142"
                                      && p.ParentParty!.PartyId!="068dd627-7837-4dac-8e0e-a34c16717599"
            )
            .Where(p => pharmaciesErpCheck.All(i => i.ErpId != p.PartyId)).ToList();
        
        var citiesErpWithPartyId = await GetCitiesErp(true);
        var regions = await _regionsService.AllRegions();

        var pharmaciesForUpload = new List<PharmacyDbInputModel>();

        foreach (var pharmacy in pharmaciesNew)
        {
            Console.WriteLine(pharmacy.PartyCode);
            
            var cityErpId = citiesErpWithPartyId.Where(c => c.Party!.PartyId==pharmacy.PartyId)
                .Select(c => c.City!.ValueId).FirstOrDefault();
            
            var pharmacyInput = new PharmacyDbInputModel
            {
                BrandexId = int.Parse(pharmacy.PartyCode!),
                Name = pharmacy.LocationName!.BG,
                PharmacyClass = PharmacyClass.Other,
                Address = pharmacy.Address!.Value,
                Active = (bool)pharmacy.IsActive!,
                CompanyId = pharmacyCompaniesErpCheck.Where(p=>p.ErpId==pharmacy.ParentParty!.PartyId).Select(p=>p.Id).FirstOrDefault(),
                CityId = citiesCheck.Where(c=>c.ErpId==cityErpId).Select(c=>c.Id).FirstOrDefault(),
                RegionId = regions.Where(r=>r.ErpId==pharmacy.Region!.ValueId).Select(r=>r.Id).FirstOrDefault(),
                ErpId = pharmacy.PartyId
            };

            if (pharmacy.PharmacyChain == null) throw new Exception($"{pharmacy.LocationName!.BG} doesn't have a chain");

            if (pharmacy.Class!=null)
            {
                pharmacyInput.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacy.Class.Value!.TrimEnd(), true);
            }

            pharmacyInput.PharmacyChainId = pharmacyChainsCheck.Where(p => p.ErpId == pharmacy.PharmacyChain.ValueId)
                .Select(p => p.Id).FirstOrDefault();

            if (pharmacy.PharmnetId is { Value: { } }) pharmacyInput.PharmnetId = int.Parse(pharmacy.PharmnetId.Value);
            if (pharmacy.PhoenixId is { Value: { } }) pharmacyInput.PhoenixId = int.Parse(pharmacy.PhoenixId.Value);
            if (pharmacy.SopharmaId is { Value: { } }) pharmacyInput.SopharmaId = int.Parse(pharmacy.SopharmaId.Value);
            if (pharmacy.StingId is { Value: { } }) pharmacyInput.StingId = int.Parse(pharmacy.StingId.Value);
            
            pharmaciesForUpload.Add(pharmacyInput);
        }

        await _pharmaciesService.UploadBulk(pharmaciesForUpload);

        return Result.Success;
    }

    private static async Task<List<ErpCityCheck>> GetCitiesErp(bool withIds)
    {
        var citiesQuery = "Crm_Customers?$top=60000&$select=CustomProperty_GRAD_u002DKLIENT";
        JObject responseContentJObj;
        List<ErpCityCheck> citiesErp;
        if (withIds)
        {
            citiesQuery = "Crm_Customers?$top=60000&$select=CustomProperty_GRAD_u002DKLIENT,Id&$expand=Party($select=PartyId)";
            responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpConstants.ErpRequests.BaseUrl}{citiesQuery}");
            citiesErp = JsonConvert.DeserializeObject<List<ErpCityCheck>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
            return citiesErp!.Where(c => c.City is { Value: { } }).ToList();
        }
        responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpConstants.ErpRequests.BaseUrl}{citiesQuery}");
        citiesErp = JsonConvert.DeserializeObject<List<ErpCityCheck>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
        return citiesErp!.Where(c => c.City is { Value: { } }).DistinctBy(c => c.City.ValueId).ToList();
    }

    private static async Task<List<ErpPharmacyCheck>> GetPharmaciesErp(bool filtered)
    {
        var pharmaciesErpQuery = filtered ? "General_Contacts_CompanyLocations?$top=60000&$filter=PartyUpdateTime%20ge%202019-01-01T00:00:00.000Z&$select=CustomProperty_ADDRES,CustomProperty_ID_u002DA_u002DKI_u002DFarmnet,CustomProperty_ID_u002DA_u002DKI_u002DPhoenix,CustomProperty_ID_u002DA_u002DKI_u002DSOPHARMA,CustomProperty_ID_u002DA_u002DKI_u002DSting,CustomProperty_RETREG,CustomProperty_%D0%92%D0%95%D0%A0%D0%98%D0%93%D0%90,CustomProperty_%D0%9A%D0%9B%D0%90%D0%A1,Id,IsActive,LocationName,PartyCode,PartyId&$expand=ParentParty($select=PartyId,PartyName)" : "General_Contacts_CompanyLocations?$top=60000&$select=CustomProperty_ADDRES,CustomProperty_ID_u002DA_u002DKI_u002DFarmnet,CustomProperty_ID_u002DA_u002DKI_u002DPhoenix,CustomProperty_ID_u002DA_u002DKI_u002DSOPHARMA,CustomProperty_ID_u002DA_u002DKI_u002DSting,CustomProperty_RETREG,CustomProperty_%D0%92%D0%95%D0%A0%D0%98%D0%93%D0%90,CustomProperty_%D0%9A%D0%9B%D0%90%D0%A1,Id,IsActive,LocationName,PartyCode,PartyId&$expand=ParentParty($select=PartyId,PartyName)";
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpConstants.ErpRequests.BaseUrl}{pharmaciesErpQuery}");
        return JsonConvert.DeserializeObject<List<ErpPharmacyCheck>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
    }

    private static PharmacyDisplayModel CreatePharmacyDisplayModel(IRow row, int codeRow, int nameRow, int addressRow, int cityRow, int vatRow, int? chainRow)
    {
        var pharmacy = new PharmacyDisplayModel();
        var codeCell = row.GetCell(codeRow);
        if (codeCell!=null) pharmacy.Code = codeCell.ToString()?.TrimEnd();
        
        var nameCell = row.GetCell(nameRow);
        if (nameCell!=null) pharmacy.Name = nameCell.ToString()?.TrimEnd();

        var addressCell = row.GetCell(addressRow);
        if (addressCell!=null) pharmacy.Address = addressCell.ToString()?.TrimEnd();

        var cityCell = row.GetCell(cityRow);
        if (cityCell!=null) pharmacy.City = cityCell.ToString()?.TrimEnd();

        var vatCell = row.GetCell(vatRow);
        if (vatCell!=null) pharmacy.Vat = vatCell.ToString()?.TrimEnd();

        if (chainRow == null) return pharmacy;
        var chainCell = row.GetCell((int)chainRow);
        if (chainCell!=null) pharmacy.PharmacyChain = chainCell.ToString()?.TrimEnd();
        
        return pharmacy;
    }
    
    // [HttpPost]
    // [Consumes("multipart/form-data")]
    // public async Task<ActionResult<string>> Import([FromForm] IFormFile file)
    // {
    //     var errorDictionary = new List<string>();
    //
    //     var validPharmacyList = new List<PharmacyDbInputModel>();
    //     var pharmaciesEdited = new List<PharmacyDbInputModel>();
    //
    //     var citiesIdsForCheck = await _citiesService.GetCitiesCheck();
    //     var pharmacyCompaniesIdsForCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesCheck();
    //     var pharmacyChainsIdsForCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
    //     var regionIdsForCheck = await _regionsService.AllRegions();
    //
    //     var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();
    //
    //     if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);
    //
    //     var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);
    //     
    //     await using var stream = new FileStream(fullPath, FileMode.Create);
    //     await file.CopyToAsync(stream);
    //     
    //     stream.Position = 0;
    //
    //     var hssfwb = new XSSFWorkbook(stream);
    //     var sheet = hssfwb.GetSheetAt(0);
    //
    //     for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
    //     {
    //         var row = sheet.GetRow(i);
    //
    //         if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;
    //
    //         if (row.GetCell(NameColumn) == null || row.GetCell(AddressColumn) == null)
    //         {
    //             errorDictionary.Add($"{i} Line: Null or blank Name or Address");
    //             continue;
    //         }
    //
    //         var newPharmacy = new PharmacyDbInputModel
    //         {
    //             PharmacyClass = PharmacyClass.Other,
    //             Active = true,
    //             Name = row.GetCell(NameColumn).ToString()?.TrimEnd(),
    //             Address = row.GetCell(AddressColumn).ToString()?.TrimEnd()
    //         };
    //
    //         CreatePharmacyInputModel(citiesIdsForCheck, pharmacyCompaniesIdsForCheck, pharmacyChainsIdsForCheck,
    //             regionIdsForCheck, newPharmacy, row, i, errorDictionary);
    //
    //         if (newPharmacy.BrandexId == 0 || newPharmacy.Name == null || newPharmacy.PharmacyChainId == 0 ||
    //             newPharmacy.RegionId == 0 || newPharmacy.CityId == 0 || newPharmacy.CompanyId == 0)
    //             continue;
    //
    //         if (pharmacyIdsForCheck.Any(p => p.BrandexId == newPharmacy.BrandexId))
    //         {
    //             pharmaciesEdited.Add(newPharmacy);
    //             continue;
    //         }
    //
    //         validPharmacyList.Add(newPharmacy);
    //     }
    //
    //     if (errorDictionary.Count != 0) return JsonConvert.SerializeObject(errorDictionary);
    //     
    //     await _pharmaciesService.UploadBulk(validPharmacyList);
    //     await _pharmaciesService.Update(pharmaciesEdited);
    //
    //     return JsonConvert.SerializeObject(errorDictionary);
    // }
    

    // private static void CreatePharmacyInputModel(IEnumerable<BasicCheckErpModel> citiesIdsForCheck,
    //     IEnumerable<PharmacyCompanyCheckModel> pharmacyCompanyIdsForCheck,
    //     IEnumerable<BasicCheckErpModel> pharmacyChainsIdsForCheck,
    //     IEnumerable<BasicCheckErpModel> regionIdsForCheck, PharmacyDbInputModel newPharmacy, IRow row, int i,
    //     ICollection<string> errorDictionary)
    // {
    //     var brandexId = row.GetCell(BrandexIdColumn);
    //     var pharmacyClass = row.GetCell(PharmacyClassColumn);
    //     var pharmacyActive = row.GetCell(ActiveColumn);
    //     var companyIdRow = row.GetCell(PharmacyCompanyColumn);
    //     var chainIdRow = row.GetCell(PharmacyChainColumn);
    //     var regionIdRow = row.GetCell(RegionColumn);
    //
    //     var cityIdRow = row.GetCell(CityColumn);
    //
    //     if (brandexId == null || companyIdRow == null || chainIdRow == null || regionIdRow == null || cityIdRow == null)
    //     {
    //         errorDictionary.Add($"{i} Line: Null or blank value at a necessary field ");
    //         return;
    //     }
    //
    //     if (int.TryParse(brandexId.ToString()!.TrimEnd(), out var brandexIdConverted))
    //     {
    //         newPharmacy.BrandexId = brandexIdConverted;
    //     }
    //
    //     else
    //     {
    //         errorDictionary.Add($"{i} Line: {IncorrectPharmacyId}");
    //     }
    //
    //     if (pharmacyClass != null && !string.IsNullOrWhiteSpace(pharmacyClass.ToString()!.TrimEnd()))
    //     {
    //         newPharmacy.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacyClass.ToString()!.TrimEnd(), true);
    //     }
    //
    //     if (pharmacyActive != null && pharmacyActive.ToString()?.TrimEnd()[0] == '0') newPharmacy.Active = false;
    //
    //     newPharmacy.CompanyId = pharmacyCompanyIdsForCheck
    //         .Where(p => p.Name == companyIdRow.ToString()!.TrimEnd().ToUpper())
    //         .Select(p => p.Id)
    //         .FirstOrDefault();
    //
    //     if (newPharmacy.CompanyId == 0) errorDictionary.Add($"{i} Line: {IncorrectPharmacyCompanyId}");
    //
    //     newPharmacy.PharmacyChainId = pharmacyChainsIdsForCheck
    //         .Where(p => p.Name == chainIdRow.ToString()?.TrimEnd().ToUpper())
    //         .Select(p => p.Id)
    //         .FirstOrDefault();
    //
    //     if (newPharmacy.PharmacyChainId == 0) errorDictionary.Add($"{i} Line: {IncorrectPharmacyChainId}");
    //
    //     newPharmacy.RegionId = regionIdsForCheck.Where(r => r.Name == regionIdRow.ToString()!.TrimEnd())
    //         .Select(r => r.Id)
    //         .FirstOrDefault();
    //
    //     if (newPharmacy.RegionId == 0) errorDictionary.Add($"{i} Line: {IncorrectRegion}");
    //
    //     newPharmacy.CityId = citiesIdsForCheck.Where(c => c.Name == cityIdRow.ToString()!.TrimEnd().ToUpper())
    //         .Select(c => c.Id)
    //         .FirstOrDefault();
    //
    //     if (newPharmacy.CityId == 0) errorDictionary.Add($"{i} Line: {IncorrectCityName}");
    //
    //     if (ConvertRowToInt(row, PharmnetIdColumn) != 0) newPharmacy.PharmnetId = ConvertRowToInt(row, PharmnetIdColumn);
    //     if (ConvertRowToInt(row, PhoenixIdColumn) != 0) newPharmacy.PhoenixId = ConvertRowToInt(row, PhoenixIdColumn);
    //     if (ConvertRowToInt(row, SopharmaIdColumn) != 0) newPharmacy.SopharmaId = ConvertRowToInt(row, SopharmaIdColumn);
    //     if (ConvertRowToInt(row, StingIdColumn) != 0) newPharmacy.StingId = ConvertRowToInt(row, StingIdColumn);
    // }
    
    
}
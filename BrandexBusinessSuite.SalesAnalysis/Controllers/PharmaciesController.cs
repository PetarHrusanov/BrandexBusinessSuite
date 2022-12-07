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

        if (!CheckXlsx(inputModel.ImageFile)) throw new Exception("No file.");

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
    public async Task<ActionResult> UpdateDatabase()
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var pharmaciesErp = await GetPharmaciesErp(true);
        
        var citiesCheck = await _citiesService.GetAllCheck();
        var citiesErpDistinct = await GetCitiesErp(false);
        var citiesNew = citiesErpDistinct.Where(c => citiesCheck.All(i => i.ErpId != c.City!.ValueId)).ToList();
        await _citiesService.UploadBulk(citiesNew);
        citiesCheck = await _citiesService.GetAllCheck();

        var pharmacyChainsCheck = await _pharmacyChainsService.GetAllCheck();
        var pharmacyChainsErpDistinct = pharmaciesErp.Where(c => c.PharmacyChain != null).DistinctBy(c => c.PharmacyChain.ValueId).ToList();
        var pharmacyChainsNew = pharmacyChainsErpDistinct
            .Where(p => pharmacyChainsCheck.All(i => i.ErpId != p.PharmacyChain!.ValueId)).ToList();
        await _pharmacyChainsService.UploadBulk(pharmacyChainsNew);
        pharmacyChainsCheck = await _pharmacyChainsService.GetAllCheck();
        
        var pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetAllCheck();
        var pharmacyCompaniesErpDistinct = pharmaciesErp!.Where(c => c.ParentParty != null).DistinctBy(c => c.ParentParty.PartyId).ToList();
        var pharmacyCompaniesNew = pharmacyCompaniesErpDistinct
            .Where(p => pharmacyCompaniesErpCheck.All(i => i.ErpId != p.ParentParty!.PartyId)).ToList();
        await _pharmacyCompaniesService.UploadBulk(pharmacyCompaniesNew);
        pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetAllCheck();
        
        var pharmaciesErpCheck = await _pharmaciesService.GetAllCheck();
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
        var regions = await _regionsService.GetAllCheck();

        var pharmaciesForUpload = new List<PharmacyDbInputModel>();

        foreach (var pharmacy in pharmaciesNew)
        {
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

        pharmaciesErp = await GetPharmaciesErp(true);

        var regionsCheck = await _regionsService.GetAllCheck();
        var regionsErpDistinct = pharmaciesErp!.Where(c=>c.Region is { Value: { } }).DistinctBy(c => c.Region!.ValueId).ToList();
        var regionsForUpdate = (from region in regionsErpDistinct 
            where regionsCheck.All(r => region.Region != null && r.Name!.ToUpper().TrimEnd() != region.Region.Value!.ToUpper().TrimEnd()) 
            select new BasicCheckErpModel
            {
                    Id = regionsCheck.Where(r => r.ErpId!.Equals(region.Region!.ValueId!)).Select(r => r.Id).FirstOrDefault(), 
                    Name = region.Region!.Value!.ToUpper().TrimEnd(),
                    ErpId = region.Region.ValueId })
            .ToList();
        await _regionsService.BulkUpdateData(regionsForUpdate);

        citiesCheck = await _citiesService.GetAllCheck();
        citiesErpDistinct = await GetCitiesErp(false);
        var citiesForUpdate = (from city in citiesErpDistinct
            where citiesCheck.All(c => c.Name!.ToUpper().TrimEnd() != city.City!.Value!.ToUpper().TrimEnd())
            select new BasicCheckErpModel
            {
                Id = citiesCheck.Where(c => c.ErpId!.Equals(city.City!.ValueId!.TrimEnd()))
                    .Select(c => c.Id)
                    .FirstOrDefault(),
                Name = city.City!.Value!.ToUpper().TrimEnd(),
                ErpId = city.City.ValueId!.TrimEnd(),
            }).ToList();
        await _citiesService.BulkUpdateData(citiesForUpdate);
    
        pharmacyChainsCheck = await _pharmacyChainsService.GetAllCheck();
        pharmacyChainsErpDistinct = pharmaciesErp.Where(c => c.PharmacyChain is { Value: { } }).DistinctBy(c => c.PharmacyChain.ValueId).ToList();
        var pharmacyChainsForUpdate = (from pharmacyChain in pharmacyChainsErpDistinct
            where pharmacyChainsCheck.All(ph => ph.Name!.ToUpper().TrimEnd() != pharmacyChain.PharmacyChain!.Value!.ToUpper().TrimEnd()) 
            select new BasicCheckErpModel
            {
                Id = pharmacyChainsCheck.FirstOrDefault(c => string.Equals(c.ErpId!.TrimEnd(), pharmacyChain.PharmacyChain!.ValueId!.TrimEnd(), StringComparison.InvariantCultureIgnoreCase))!.Id, 
                Name = pharmacyChain.PharmacyChain!.Value!.ToUpper().TrimEnd(), 
                ErpId = pharmacyChain.PharmacyChain!.ValueId!.TrimEnd()
            }).ToList();
        await _pharmacyChainsService.BulkUpdateData(pharmacyChainsForUpdate);
        
        var pharmacyCompaniesCheck = await _pharmacyCompaniesService.GetAllCheck();
        pharmacyCompaniesErpDistinct = pharmaciesErp!.Where(c => c.ParentParty?.PartyName?.BG != null && c.ParentParty.PartyId!=null).DistinctBy(c => c.ParentParty.PartyId).ToList();
        var pharmacyCompaniesForUpdate = (from pharmacyCompany in pharmacyCompaniesErpDistinct
            where pharmacyCompaniesCheck.All(pc => pc.Name!.ToUpper().TrimEnd() != pharmacyCompany.ParentParty!.PartyName!.BG!.ToUpper().TrimEnd()) 
            select new BasicCheckErpModel
            {
                Id = pharmacyCompaniesCheck.FirstOrDefault(pc => pc.ErpId == pharmacyCompany.ParentParty!.PartyId!.TrimEnd())!.Id,
                Name = pharmacyCompany.ParentParty!.PartyName!.BG!.ToUpper().TrimEnd(),
                ErpId = pharmacyCompany.ParentParty.PartyId!.TrimEnd()
            }).ToList();
        await _pharmacyCompaniesService.BulkUpdateData(pharmacyCompaniesForUpdate);
        
        var pharmaciesCheck = await _pharmaciesService.GetAllCheckErp();
        pharmaciesCheck = pharmaciesCheck.Where(p => p.ErpId != "e6400a83-398c-496d-bf7d-558104e978a3").ToList();

        pharmaciesErpDistinct = pharmaciesErp!.Where(c => c.PartyId != null && c.Address!=null).DistinctBy(c => c.PartyId).ToList();

        var pharmaciesForUpdate = new List<PharmacyDbUpdateModel>();
        
        foreach (var pharmacy in pharmaciesCheck)
        {
            var pharmacyChanged = new PharmacyDbUpdateModel()
            {
                Id = pharmacy.Id,
                Address = pharmacy.Address,
                CompanyId = pharmacy.CompanyId,
                Name = pharmacy.Name,
                PharmacyChainId = pharmacy.PharmacyChainId,
                PharmnetId = pharmacy.PharmnetId,
                PhoenixId = pharmacy.PhoenixId,
                RegionId = pharmacy.RegionId,
                SopharmaId = pharmacy.SopharmaId,
                StingId = pharmacy.StingId,
                ModifiedOn = DateTime.Now
            };

            if (pharmaciesErpDistinct.All(p => p.PartyId != pharmacy.ErpId)) continue;
            var pharmacyErp = pharmaciesErpDistinct.FirstOrDefault(p => p.PartyId == pharmacy.ErpId);

            if (pharmacyErp is { LocationName: { } } && pharmacyErp.LocationName.BG! != pharmacy.Name) pharmacyChanged.Name = pharmacyErp.LocationName.BG.ToUpper().TrimEnd();
            if (pharmacyErp is { Address: { } } && pharmacyErp.Address.Value != pharmacy.Address) pharmacyChanged.Address = pharmacyErp.Address.Value.ToUpper().TrimEnd();

            if (pharmacyErp?.PhoenixId?.Value != null
                && pharmacyErp.PhoenixId.Value!= " "
                && !string.IsNullOrEmpty(pharmacyErp.PhoenixId.Value) 
                && int.Parse(pharmacyErp.PhoenixId.Value.TrimEnd())!=pharmacy.PhoenixId )
            {
                pharmacyChanged.PhoenixId = int.Parse(pharmacyErp.PhoenixId.Value.TrimEnd());
            }

            if (pharmacyErp?.PharmnetId?.Value != null
                && pharmacyErp.PharmnetId.Value!= " "
                && !string.IsNullOrEmpty(pharmacyErp.PharmnetId.Value) 
                && int.TryParse(pharmacyErp.PharmnetId.Value.TrimEnd(), out int pharmnetParsed)
                && int.Parse(pharmacyErp.PharmnetId.Value.TrimEnd())!=pharmacy.PharmnetId )
            {
                pharmacyChanged.PharmnetId = pharmnetParsed;
            }
            
            if (pharmacyErp?.SopharmaId?.Value != null
                && pharmacyErp.SopharmaId.Value!= " "
                && !string.IsNullOrEmpty(pharmacyErp.SopharmaId?.Value) 
                && int.TryParse(pharmacyErp.SopharmaId.Value.TrimEnd(), out var sopharmaParsed) 
                && int.Parse(pharmacyErp.SopharmaId?.Value.TrimEnd())!=pharmacy.SopharmaId )
            {
                pharmacyChanged.SopharmaId = sopharmaParsed;
            }
            
            if (pharmacyErp?.StingId?.Value != null
                && pharmacyErp.StingId.Value!= " "
                && !string.IsNullOrEmpty(pharmacyErp.StingId.Value) 
                && int.TryParse(pharmacyErp.StingId.Value.TrimEnd(), out int stingParsed) 
                && int.Parse(pharmacyErp.StingId.Value.TrimEnd())!=pharmacy.StingId )
            {
                pharmacyChanged.StingId = stingParsed;
            }

            if (pharmacyErp.ParentParty.PartyId!=null && pharmacyErp.ParentParty.PartyId.TrimEnd()!=pharmacy.CompanyIdErp)
            {
                pharmacyChanged.CompanyId = pharmacyCompaniesCheck.Where(p => p.ErpId == pharmacyErp.ParentParty!.PartyId!.TrimEnd())
                    .Select(p => p.Id).FirstOrDefault();
            }
            
            if (pharmacyErp.Region.ValueId!=null && pharmacyErp.Region?.ValueId!=pharmacy.RegionErp)
            {
                pharmacyChanged.RegionId = regionsCheck.Where(p => p.ErpId == pharmacyErp.Region?.ValueId!.TrimEnd())
                    .Select(p => p.Id).FirstOrDefault();
            }
            
            if (pharmacyErp.PharmacyChain?.ValueId!=null && pharmacyErp.PharmacyChain.ValueId!=pharmacy.PharmacyChainErp)
            {
                pharmacyChanged.PharmacyChainId = pharmacyChainsCheck.Where(p => p.ErpId == pharmacyErp.PharmacyChain!.ValueId!.TrimEnd())
                    .Select(p => p.Id).FirstOrDefault();
            }

            if (pharmacy.Address!=pharmacyChanged.Address || pharmacy.Name!=pharmacyChanged.Name || pharmacy.PharmnetId!=pharmacyChanged.PharmnetId
                || pharmacy.PhoenixId!=pharmacyChanged.PhoenixId || pharmacy.RegionId != pharmacyChanged.RegionId || pharmacy.SopharmaId != pharmacyChanged.SopharmaId
                || pharmacy.StingId != pharmacyChanged.StingId || pharmacy.CompanyId != pharmacyChanged.CompanyId || pharmacy.PharmacyChainId != pharmacyChanged.PharmacyChainId
                )
            {
                pharmaciesForUpdate.Add(pharmacyChanged);
            }
        }

        await _pharmaciesService.BulkUpdateData(pharmaciesForUpdate);

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
}
namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

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

        AuthenticateUserBasicHeader(Client, _erpUserSettings.User, _erpUserSettings.Password);

        var pharmaciesFile = new  HashSet<PharmacyDisplayModel>();

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
            
            pharmaciesFile.Add(pharmacyDisplay);
        }
        
        var pharmaciesErp = await GetPharmaciesErp(false);
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
        
        // OPTIMISATION VERSION
        // var pharmaciesErpSelected = pharmaciesErp.SelectMany(p => p.GetType()
        //         .GetProperties()
        //         .Where(x => x.Name.Equals($"{inputModel.Distributor}Id"))
        //         .Select(x => x.GetValue(p))
        //         .Where(x => x != null)
        //         .Select(x => x.ToString()))
        //     .ToList();
        
        return pharmaciesFile.Where(pharmacy => !pharmaciesErpSelected.Contains(pharmacy.Code)).ToList();
    }
    
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> UpdateDatabase()
    {
        AuthenticateUserBasicHeader(Client, _erpUserSettings.User, _erpUserSettings.Password);

        var pharmaciesErp = await GetPharmaciesErp(true);
        
        var citiesCheck = await _citiesService.GetAllCheck();
        var citiesErpDistinct = await GetCitiesErp(false);
        var citiesNew = citiesErpDistinct.Where(c => citiesCheck.All(i => i.ErpId != c.City!.ValueId)).ToList();
        await _citiesService.UploadBulk(citiesNew);
        citiesCheck = await _citiesService.GetAllCheck();
        var citiesForUpdate = citiesErpDistinct
            .Select(c => c.City)
            .Where(c => c != null)
            .Distinct()
            .Where(c => citiesCheck.All(cc => cc.Name!.ToUpper().TrimEnd() != c.Value!.ToUpper().TrimEnd()))
            .Select(c => new BasicCheckErpModel
            {
                Id = (int)citiesCheck.FirstOrDefault(cc => cc.ErpId == c.ValueId!.TrimEnd())?.Id!,
                Name = c.Value!.ToUpper().TrimEnd(),
                ErpId = c.ValueId!.TrimEnd(),
            })
            .ToList();
        await _citiesService.BulkUpdateData(citiesForUpdate);

        var pharmacyChainsCheck = await _pharmacyChainsService.GetAllCheck();
        var pharmacyChainsErpDistinct = pharmaciesErp.Where(c => c.PharmacyChain?.Value != null & c.PharmacyChain?.ValueId!=null).DistinctBy(c => c.PharmacyChain.ValueId).ToList();
        var pharmacyChainsNew = pharmacyChainsErpDistinct
            .Where(p => pharmacyChainsCheck.All(i=> !string.Equals(i.ErpId, p.PharmacyChain!.ValueId, StringComparison.InvariantCultureIgnoreCase))).ToList();
        await _pharmacyChainsService.UploadBulk(pharmacyChainsNew);
        pharmacyChainsCheck = await _pharmacyChainsService.GetAllCheck();
        var pharmacyChainsForUpdate = (from pharmacyChain in pharmacyChainsErpDistinct
            where pharmacyChainsCheck.All(ph => ph.Name!.ToUpper().TrimEnd() != pharmacyChain.PharmacyChain!.Value!.ToUpper().TrimEnd()) 
            select new BasicCheckErpModel
            {
                Id = pharmacyChainsCheck.FirstOrDefault(c => string.Equals(c.ErpId!.TrimEnd(), pharmacyChain.PharmacyChain!.ValueId!.TrimEnd(), StringComparison.InvariantCultureIgnoreCase))!.Id, 
                Name = pharmacyChain.PharmacyChain!.Value!.ToUpper().TrimEnd(), 
                ErpId = pharmacyChain.PharmacyChain!.ValueId!.TrimEnd()
            }).ToList();
        await _pharmacyChainsService.BulkUpdateData(pharmacyChainsForUpdate);
        
        var pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetAllCheck();
        var pharmacyCompaniesErpDistinct = pharmaciesErp!.Where(c => c.ParentParty?.PartyName?.BG != null && c.ParentParty.PartyId!=null).DistinctBy(c => c.ParentParty.PartyId).ToList();
        var pharmacyCompaniesNew = pharmacyCompaniesErpDistinct
            .Where(p => pharmacyCompaniesErpCheck.All(i => i.ErpId != p.ParentParty!.PartyId)).ToList();
        await _pharmacyCompaniesService.UploadBulk(pharmacyCompaniesNew);
        pharmacyCompaniesErpCheck = await _pharmacyCompaniesService.GetAllCheck();
        var pharmacyCompaniesForUpdate = (from pharmacyCompany in pharmacyCompaniesErpDistinct
            where pharmacyCompaniesErpCheck.All(pc => pc.Name!.ToUpper().TrimEnd() != pharmacyCompany.ParentParty!.PartyName!.BG!.ToUpper().TrimEnd()) 
            select new BasicCheckErpModel
            {
                Id = pharmacyCompaniesErpCheck.FirstOrDefault(pc => pc.ErpId == pharmacyCompany.ParentParty!.PartyId!.TrimEnd())!.Id,
                Name = pharmacyCompany.ParentParty!.PartyName!.BG!.ToUpper().TrimEnd(),
                ErpId = pharmacyCompany.ParentParty.PartyId!.TrimEnd()
            }).ToList();
        await _pharmacyCompaniesService.BulkUpdateData(pharmacyCompaniesForUpdate);
        
        var regionsCheck = await _regionsService.GetAllCheck();
        var regionsForUpdate = pharmaciesErp
            .Select(c => c.Region)
            .Where(r => r != null)
            .Distinct()
            .Where(r => regionsCheck.All(rc => rc.Name?.ToUpper().TrimEnd() != r.Value?.ToUpper().TrimEnd()))
            .Select(r => new BasicCheckErpModel
            {
                Id = (int)regionsCheck.FirstOrDefault(rc => rc.ErpId == r.ValueId)?.Id!,
                Name = r.Value!.ToUpper().TrimEnd(),
                ErpId = r.ValueId
            })
            .ToList();
        await _regionsService.BulkUpdateData(regionsForUpdate);
        
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

        if (pharmaciesNew.Any(pharmacy => pharmacy.PharmacyChain == null))
            return Result.Failure("Some pharmacies don't have a chain");
        if (pharmaciesNew.Any(pharmacy => pharmacy.ParentParty!.PartyId == null)) 
            return Result.Failure("Some pharmacies don't have a companies");
        if (pharmaciesNew.Any(pharmacy => pharmacy.Region!.ValueId == null)) 
            return Result.Failure("Some pharmacies don't have a region");

        var pharmaciesErpDict = pharmaciesErpDistinct.ToDictionary(p => p.PartyId);
        var pharmacyCompaniesDict = pharmaciesErpCheck.ToDictionary(p => p.ErpId);
        var regionsCheckDict = regionsCheck.ToDictionary(p => p.ErpId);
        var pharmacyChainDict = pharmacyChainsCheck.DistinctBy(c => c.ErpId).ToList().ToDictionary(p => p.ErpId);
        
        var pharmaciesForUpload = pharmaciesNew.Select(pharmacy => new PharmacyDbInputModel
        {
            BrandexId = int.Parse(pharmacy.PartyCode!),
            Name = pharmacy.LocationName!.BG,
            PharmacyClass = pharmacy.Class != null ? (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacy.Class.Value!.TrimEnd(), true) : PharmacyClass.Other,
            Address = pharmacy.Address!.Value,
            Active = (bool)pharmacy.IsActive!,
            CompanyId = pharmacyCompaniesDict.TryGetValue(pharmacy?.ParentParty.PartyId.TrimEnd(), out var company) ? company.Id :0 ,
            CityId = citiesCheck.FirstOrDefault(c => c.ErpId == citiesErpWithPartyId.FirstOrDefault(c => c.Party!.PartyId == pharmacy.PartyId)?.City!.ValueId)?.Id ?? 0,
            RegionId = regionsCheckDict.TryGetValue(pharmacy.Region.ValueId, out var region) ? region.Id : 0,
            ErpId = pharmacy.PartyId,
            PharmacyChainId = pharmacyChainDict.TryGetValue(pharmacy.PharmacyChain.ValueId, out var pharmacyChain) ? pharmacyChain.Id : 0,
            PharmnetId = pharmacy.PharmnetId?.Value != null ? int.Parse(pharmacy.PharmnetId.Value) : (int?)null,
            PhoenixId = pharmacy.PhoenixId?.Value != null ? int.Parse(pharmacy.PhoenixId.Value) : (int?)null,
            SopharmaId = pharmacy.SopharmaId?.Value != null ? int.Parse(pharmacy.SopharmaId.Value) : (int?)null,
            StingId = pharmacy.StingId?.Value != null ? int.Parse(pharmacy.StingId.Value) : (int?)null,
        }).ToList();

        await _pharmaciesService.UploadBulk(pharmaciesForUpload);

        var pharmaciesCheck = await _pharmaciesService.GetAllCheckErp();
        pharmaciesCheck = pharmaciesCheck.Where(p => p.ErpId != "e6400a83-398c-496d-bf7d-558104e978a3").ToList();
        

        var pharmaciesForUpdate = new List<PharmacyDbUpdateModel>();

        foreach (var pharmacy in pharmaciesCheck)
        {
            var pharmacyErp = pharmaciesErpDict.GetValueOrDefault(pharmacy.ErpId);
            if (pharmacyErp == null) continue;

            var pharmacyChanged = new PharmacyDbUpdateModel
            {
                Id = pharmacy.Id,
                Address = pharmacyErp!.Address?.Value?.ToUpper().TrimEnd() ?? pharmacy.Address,
                CompanyId = pharmacy.CompanyId,
                Name = pharmacyErp!.LocationName?.BG?.ToUpper().TrimEnd() ?? pharmacy.Name,
                PharmacyChainId = pharmacy.PharmacyChainId,
                PharmnetId = int.TryParse(pharmacyErp!.PharmnetId?.Value?.TrimEnd(), out var pharmnetParsed) ? pharmnetParsed : pharmacy.PharmnetId,
                PhoenixId = int.TryParse(pharmacyErp.PhoenixId?.Value?.TrimEnd(), out var phoenixParsed) ? phoenixParsed : pharmacy.PhoenixId,
                RegionId = pharmacy.RegionId,
                SopharmaId = int.TryParse(pharmacyErp.SopharmaId?.Value?.TrimEnd(), out var sopharmaParsed) ? sopharmaParsed : pharmacy.SopharmaId,
                StingId = int.TryParse(pharmacyErp.StingId?.Value?.TrimEnd(), out var stingParsed) ? stingParsed : pharmacy.StingId,
                ModifiedOn = DateTime.Now
            };
            
            if (pharmacyErp?.ParentParty?.PartyId != null 
                && pharmacyCompaniesDict.TryGetValue(pharmacyErp?.ParentParty.PartyId.TrimEnd(), out var company))
            {
                pharmacyChanged.CompanyId = company.Id;
            }
            
            if (pharmacyErp!.Region?.ValueId != null && regionsCheckDict.TryGetValue(pharmacyErp.Region.ValueId, out var region))
            {
                pharmacyChanged.RegionId = region.Id;
            }
            
            if (pharmacyErp.PharmacyChain?.ValueId != null && pharmacyChainDict.TryGetValue(pharmacyErp.PharmacyChain.ValueId, out var pharmacyChain))
            {
                pharmacyChanged.PharmacyChainId = pharmacyChain.Id;
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
        if (withIds) citiesQuery += ",Id&$expand=Party($select=PartyId)";
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpConstants.ErpRequests.BaseUrl}{citiesQuery}");
        var citiesErp = JsonConvert.DeserializeObject<List<ErpCityCheck>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
        return withIds ? citiesErp.Where(c => c.City is { Value: { } }).ToList() : citiesErp.Where(c => c.City is { Value: { } }).DistinctBy(c => c.City.ValueId).ToList();
    }

    private static async Task<List<ErpPharmacyCheck>> GetPharmaciesErp(bool filtered)
    {
        var pharmaciesErpQuery = filtered ? "General_Contacts_CompanyLocations?$top=60000&$filter=PartyUpdateTime%20ge%202019-01-01T00:00:00.000Z&$select=CustomProperty_ADDRES,CustomProperty_ID_u002DA_u002DKI_u002DFarmnet,CustomProperty_ID_u002DA_u002DKI_u002DPhoenix,CustomProperty_ID_u002DA_u002DKI_u002DSOPHARMA,CustomProperty_ID_u002DA_u002DKI_u002DSting,CustomProperty_RETREG,CustomProperty_%D0%92%D0%95%D0%A0%D0%98%D0%93%D0%90,CustomProperty_%D0%9A%D0%9B%D0%90%D0%A1,Id,IsActive,LocationName,PartyCode,PartyId&$expand=ParentParty($select=PartyId,PartyName)" : "General_Contacts_CompanyLocations?$top=60000&$select=CustomProperty_ADDRES,CustomProperty_ID_u002DA_u002DKI_u002DFarmnet,CustomProperty_ID_u002DA_u002DKI_u002DPhoenix,CustomProperty_ID_u002DA_u002DKI_u002DSOPHARMA,CustomProperty_ID_u002DA_u002DKI_u002DSting,CustomProperty_RETREG,CustomProperty_%D0%92%D0%95%D0%A0%D0%98%D0%93%D0%90,CustomProperty_%D0%9A%D0%9B%D0%90%D0%A1,Id,IsActive,LocationName,PartyCode,PartyId&$expand=ParentParty($select=PartyId,PartyName)";
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpConstants.ErpRequests.BaseUrl}{pharmaciesErpQuery}");
        return JsonConvert.DeserializeObject<List<ErpPharmacyCheck>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
    }

    // private static PharmacyDisplayModel CreatePharmacyDisplayModel(IRow row, int codeRow, int nameRow, int addressRow, int cityRow, int vatRow, int? chainRow)
    // {
    //     var pharmacy = new PharmacyDisplayModel();
    //     var codeCell = row.GetCell(codeRow);
    //     if (codeCell!=null) pharmacy.Code = codeCell.ToString()?.TrimEnd();
    //     
    //     var nameCell = row.GetCell(nameRow);
    //     if (nameCell!=null) pharmacy.Name = nameCell.ToString()?.TrimEnd();
    //
    //     var addressCell = row.GetCell(addressRow);
    //     if (addressCell!=null) pharmacy.Address = addressCell.ToString()?.TrimEnd();
    //
    //     var cityCell = row.GetCell(cityRow);
    //     if (cityCell!=null) pharmacy.City = cityCell.ToString()?.TrimEnd();
    //
    //     var vatCell = row.GetCell(vatRow);
    //     if (vatCell!=null) pharmacy.Vat = vatCell.ToString()?.TrimEnd();
    //
    //     if (chainRow == null) return pharmacy;
    //     var chainCell = row.GetCell((int)chainRow);
    //     if (chainCell!=null) pharmacy.PharmacyChain = chainCell.ToString()?.TrimEnd();
    //     
    //     return pharmacy;
    // }
    
    private static PharmacyDisplayModel CreatePharmacyDisplayModel(IRow row, int codeRow, int nameRow, int addressRow, int cityRow, int vatRow, int? chainRow)
    {
        var pharmacy = new PharmacyDisplayModel
        {
            Code = GetCellValue(row, codeRow)?.TrimEnd(),
            Name = GetCellValue(row, nameRow)?.TrimEnd(),
            Address = GetCellValue(row, addressRow)?.TrimEnd(),
            City = GetCellValue(row, cityRow)?.TrimEnd(),
            Vat = GetCellValue(row, vatRow)?.TrimEnd(),
            PharmacyChain = GetCellValue(row, chainRow)?.TrimEnd()
        };
        return pharmacy;
    }

    private static string? GetCellValue(IRow row, int? index)
    {
        return index != null ? row.GetCell((int)index)?.ToString() : null;
    }
}
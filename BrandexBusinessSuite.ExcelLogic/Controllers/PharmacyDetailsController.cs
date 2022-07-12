﻿namespace BrandexBusinessSuite.ExcelLogic.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;
    
using Data.Enums;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models;

using Models.Pharmacies;
using Models.Cities;
using Models.PharmacyChains;
using Models.PharmacyCompanies;
using Models.Regions;

using Services.Cities;
using Services.Pharmacies;
using Services.PharmacyChains;
using Services.Regions;
using Services.PharmacyCompanies;

using static Common.ExcelDataConstants.ExcelLineErrors;

public class PharmacyDetailsController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IPharmaciesService _pharmaciesService;
    private readonly IPharmacyCompaniesService _pharmacyCompaniesService;
    private readonly IRegionsService _regionsService;
    private readonly IPharmacyChainsService _pharmacyChainsService;
    private readonly ICitiesService _citiesService;

    private const int BrandexIdColumn = 19;
    private const int NameColumn = 4;
    private const int PharmacyClassColumn = 2;
    private const int ActiveColumn = 1;
    private const int PharmacyCompanyColumn = 3;
    private const int PharmacyChainColumn = 5;
    private const int AddressColumn = 6;
    private const int CityColumn = 20;
    private const int PharmnetIdColumn = 14;
    private const int PhoenixIdColumn = 15;
    private const int SopharmaIdColumn = 16;
    private const int StingIdColumn = 17;
    private const int RegionColumn = 8;


    public PharmacyDetailsController(
        IWebHostEnvironment hostEnvironment,
        IPharmaciesService pharmaciesService,
        IPharmacyCompaniesService pharmacyCompaniesService,
        IPharmacyChainsService pharmacyChainsService,
        IRegionsService regionsService,
        ICitiesService citiesService)

    {
        _hostEnvironment = hostEnvironment;
        _pharmaciesService = pharmaciesService;
        _pharmacyCompaniesService = pharmacyCompaniesService;
        _pharmacyChainsService = pharmacyChainsService;
        _regionsService = regionsService;
        _citiesService = citiesService;

    }
    
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm]IFormFile file)
    {

        var folderName = "UploadExcel";

        var webRootPath = _hostEnvironment.WebRootPath;

        var newPath = Path.Combine(webRootPath, folderName);

        var errorDictionary = new Dictionary<int, string>();
            
        var validPharmacyList = new List<PharmacyDbInputModel>();
            
        var pharmaciesEdited = new List<PharmacyDbInputModel>();
            
        var citiesIdsForCheck = await _citiesService.GetCitiesCheck();
        var pharmacyCompaniesIdsForCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesCheck();
        var pharmacyChainsIdsForCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        var regionIdsForCheck = await _regionsService.AllRegions();
            
        var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();

        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }

        if (file.Length > 0)
        {

            var sFileExtension = Path.GetExtension(file.FileName)!.ToLower();
            var fullPath = Path.Combine(newPath, file.FileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            stream.Position = 0;

            ISheet sheet;
            if (sFileExtension == ".xls")
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                sheet = hssfwb.GetSheetAt(0);
            }

            else
            {
                var hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                sheet = hssfwb.GetSheetAt(0);
            }

            for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
            {

                var row = sheet.GetRow(i);
                if (row == null) continue;

                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                var newPharmacy = new PharmacyDbInputModel
                {
                    PharmacyClass = PharmacyClass.Other,
                    Active = true,
                    Name = row.GetCell(NameColumn).ToString()?.TrimEnd(),
                    Address = row.GetCell(AddressColumn).ToString()?.TrimEnd(),
                }; 
                    
                CreatePharmacyInputModel(
                    citiesIdsForCheck,
                    pharmacyCompaniesIdsForCheck,
                    pharmacyChainsIdsForCheck,
                    regionIdsForCheck,
                    newPharmacy,
                    row,
                    i,
                    errorDictionary);


                if (newPharmacy.BrandexId == 0 || newPharmacy.Name == null || newPharmacy.PharmacyChainId == 0 ||
                    newPharmacy.RegionId == 0 || newPharmacy.CityId == 0 || newPharmacy.CompanyId == 0) continue;
                    
                if (pharmacyIdsForCheck.Any(p=> p.BrandexId == newPharmacy.BrandexId))
                {
                    pharmaciesEdited.Add(newPharmacy);
                }
                else
                {
                    validPharmacyList.Add(newPharmacy);
                }

            }
            
        }

        if (errorDictionary.Count==0)
        {
            await _pharmaciesService.UploadBulk(validPharmacyList);
            await _pharmaciesService.Update(pharmaciesEdited);
        }

        var pharmacyErrorModel = new CustomErrorDictionaryOutputModel
        {
            Errors = errorDictionary
        };
            
        var outputSerialized = JsonConvert.SerializeObject(pharmacyErrorModel);

        return outputSerialized;
            
    }
    
    [HttpPost]
    public async Task<string> Upload([FromBody]PharmacyInputModel pharmacyInputModel)
    {

        if(pharmacyInputModel.BrandexId!=0
           && pharmacyInputModel.Name!=null
           && await _pharmacyCompaniesService.CheckCompanyByName(pharmacyInputModel.CompanyName)
           && await _pharmacyChainsService.CheckPharmacyChainByName(pharmacyInputModel.PharmacyChainName)
           && await _citiesService.CheckCityName(pharmacyInputModel.CityName)
           && await _regionsService.CheckRegionByName(pharmacyInputModel.RegionName)
           && pharmacyInputModel.Address != null)
        {
            var pharmacyDbInputModel = new PharmacyDbInputModel
            {
                BrandexId = pharmacyInputModel.BrandexId,
                Name = pharmacyInputModel.Name,
                PharmacyClass = pharmacyInputModel.PharmacyClass,
                Active = pharmacyInputModel.Active,
                CompanyId = await _pharmacyCompaniesService.IdByName(pharmacyInputModel.CompanyName),
                PharmacyChainId = await _pharmacyChainsService.IdByName(pharmacyInputModel.PharmacyChainName),
                Address = pharmacyInputModel.Address,
                CityId = await _citiesService.IdByName(pharmacyInputModel.CityName),
                PharmnetId = pharmacyInputModel.PharmnetId,
                PhoenixId = pharmacyInputModel.PhoenixId,
                SopharmaId = pharmacyInputModel.SopharmaId,
                StingId = pharmacyInputModel.StingId,
                RegionId = await _regionsService.IdByName(pharmacyInputModel.RegionName)

            };

            if(await _pharmaciesService.CreatePharmacy(pharmacyDbInputModel) != "")
            {
                var pharmacyOutputModel = new PharmacyOutputModel
                {

                    Name = pharmacyInputModel.Name,
                    PharmacyClass = pharmacyInputModel.PharmacyClass.ToString(),
                    CompanyName = pharmacyInputModel.CompanyName,
                    PharmacyChainName = pharmacyInputModel.PharmacyChainName,
                    Address = pharmacyInputModel.Address,
                    CityName =pharmacyInputModel.CityName,
                    Region = pharmacyInputModel.RegionName,
                    BrandexId = pharmacyInputModel.BrandexId,
                    PharmnetId = pharmacyInputModel.PharmnetId,
                    PhoenixId = pharmacyInputModel.PhoenixId,
                    SopharmaId = pharmacyInputModel.SopharmaId,
                    StingId = pharmacyInputModel.StingId,
                };

                var outputSerialized = JsonConvert.SerializeObject(pharmacyOutputModel);

                return outputSerialized;

            }
                
        }

        throw new InvalidOperationException();
    }

    private void CreatePharmacyInputModel(
        IEnumerable<CityCheckModel> citiesIdsForCheck,
        IEnumerable<PharmacyCompanyCheckModel> pharmacyCompanyIdsForCheck,
        IEnumerable<PharmacyChainCheckModel> pharmacyChainsIdsForCheck,
        IEnumerable<RegionOutputModel> regionIdsForCheck,
        PharmacyDbInputModel newPharmacy,
        IRow row,
        int i,
        IDictionary<int, string> errorDictionary
    )
    {
        var brandexId = row.GetCell(BrandexIdColumn);

        if (brandexId!=null && int.TryParse(brandexId.ToString()!.TrimEnd(), out var brandexIdConverted))
        {
            newPharmacy.BrandexId = brandexIdConverted;
        }
        
        else
        {
            errorDictionary[i + 1] = IncorrectPharmacyId;
        }

        var pharmacyClass = row.GetCell(PharmacyClassColumn);

        if (pharmacyClass!=null && !string.IsNullOrWhiteSpace(pharmacyClass.ToString()!.TrimEnd()))
        {
            newPharmacy.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacyClass.ToString()!.TrimEnd(), true);
        }

        var pharmacyActive = row.GetCell(ActiveColumn);

        if (pharmacyActive != null && pharmacyActive.ToString()?.TrimEnd()[0] == '0')
        {
            newPharmacy.Active = false;
        }

        var companyIdRow = row.GetCell(PharmacyCompanyColumn);

        if (companyIdRow!=null && 
            pharmacyCompanyIdsForCheck.Any(p => companyIdRow != null && p.Name == companyIdRow.ToString().TrimEnd().ToUpper()))
        {
            var companyId = pharmacyCompanyIdsForCheck
                .Where(p => companyIdRow != null && p.Name == companyIdRow.ToString().TrimEnd().ToUpper())
                .Select(p => p.Id)
                .FirstOrDefault();

            newPharmacy.CompanyId = companyId;
            
        }
            
        else
        {
            errorDictionary[i + 1] = IncorrectPharmacyCompanyId;
        }
            

        var chainIdRow = row.GetCell(PharmacyChainColumn);
        
        if (chainIdRow!=null && 
            pharmacyChainsIdsForCheck.Any(p => p.Name == chainIdRow.ToString()?.TrimEnd().ToUpper())
           )
        {
            var chainId = pharmacyChainsIdsForCheck
                .Where(p => p.Name == chainIdRow.ToString()?.TrimEnd().ToUpper())
                .Select(p => p.Id)
                .FirstOrDefault();
            
            newPharmacy.PharmacyChainId = chainId;

        }
        else
        {
            errorDictionary[i + 1] = IncorrectPharmacyChainId;
        }
        
        var regionIdRow = row.GetCell(RegionColumn);
        
        if (regionIdRow!=null && 
            regionIdsForCheck.Any(r => r.Name==regionIdRow.ToString()!.TrimEnd()))
        {
            var regionId = regionIdsForCheck
                .Where(r => r.Name==regionIdRow.ToString()!.TrimEnd())
                .Select(r => r.Id)
                .FirstOrDefault();

            newPharmacy.RegionId = regionId;
        }
            
        else
        {
            errorDictionary[i + 1] = IncorrectRegion;
        }
            

        var pharmnetIdRow = row.GetCell(PharmnetIdColumn);
        
        if (pharmnetIdRow != null && int.TryParse(pharmnetIdRow.ToString()?.TrimEnd(), out var pharmnetId))
        {
            newPharmacy.PharmnetId = pharmnetId;
        }

        var phoenixIdRow = row.GetCell(PhoenixIdColumn);
        
        if (phoenixIdRow != null && int.TryParse(phoenixIdRow.ToString()?.TrimEnd(), out var phoenixId))
        {
            newPharmacy.PhoenixId = phoenixId;
        }

        var sopharmaIdRow = row.GetCell(SopharmaIdColumn);
        
        if (sopharmaIdRow != null && int.TryParse(sopharmaIdRow.ToString()?.TrimEnd(), out var sopharmaId))
        {
            newPharmacy.SopharmaId = sopharmaId;
        }

        var stingIdRow = row.GetCell(StingIdColumn);
        
        if (stingIdRow != null && int.TryParse(stingIdRow.ToString()?.TrimEnd(), out var stingId))
        {
            newPharmacy.StingId = stingId;
        }

        var cityIdRow = row.GetCell(CityColumn);
        
        if (cityIdRow!=null && citiesIdsForCheck.Any(c=>c.Name==cityIdRow.ToString()!
                .TrimEnd().ToUpper()))
        {

            var cityId = citiesIdsForCheck.Where(c=>c.Name==cityIdRow.ToString()!
                    .TrimEnd().ToUpper())
                .Select(c=>c.Id)
                .FirstOrDefault();
            
            newPharmacy.CityId = cityId;
        }

        else
        {
            errorDictionary[i + 1] = IncorrectCityName;
        }
    }
}
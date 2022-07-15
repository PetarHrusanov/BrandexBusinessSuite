namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using Data.Enums;
using Infrastructure;

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

using static Methods.ExcelMethods;

using static Common.ExcelDataConstants.ExcelLineErrors;

public class PharmacyDetailsController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    
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

    public PharmacyDetailsController(IWebHostEnvironment hostEnvironment, IPharmaciesService pharmaciesService,
        IPharmacyCompaniesService pharmacyCompaniesService, IPharmacyChainsService pharmacyChainsService,
        IRegionsService regionsService, ICitiesService citiesService)

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
    public async Task<string> Import([FromForm] IFormFile file)
    {
        var errorDictionary = new List<string>();

        var validPharmacyList = new List<PharmacyDbInputModel>();
        var pharmaciesEdited = new List<PharmacyDbInputModel>();

        var citiesIdsForCheck = await _citiesService.GetCitiesCheck();
        var pharmacyCompaniesIdsForCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesCheck();
        var pharmacyChainsIdsForCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        var regionIdsForCheck = await _regionsService.AllRegions();

        var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();

        if (!CheckXlsx(file, errorDictionary)) return JsonConvert.SerializeObject(errorDictionary.ToArray());

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);
        
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);
        
        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            if (row.GetCell(NameColumn) == null || row.GetCell(AddressColumn) == null)
            {
                errorDictionary.Add($"{i} Line: Null or blank Name or Address");
                continue;
            }

            var newPharmacy = new PharmacyDbInputModel
            {
                PharmacyClass = PharmacyClass.Other,
                Active = true,
                Name = row.GetCell(NameColumn).ToString()?.TrimEnd(),
                Address = row.GetCell(AddressColumn).ToString()?.TrimEnd()
            };

            CreatePharmacyInputModel(citiesIdsForCheck, pharmacyCompaniesIdsForCheck, pharmacyChainsIdsForCheck,
                regionIdsForCheck, newPharmacy, row, i, errorDictionary);

            if (newPharmacy.BrandexId == 0 || newPharmacy.Name == null || newPharmacy.PharmacyChainId == 0 ||
                newPharmacy.RegionId == 0 || newPharmacy.CityId == 0 || newPharmacy.CompanyId == 0)
                continue;

            if (pharmacyIdsForCheck.Any(p => p.BrandexId == newPharmacy.BrandexId))
            {
                pharmaciesEdited.Add(newPharmacy);
                continue;
            }

            validPharmacyList.Add(newPharmacy);
        }

        if (errorDictionary.Count != 0) return JsonConvert.SerializeObject(errorDictionary);
        
        await _pharmaciesService.UploadBulk(validPharmacyList);
        await _pharmaciesService.Update(pharmaciesEdited);

        return JsonConvert.SerializeObject(errorDictionary);
    }

    private static void CreatePharmacyInputModel(IEnumerable<CityCheckModel> citiesIdsForCheck,
        IEnumerable<PharmacyCompanyCheckModel> pharmacyCompanyIdsForCheck,
        IEnumerable<PharmacyChainCheckModel> pharmacyChainsIdsForCheck,
        IEnumerable<RegionOutputModel> regionIdsForCheck, PharmacyDbInputModel newPharmacy, IRow row, int i,
        ICollection<string> errorDictionary)
    {
        var brandexId = row.GetCell(BrandexIdColumn);
        var pharmacyClass = row.GetCell(PharmacyClassColumn);
        var pharmacyActive = row.GetCell(ActiveColumn);
        var companyIdRow = row.GetCell(PharmacyCompanyColumn);
        var chainIdRow = row.GetCell(PharmacyChainColumn);
        var regionIdRow = row.GetCell(RegionColumn);

        var cityIdRow = row.GetCell(CityColumn);

        if (brandexId == null || companyIdRow == null || chainIdRow == null || regionIdRow == null || cityIdRow == null)
        {
            errorDictionary.Add($"{i} Line: Null or blank value at a necessary field ");
            return;
        }

        if (int.TryParse(brandexId.ToString()!.TrimEnd(), out var brandexIdConverted))
        {
            newPharmacy.BrandexId = brandexIdConverted;
        }

        else
        {
            errorDictionary.Add($"{i} Line: {IncorrectPharmacyId}");
        }

        if (pharmacyClass != null && !string.IsNullOrWhiteSpace(pharmacyClass.ToString()!.TrimEnd()))
        {
            newPharmacy.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacyClass.ToString()!.TrimEnd(), true);
        }

        if (pharmacyActive != null && pharmacyActive.ToString()?.TrimEnd()[0] == '0') newPharmacy.Active = false;

        newPharmacy.CompanyId = pharmacyCompanyIdsForCheck
            .Where(p => p.Name == companyIdRow.ToString()!.TrimEnd().ToUpper())
            .Select(p => p.Id)
            .FirstOrDefault();

        if (newPharmacy.CompanyId == 0) errorDictionary.Add($"{i} Line: {IncorrectPharmacyCompanyId}");

        newPharmacy.PharmacyChainId = pharmacyChainsIdsForCheck
            .Where(p => p.Name == chainIdRow.ToString()?.TrimEnd().ToUpper())
            .Select(p => p.Id)
            .FirstOrDefault();

        if (newPharmacy.PharmacyChainId == 0) errorDictionary.Add($"{i} Line: {IncorrectPharmacyChainId}");

        newPharmacy.RegionId = regionIdsForCheck.Where(r => r.Name == regionIdRow.ToString()!.TrimEnd())
            .Select(r => r.Id)
            .FirstOrDefault();

        if (newPharmacy.RegionId == 0) errorDictionary.Add($"{i} Line: {IncorrectRegion}");

        newPharmacy.CityId = citiesIdsForCheck.Where(c => c.Name == cityIdRow.ToString()!.TrimEnd().ToUpper())
            .Select(c => c.Id)
            .FirstOrDefault();

        if (newPharmacy.CityId == 0) errorDictionary.Add($"{i} Line: {IncorrectCityName}");

        if (ConvertRowToInt(row, PharmnetIdColumn) != 0) newPharmacy.PharmnetId = ConvertRowToInt(row, PharmnetIdColumn);
        if (ConvertRowToInt(row, PhoenixIdColumn) != 0) newPharmacy.PhoenixId = ConvertRowToInt(row, PhoenixIdColumn);
        if (ConvertRowToInt(row, SopharmaIdColumn) != 0) newPharmacy.SopharmaId = ConvertRowToInt(row, SopharmaIdColumn);
        if (ConvertRowToInt(row, StingIdColumn) != 0) newPharmacy.StingId = ConvertRowToInt(row, StingIdColumn);
    }
}
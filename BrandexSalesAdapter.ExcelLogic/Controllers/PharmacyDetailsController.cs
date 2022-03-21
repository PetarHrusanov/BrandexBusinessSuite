using Newtonsoft.Json;

namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
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
    using BrandexSalesAdapter.ExcelLogic.Data.Enums;
    using BrandexSalesAdapter.ExcelLogic.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services;
    using BrandexSalesAdapter.ExcelLogic.Services.Cities;
    using BrandexSalesAdapter.ExcelLogic.Services.Companies;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.PharmacyChains;
    using BrandexSalesAdapter.ExcelLogic.Services.Regions;
    using Microsoft.AspNetCore.Authorization;
    
    using static Common.DataConstants.ExcelLineErrors;

    public class PharmacyDetailsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // db Services
        private readonly IPharmaciesService _pharmaciesService;
        private readonly ICompaniesService _companiesService;
        private readonly IRegionsService _regionsService;
        private readonly IPharmacyChainsService _pharmacyChainsService;
        private readonly ICitiesService _citiesService; 

        // universal Services
        private readonly INumbersChecker _numbersChecker;

        public PharmacyDetailsController(
            IWebHostEnvironment hostEnvironment,
            INumbersChecker numbersChecker,
            IPharmaciesService pharmaciesService,
            ICompaniesService companiesService,
            IPharmacyChainsService pharmacyChainsService,
            IRegionsService regionsService,
            ICitiesService citiesService)

        {

            this._hostEnvironment = hostEnvironment;
            this._numbersChecker = numbersChecker;
            this._pharmaciesService = pharmaciesService;
            this._companiesService = companiesService;
            this._pharmacyChainsService = pharmacyChainsService;
            this._regionsService = regionsService;
            this._citiesService = citiesService;

        }

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm]IFormFile file)
        {

            // IFormFile file = Request.Form.Files[0];

            string folderName = "UploadExcel";

            string webRootPath = _hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            if (!Directory.Exists(newPath))

            {

                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)

            {

                string sFileExtension = Path.GetExtension(file.FileName)!.ToLower();

                string fullPath = Path.Combine(newPath, file.FileName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))

                {

                    await file.CopyToAsync(stream);

                    stream.Position = 0;

                    ISheet sheet;
                    if (sFileExtension == ".xls")

                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  

                    }

                    else

                    {

                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row

                    int cellCount = headerRow.LastCellNum;

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newPharmacy = new PharmacyInputModel
                        {
                            PharmacyClass = PharmacyClass.Other,
                            Active = true,
                            Name = row.GetCell(5).ToString()?.TrimEnd(),
                            Address = row.GetCell(7).ToString()?.TrimEnd(),
                        };

                        var brandexId = row.GetCell(0).ToString()?.TrimEnd();
                        
                        if (_numbersChecker.WholeNumberCheck(brandexId))
                        {
                            newPharmacy.BrandexId = int.Parse(brandexId);
                        }
                        else
                        {
                            errorDictionary[i+1] = IncorrectPharmacyId;
                        }
                        
                        var pharmacyClass = row.GetCell(2).ToString()?.TrimEnd();
                        
                        if (pharmacyClass != "")
                            newPharmacy.PharmacyClass =
                                (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacyClass, true);
                        
                        var pharmacyActive = row.GetCell(3).ToString()?.TrimEnd();

                        if (pharmacyActive != null && pharmacyActive[0] == '0')
                        {
                            newPharmacy.Active = false;
                        }
                        
                        var companyIdRow = row.GetCell(4).ToString()?.TrimEnd();
                        int companyId = await this._companiesService.IdByName(companyIdRow);

                        if (companyId!=0)
                        {
                            newPharmacy.CompanyId = companyId;
                            
                        }
                        else
                        {
                            errorDictionary[i+1] = "COMPANY ID";
                        }
                        
                        var chainIdRow = row.GetCell(6).ToString()?.TrimEnd();
                        
                        var chainId = await this._pharmacyChainsService.IdByName(chainIdRow);
                        if (chainId!=0)
                        {
                            newPharmacy.PharmacyChainId = chainId;
                        }
                        else
                        {
                            errorDictionary[i+1] = "PHARMACY CHAIN ID";
                        }
                        
                        var regionIdRow = row.GetCell(9).ToString()?.TrimEnd();
                        
                        int regionId = await this._regionsService.IdByName(regionIdRow);

                        if (regionId!=0)
                        {
                            newPharmacy.RegionId = regionId;
                        }
                        else
                        {
                            errorDictionary[i+1] = "REGION ID";
                        }
                        
                        var pharmnetIdRow = row.GetCell(15).ToString()?.TrimEnd();

                        if(pharmnetIdRow != null && int.TryParse(pharmnetIdRow, out var pharmnetId))
                        {
                            newPharmacy.PharmnetId = pharmnetId;
                        }
                        
                        var phoenixIdRow = row.GetCell(16).ToString()?.TrimEnd();

                        if(phoenixIdRow != null && int.TryParse(phoenixIdRow, out var phoenixId))
                        {
                            newPharmacy.PhoenixId = phoenixId;
                        }
                        
                        var sopharmaIdRow = row.GetCell(17).ToString()?.TrimEnd();

                        if(sopharmaIdRow != null && int.TryParse(sopharmaIdRow, out var sopharmaId))
                        {
                            newPharmacy.SopharmaId = sopharmaId;
                        }
                        
                        var stingIdRow = row.GetCell(18).ToString()?.TrimEnd();

                        if(stingIdRow != null && int.TryParse(stingIdRow, out var stingId))
                        {
                            newPharmacy.StingId = stingId;
                        }
                        
                        var cityIdRow = row.GetCell(21).ToString()?.TrimEnd();
                        var cityId = await this._citiesService.IdByName(cityIdRow);

                        if (cityId != 0)
                        {
                            newPharmacy.CityId = cityId;
                        }
                            
                        else
                        {
                            errorDictionary[i+1] = "Wrong City ID";
                        }

                        await this._pharmaciesService.CreatePharmacy(newPharmacy);

                    }   
                }
            }

            var pharmacyErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };
            
            string outputSerialized = JsonConvert.SerializeObject(pharmacyErrorModel);

            return outputSerialized;

            // return this.View(pharmacyErrorModel);
        }

        // [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(int brandexId,
            string name,
            PharmacyClass pharmacyClass,
            bool active,
            string companyName,
            string pharmacyChainName,
            string address,
            string cityName,
            int? pharmnetId,
            int? phoenixId,
            int? sopharmaId,
            int? stingId,
            string regionName
            )
        {
            if(brandexId!=0
                && name!=null
                && await this._companiesService.CheckCompanyByName(companyName)
                && await this._pharmacyChainsService.CheckPharmacyChainByName(pharmacyChainName)
                && await this._citiesService.CheckCityName(cityName)
                && await this._regionsService.CheckRegionByName(regionName)
                && address != null)
            {
                var pharmacyInputModel = new PharmacyInputModel
                {
                    BrandexId = brandexId,
                    Name = name,
                    PharmacyClass = pharmacyClass,
                    Active = active,
                    CompanyId = await this._companiesService.IdByName(companyName),
                    PharmacyChainId = await this._pharmacyChainsService.IdByName(pharmacyChainName),
                    Address = address,
                    CityId = await this._citiesService.IdByName(cityName),
                    PharmnetId = pharmnetId,
                    PhoenixId = phoenixId,
                    SopharmaId = sopharmaId,
                    StingId = stingId,
                    RegionId = await this._regionsService.IdByName(regionName)

                };

                if(await this._pharmaciesService.CreatePharmacy(pharmacyInputModel) != "")
                {
                    var pharmacyOutputModel = new PharmacyOutputModel
                    {

                        Name = name,
                        PharmacyClass = pharmacyClass.ToString(),
                        CompanyName = companyName,
                        PharmacyChainName = pharmacyChainName,
                        Address = address,
                        CityName = cityName,
                        Region = regionName,
                        BrandexId = brandexId,
                        PharmnetId = pharmnetId,
                        PhoenixId = phoenixId,
                        SopharmaId = sopharmaId,
                        StingId = stingId
                    };

                    return this.View(pharmacyOutputModel);

                }

                else
                {
                    return Redirect("Index");
                }
            }

            return Redirect("Index");

        }
    }
}

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
    
    using Data.Enums;
    
    using Models;
    using Models.Pharmacies;
    
    using Services;
    using Services.Cities;
    using Services.Pharmacies;
    using Services.PharmacyChains;
    using Services.Regions;
    using Services.PharmacyCompanies;
    
    using Newtonsoft.Json;

    using static Common.DataConstants.ExcelLineErrors;

    public class PharmacyDetailsController : Controller
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
        private const int PharmacyClassColumn = 1;
        private const int ActiveColumn = 2;
        private const int PharmacyCompanyColumn = 3;
        private const int PharmacyChainColumn = 5;
        private const int AddressColumn = 6;
        private const int CityColumn = 20;
        private const int PharmnetIdColumn = 14;
        private const int PhoenixIdColumn = 15;
        private const int SopharmaIdColumn = 16;
        private const int StingIdColumn = 17;
        private const int RegionColumn = 8;

        // universal Services
        private readonly INumbersChecker _numbersChecker;

        public PharmacyDetailsController(
            IWebHostEnvironment hostEnvironment,
            INumbersChecker numbersChecker,
            IPharmaciesService pharmaciesService,
            IPharmacyCompaniesService pharmacyCompaniesService,
            IPharmacyChainsService pharmacyChainsService,
            IRegionsService regionsService,
            ICitiesService citiesService)

        {
            _hostEnvironment = hostEnvironment;
            _numbersChecker = numbersChecker;
            _pharmaciesService = pharmaciesService;
            _pharmacyCompaniesService = pharmacyCompaniesService;
            _pharmacyChainsService = pharmacyChainsService;
            _regionsService = regionsService;
            _citiesService = citiesService;

        }

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }
        
                // [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<string> Check([FromForm]IFormFile file)
        {

            // IFormFile file = Request.Form.Files[0];

            var folderName = "UploadExcel";

            var webRootPath = _hostEnvironment.WebRootPath;

            var newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            if (!Directory.Exists(newPath))

            {

                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)

            {

                var sFileExtension = Path.GetExtension(file.FileName)!.ToLower();

                if (file.FileName != null)
                {
                    var fullPath = Path.Combine(newPath, file.FileName);

                    await using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    stream.Position = 0;

                    ISheet sheet;
                    if (sFileExtension == ".xls")

                    {

                        var hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  

                    }

                    else

                    {

                        var hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

                    }

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

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
                            PharmacyChainId = await this._pharmacyChainsService.IdByName(string.Empty)
                        };

                        var brandexId = row.GetCell(BrandexIdColumn).ToString()?.TrimEnd();
                        
                        if (_numbersChecker.WholeNumberCheck(brandexId))
                        {
                            newPharmacy.BrandexId = int.Parse(brandexId);
                        }
                        else
                        {
                            errorDictionary[i+1] = IncorrectPharmacyId;
                        }
                        
                        var pharmacyClass = row.GetCell(PharmacyClassColumn).ToString()?.TrimEnd();
                        
                        if (pharmacyClass != "")
                            newPharmacy.PharmacyClass =
                                (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacyClass, true);
                        
                        var pharmacyActive = row.GetCell(ActiveColumn).ToString()?.TrimEnd();

                        if (pharmacyActive != null && pharmacyActive[0] == '0')
                        {
                            newPharmacy.Active = false;
                        }
                        
                        var companyIdRow = row.GetCell(PharmacyCompanyColumn).ToString()?.TrimEnd();
                        var companyId = await _pharmacyCompaniesService.IdByName(companyIdRow);

                        if (companyId!=0)
                        {
                            newPharmacy.CompanyId = companyId;
                            
                        }
                        else
                        {
                            errorDictionary[i+1] = IncorrectPharmacyCompanyId;
                        }
                        
                        var chainIdRow = row.GetCell(PharmacyChainColumn).ToString()?.TrimEnd();
                        var chainId = await _pharmacyChainsService.IdByName(chainIdRow);
                        if (chainId!=0)
                        {
                            newPharmacy.PharmacyChainId = chainId;
                        }
                        else
                        {
                            errorDictionary[i+1] = IncorrectPharmacyChainId;
                        }
                        
                        var regionIdRow = row.GetCell(RegionColumn).ToString()?.TrimEnd();
                        
                        int regionId = await _regionsService.IdByName(regionIdRow);

                        if (regionId!=0)
                        {
                            newPharmacy.RegionId = regionId;
                        }
                        else
                        {
                            errorDictionary[i+1] = IncorrectRegionId;
                        }
                        
                        var pharmnetIdRow = row.GetCell(PharmnetIdColumn).ToString()?.TrimEnd();

                        if(pharmnetIdRow != null && int.TryParse(pharmnetIdRow, out var pharmnetId))
                        {
                            newPharmacy.PharmnetId = pharmnetId;
                        }
                        
                        var phoenixIdRow = row.GetCell(PhoenixIdColumn).ToString()?.TrimEnd();

                        if(phoenixIdRow != null && int.TryParse(phoenixIdRow, out var phoenixId))
                        {
                            newPharmacy.PhoenixId = phoenixId;
                        }
                        
                        var sopharmaIdRow = row.GetCell(SopharmaIdColumn).ToString()?.TrimEnd();

                        if(sopharmaIdRow != null && int.TryParse(sopharmaIdRow, out var sopharmaId))
                        {
                            newPharmacy.SopharmaId = sopharmaId;
                        }
                        
                        var stingIdRow = row.GetCell(StingIdColumn).ToString()?.TrimEnd();

                        if(stingIdRow != null && int.TryParse(stingIdRow, out var stingId))
                        {
                            newPharmacy.StingId = stingId;
                        }
                        
                        var cityIdRow = row.GetCell(CityColumn).ToString()?.TrimEnd();
                        var cityId = await _citiesService.IdByName(cityIdRow.ToUpper());

                        if (cityId != 0)
                        {
                            newPharmacy.CityId = cityId;
                        }
                            
                        else
                        {
                            errorDictionary[i+1] = IncorrectCityId;
                        }
                        
                    }
                }
            }

            var pharmacyErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };
            
            string outputSerialized = JsonConvert.SerializeObject(pharmacyErrorModel);

            return outputSerialized;
            
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

                await using var stream = new FileStream(fullPath, FileMode.Create);
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

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

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

                    var brandexId = row.GetCell(BrandexIdColumn).ToString()?.TrimEnd();
                        
                    if (_numbersChecker.WholeNumberCheck(brandexId))
                    {
                        newPharmacy.BrandexId = int.Parse(brandexId);
                    }
                    else
                    {
                        errorDictionary[i+1] = IncorrectPharmacyId;
                    }
                        
                    var pharmacyClass = row.GetCell(PharmacyClassColumn).ToString()?.TrimEnd();
                        
                    if (pharmacyClass != "")
                        newPharmacy.PharmacyClass =
                            (PharmacyClass)Enum.Parse(typeof(PharmacyClass), pharmacyClass, true);
                        
                    var pharmacyActive = row.GetCell(ActiveColumn).ToString()?.TrimEnd();

                    if (pharmacyActive != null && pharmacyActive[0] == '0')
                    {
                        newPharmacy.Active = false;
                    }
                        
                    var companyIdRow = row.GetCell(PharmacyCompanyColumn).ToString()?.TrimEnd();
                    var companyId = await _pharmacyCompaniesService.IdByName(companyIdRow);

                    if (companyId!=0)
                    {
                        newPharmacy.CompanyId = companyId;
                            
                    }
                    else
                    {
                        errorDictionary[i+1] = "COMPANY ID";
                    }
                        
                    var chainIdRow = row.GetCell(PharmacyChainColumn).ToString()?.TrimEnd();
                    var chainId = await _pharmacyChainsService.IdByName(chainIdRow);
                    
                    if (chainId!=0)
                    {
                        newPharmacy.PharmacyChainId = chainId;
                    }
                    else
                    {
                        errorDictionary[i+1] = "PHARMACY CHAIN ID";
                    }
                        
                    var regionIdRow = row.GetCell(RegionColumn).ToString()?.TrimEnd();
                    int regionId = await _regionsService.IdByName(regionIdRow);
                    
                    if (regionId!=0)
                    {
                        newPharmacy.RegionId = regionId;
                    }
                    else
                    {
                        errorDictionary[i+1] = "REGION ID";
                    }
                        
                    var pharmnetIdRow = row.GetCell(PharmnetIdColumn).ToString()?.TrimEnd();
                    
                    if(pharmnetIdRow != null && int.TryParse(pharmnetIdRow, out var pharmnetId))
                    {
                        newPharmacy.PharmnetId = pharmnetId;
                    }
                        
                    var phoenixIdRow = row.GetCell(PhoenixIdColumn).ToString()?.TrimEnd();

                    if(phoenixIdRow != null && int.TryParse(phoenixIdRow, out var phoenixId))
                    {
                        newPharmacy.PhoenixId = phoenixId;
                    }
                        
                    var sopharmaIdRow = row.GetCell(SopharmaIdColumn).ToString()?.TrimEnd();

                    if(sopharmaIdRow != null && int.TryParse(sopharmaIdRow, out var sopharmaId))
                    {
                        newPharmacy.SopharmaId = sopharmaId;
                    }
                        
                    var stingIdRow = row.GetCell(StingIdColumn).ToString()?.TrimEnd();

                    if(stingIdRow != null && int.TryParse(stingIdRow, out var stingId))
                    {
                        newPharmacy.StingId = stingId;
                    }
                        
                    var cityIdRow = row.GetCell(CityColumn).ToString()?.TrimEnd();
                    var cityId = await _citiesService.IdByName(cityIdRow.ToUpper());

                    if (cityId != 0)
                    {
                        newPharmacy.CityId = cityId;
                    }
                            
                    else
                    {
                        errorDictionary[i+1] = "Wrong City ID";
                    }

                    await _pharmaciesService.CreatePharmacy(newPharmacy);

                }
            }

            var pharmacyErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };
            
            string outputSerialized = JsonConvert.SerializeObject(pharmacyErrorModel);

            return outputSerialized;
            
        }

        // [Authorize]
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
    }
}

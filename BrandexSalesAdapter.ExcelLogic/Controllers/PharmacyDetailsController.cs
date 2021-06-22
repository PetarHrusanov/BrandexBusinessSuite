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

    public class PharmacyDetailsController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly IPharmaciesService pharmaciesService;
        private readonly ICompaniesService companiesService;
        private readonly IRegionsService regionsService;
        private readonly IPharmacyChainsService pharmacyChainsService;
        private readonly ICitiesService citiesService; 

        // universal Services
        private readonly INumbersChecker numbersChecker;

        public PharmacyDetailsController(
            IWebHostEnvironment hostEnvironment,
            INumbersChecker numbersChecker,
            IPharmaciesService pharmaciesService,
            ICompaniesService companiesService,
            IPharmacyChainsService pharmacyChainsService,
            IRegionsService regionsService,
            ICitiesService citiesService)

        {

            this.hostEnvironment = hostEnvironment;
            this.numbersChecker = numbersChecker;
            this.pharmaciesService = pharmaciesService;
            this.companiesService = companiesService;
            this.pharmacyChainsService = pharmacyChainsService;
            this.regionsService = regionsService;
            this.citiesService = citiesService;

        }

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ImportAsync()
        {

            IFormFile file = Request.Form.Files[0];

            string folderName = "UploadExcel";

            string webRootPath = hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            if (!Directory.Exists(newPath))

            {

                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)

            {

                string sFileExtension = Path.GetExtension(file.FileName).ToLower();

                ISheet sheet;

                string fullPath = Path.Combine(newPath, file.FileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))

                {

                    file.CopyTo(stream);

                    stream.Position = 0;

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

                        var newPharmacy = new PharmacyInputModel();

                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {
                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                            }

                            switch (j)
                            {
                                case 0:
                                    if (numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        newPharmacy.BrandexId = int.Parse(currentRow);
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 1:
                                    break;

                                case 2:
                                    if (currentRow == "")
                                    {
                                        newPharmacy.PharmacyClass = PharmacyClass.Other;
                                    }
                                    else
                                    {
                                        newPharmacy.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass), currentRow, true);
                                    }
                                    break;

                                case 3:
                                    if (currentRow == "1")
                                    {
                                        newPharmacy.Active = true;
                                    }
                                    else
                                    {
                                        newPharmacy.Active = false;
                                    }
                                    break;

                                case 4:
                                    int companyId = await this.companiesService.IdByName(currentRow);
                                    if (companyId!=0)
                                    {
                                        //int companyId = await this.companiesService.IdByName(currentRow);
                                        newPharmacy.CompanyId = companyId;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 5:
                                    newPharmacy.Name = currentRow;
                                    break;

                                case 6:
                                    int chainId = await this.pharmacyChainsService.IdByName(currentRow);
                                    if (chainId!=0)
                                    {
                                        newPharmacy.PharmacyChainId = chainId;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 7:
                                    newPharmacy.Address = currentRow;
                                    break;

                                case 9:
                                    int regionId = await this.regionsService.IdByName(currentRow);

                                    if (regionId!=0)
                                    {
                                        newPharmacy.RegionId = regionId;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 10:
                                    break;
                                case 11:
                                    break;
                                case 12:
                                    break;
                                case 13:
                                    break;
                                case 14:
                                    break;

                                case 15:
                                    if (currentRow != "")
                                    {
                                        if (numbersChecker.WholeNumberCheck(currentRow))
                                        {
                                            newPharmacy.PharmnetId = int.Parse(currentRow);
                                        }
                                        
                                    }
                                    break;

                                case 16:
                                    if (currentRow != "")
                                    {
                                        if (numbersChecker.WholeNumberCheck(currentRow))
                                        {
                                            newPharmacy.PhoenixId = int.Parse(currentRow);
                                        }
                                        
                                    }
                                    break;

                                case 17:
                                    if (currentRow != "")
                                    {
                                        if (numbersChecker.WholeNumberCheck(currentRow))
                                        {
                                            newPharmacy.SopharmaId = int.Parse(currentRow);
                                        }
                                        
                                    }
                                    break;

                                case 18:
                                    if (currentRow != "")
                                    {
                                        if (numbersChecker.WholeNumberCheck(currentRow))
                                        {
                                            newPharmacy.StingId = int.Parse(currentRow);
                                        }
                                        
                                    }
                                    break;

                                case 19:
                                    break;
                                case 20:
                                    break;

                                case 21:
                                    int cityId = await this.citiesService.IdByName(currentRow);
                                    if (cityId!=0)
                                    {
                                        //int cityId = await this.citiesService.IdByName(currentRow);
                                        newPharmacy.CityId = cityId;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;
                            }
                        }

                        await this.pharmaciesService.CreatePharmacy(newPharmacy);

                    }   
                }
            }

            var pharmacyErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            return this.View(pharmacyErrorModel);
        }

        [Authorize]
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
                && await this.companiesService.CheckCompanyByName(companyName)
                && await this.pharmacyChainsService.CheckPharmacyChainByName(pharmacyChainName)
                && await this.citiesService.CheckCityName(cityName)
                && await this.regionsService.CheckRegionByName(regionName)
                && address != null)
            {
                var pharmacyInputModel = new PharmacyInputModel
                {
                    BrandexId = brandexId,
                    Name = name,
                    PharmacyClass = pharmacyClass,
                    Active = active,
                    CompanyId = await this.companiesService.IdByName(companyName),
                    PharmacyChainId = await this.pharmacyChainsService.IdByName(pharmacyChainName),
                    Address = address,
                    CityId = await this.citiesService.IdByName(cityName),
                    PharmnetId = pharmnetId,
                    PhoenixId = phoenixId,
                    SopharmaId = sopharmaId,
                    StingId = stingId,
                    RegionId = await this.regionsService.IdByName(regionName)

                };

                if(await this.pharmaciesService.CreatePharmacy(pharmacyInputModel) != "")
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

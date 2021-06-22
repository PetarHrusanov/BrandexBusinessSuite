namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Models.Brandex;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using BrandexSalesAdapter.ExcelLogic.Services;
    using BrandexSalesAdapter.ExcelLogic.Services.Distributor;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.Products;
    using BrandexSalesAdapter.ExcelLogic.Services.Sales;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using static Common.DataConstants.Ditributors;

    public class BrandexController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly ISalesService salesService;
        private readonly IProductsService productsService;
        private readonly IPharmaciesService pharmaciesService;
        private readonly IDistributorService distributorService;

        // user service


        // universal Services
        private readonly INumbersChecker numbersChecker;

        public BrandexController(
            IWebHostEnvironment hostEnvironment,
            ISalesService salesService,
            INumbersChecker numbersChecker,
            IProductsService productsService,
            IPharmaciesService pharmaciesService,
            IDistributorService distributorService
            )

        {

            this.hostEnvironment = hostEnvironment;
            this.salesService = salesService;
            this.numbersChecker = numbersChecker;
            this.productsService = productsService;
            this.pharmaciesService = pharmaciesService;
            this.distributorService = distributorService;

        }

        //[Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        //[Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm] BrandexInputModel brandexInput)
        {

            // version with BrandexInput
            string dateFromClient = brandexInput.Date;

            DateTime dateForDb = DateTime.ParseExact(dateFromClient, "dd-MM-yyyy", null);

            string distributorName = brandexInput.Distributor;

            // version with BrandexInput
            IFormFile file = brandexInput.ImageFile;

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

                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = headerRow.GetCell(j);

                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                    }

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newSale = new SaleInputModel();
                        newSale.Date = dateForDb;


                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {

                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                            }

                            switch (distributorName)
                            {
                                case BrandexENG:

                                    switch (j)
                                    {
                                        case 0:
                                            if (ResolveDate(currentRow, Brandex) != null)
                                            {
                                                newSale.Date = (DateTime)ResolveDate(currentRow, Brandex);
                                            }

                                            else
                                            {
                                                errorDictionary[i] = currentRow;
                                            }

                                            break;

                                        case 3:

                                            if (await ResolveProductIDAsync(currentRow, Brandex) != 0)
                                            {
                                                newSale.ProductId = (int)await ResolveProductIDAsync(currentRow, Brandex);
                                            }

                                            else
                                            {
                                                errorDictionary[i] = currentRow;
                                            }
                                            break;
                                        case 4:
                                            if (ResolveSaleCount(currentRow) != null)
                                            {
                                                newSale.Count = (int)ResolveSaleCount(currentRow);
                                            }
                                            else
                                            {
                                                errorDictionary[i] = currentRow;
                                            }
                                            break;
                                        case 5:
                                            if (await ResolvePharmacyID(currentRow, Brandex) != 0)
                                            {
                                                newSale.PharmacyId = await ResolvePharmacyID(currentRow, Brandex);
                                            }
                                            else
                                            {
                                                errorDictionary[i] = currentRow;
                                            }
                                            break;

                                    }

                                    break;

                                case SopharmaENG:


                                    break;



                            }

                        }

                        await this.salesService.CreateSale(newSale, Brandex);
                    }

                }

            }

            var brandexOutputModel = new BrandexOutputModel {
                Date = dateFromClient,
                Errors = errorDictionary
            };

            string brandexOutputSerialized = JsonConvert.SerializeObject(brandexOutputModel);

            return brandexOutputSerialized;

            //return this.View(brandexOutputModel);

        }

        [Authorize]
        [HttpPost]
        public async Task<string> Check(IFormFile ImageFile)
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

                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = headerRow.GetCell(j);

                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                    }

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {

                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                            }

                            if (j > 5)
                            {
                                continue;
                            }

                            switch (j)
                            {
                                case 0:
                                    break;

                                case 1:
                                    break;

                                case 2:
                                    break;

                                case 3:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        if (await this.productsService.ProductIdByDistributor(currentRow, Brandex) == 0)
                                        {
                                            errorDictionary[i] = currentRow;
                                        }
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 4:
                                    if (currentRow == "" || !this.numbersChecker.NegativeNumberIncludedCheck(currentRow))
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 5:
                                    if (!this.numbersChecker.WholeNumberCheck(currentRow)
                                        || await this.pharmaciesService.PharmacyIdByDistributor(currentRow, Brandex) ==0)
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                            }

                        }
                    }

                }

            }

            var brandexOutputModel = new BrandexOutputModel
            {
                Errors = errorDictionary
            };

            string brandexOutputSerializsed = JsonConvert.SerializeObject(brandexOutputModel);

            return brandexOutputSerializsed;

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(string pharmacyId, string productId, string date, int count)
        {
            if(await this.salesService.UploadIndividualSale(pharmacyId, productId, date, count, Brandex))
            {
                var saleOutputModel = new SaleOutputModel
                {
                    ProductName = await this.productsService.NameById(productId, Brandex),
                    PharmacyName = await this.pharmaciesService.NameById(pharmacyId, Brandex),
                    Count = count,
                    Date = date,
                    DistributorName = Brandex
                };
                return this.View(saleOutputModel);
            }

            return Redirect("Index");
        }

        public static DateTime? ResolveDate(string inputDate, string inputDistributor)
        {
            switch (inputDistributor)
            {
                case Brandex:
                    return DateTime.ParseExact(inputDate, "yyyy-MM", null);
                case Phoenix:
                    return DateTime.Parse(inputDate);
                case Sopharma:
                    return DateTime.ParseExact(inputDate, "MM.yyyy", null);
                default:
                    return null;
            }       
        }

        public async Task<int> ResolveProductIDAsync(string inputProductID, string inputDistributor)
        {

            if (!numbersChecker.WholeNumberCheck(inputProductID))
            {
                return 0;
            }
            switch (inputDistributor)
            {
                case Brandex:
                    var producId = await productsService.ProductIdByDistributor(inputProductID, Brandex);
                    return producId;

            }

                

            return 0;
        }

        public int? ResolveSaleCount(string inputSaleCount)
        {
            if (this.numbersChecker.NegativeNumberIncludedCheck(inputSaleCount))
            {
                return int.Parse(inputSaleCount);
            }

            return null;

        }

        public async Task<int> ResolvePharmacyID(string inputPharmacyID, string inputDistributor)
        {

            if (this.numbersChecker.WholeNumberCheck(inputPharmacyID))
            {
                switch (inputDistributor)
                {
                    case Brandex:
                        if (await this.pharmaciesService.CheckPharmacyByDistributor(inputPharmacyID, Brandex))
                        {
                            return await this.pharmaciesService.PharmacyIdByDistributor(inputPharmacyID, Brandex);
                        }

                        return 0;
                }

            }

            return 0;
        }


    }
}

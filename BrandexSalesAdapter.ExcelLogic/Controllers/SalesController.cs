namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BrandexSalesAdapter.ExcelLogic.Models.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Models.Products;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using BrandexSalesAdapter.ExcelLogic.Services;
    using BrandexSalesAdapter.ExcelLogic.Services.Distributor;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.Products;
    using BrandexSalesAdapter.ExcelLogic.Services.Sales;
    
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    
    using static Common.DataConstants.Ditributors;
    using static Common.DataConstants.ExcelLineErrors;

    public class SalesController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // db Services
        private readonly ISalesService _salesService;
        private readonly IProductsService _productsService;
        private readonly IPharmaciesService _pharmaciesService;
        private readonly IDistributorService _distributorService;

        // user service


        // universal Services
        private readonly INumbersChecker _numbersChecker;

        public SalesController(
            IWebHostEnvironment hostEnvironment,
            ISalesService salesService,
            INumbersChecker numbersChecker,
            IProductsService productsService,
            IPharmaciesService pharmaciesService,
            IDistributorService distributorService
            )

        {

            this._hostEnvironment = hostEnvironment;
            this._salesService = salesService;
            this._numbersChecker = numbersChecker;
            this._productsService = productsService;
            this._pharmaciesService = pharmaciesService;
            this._distributorService = distributorService;

        }

        //[Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Generate([FromBody] SalesRegionDateInputModel inputModel)
        {
            var date = inputModel.Date;
            var regionId = inputModel.RegionId;
            
            string sWebRootFolder = _hostEnvironment.WebRootPath;
            string sFileName = @"Sales.xlsx";

            var URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            var memory = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {

                List<PharmacyExcelModel> collectionPharmacies;

                IWorkbook workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("sales");

                IRow row = excelSheet.CreateRow(0);

                var products = await this._productsService.GetProductsIdPrices();
                int productCounter = 4;
                int counter = 1;

                int currentProduct = 0;
                
                double allSalesSum = 0;
                double rowSum = 0;

                productCounter = await CreateHeaderColumnsAsync(row, productCounter);

                if (date != null)
                {
                    var currRowDate = DateTime.ParseExact(date, "MM/yyyy", null);
                    collectionPharmacies = await this._pharmaciesService.GetPharmaciesExcelModel(currRowDate, regionId);

                    foreach (var pharmacy in collectionPharmacies)
                    {
                        row = excelSheet.CreateRow(counter);
                        rowSum = 0;

                        row.CreateCell(0).SetCellValue(pharmacy.Name);
                        row.CreateCell(1).SetCellValue(pharmacy.Address);
                        row.CreateCell(2).SetCellValue(pharmacy.PharmacyClass.ToString());
                        row.CreateCell(3).SetCellValue(pharmacy.Region);

                        productCounter = 4;
                        currentProduct = 0;

                        foreach (var product in products)
                        {
                            int sumCount = pharmacy.Sales.Where(i => i.ProductId == product.Id).Sum(b => b.Count);
                            rowSum += sumCount * product.Price;
                            allSalesSum += rowSum;
                            row.CreateCell(productCounter).SetCellValue(sumCount);
                            productCounter++;
                            currentProduct++;
                        }

                        row.CreateCell(productCounter).SetCellValue(rowSum);

                        counter++;

                    }

                    row = excelSheet.CreateRow(counter);

                    productCounter = 4;
                    foreach (var product in products)
                    {
                        int sumCount = await this._salesService.ProductCountSumByIdDate(product.Id, currRowDate, regionId);
                        row.CreateCell(productCounter).SetCellValue(sumCount);
                        productCounter++;
                    }

                    counter++;
                    row = excelSheet.CreateRow(counter);

                    productCounter = 4;

                    foreach (var product in products)
                    {
                        int sumCount = await this._salesService.ProductCountSumByIdDate(product.Id, currRowDate, regionId);
                        double productRevenue = sumCount * product.Price;
                        row.CreateCell(productCounter).SetCellValue(productRevenue);
                        productCounter++;
                    }

                    row.CreateCell(productCounter).SetCellValue(allSalesSum);


                }

                else
                {

                    row.CreateCell(productCounter + 1).SetCellValue("Date");

                    var dates = await this._salesService.GetDistinctDatesByMonths();

                    foreach (var currentDate in dates)
                    {
                        // da se mahat li tezi bez prodajbi
                        collectionPharmacies = await this._pharmaciesService.GetPharmaciesExcelModel(currentDate, regionId);

                        foreach (var pharmacy in collectionPharmacies)
                        {
                            rowSum = 0;

                            row = excelSheet.CreateRow(counter);

                            row.CreateCell(0).SetCellValue(pharmacy.Name);
                            row.CreateCell(1).SetCellValue(pharmacy.Address);
                            row.CreateCell(2).SetCellValue(pharmacy.PharmacyClass.ToString());
                            row.CreateCell(3).SetCellValue(pharmacy.Region);

                            productCounter = 4;

                            currentProduct = 0;

                            foreach (var product in products)
                            {
                                int sumCount = pharmacy.Sales.Where(i => i.ProductId == product.Id).Sum(b => b.Count);
                                rowSum += sumCount * product.Price;
                                allSalesSum += sumCount * product.Price;
                                row.CreateCell(productCounter).SetCellValue(sumCount);
                                productCounter++;
                                currentProduct++;
                            }

                            row.CreateCell(productCounter).SetCellValue(rowSum);
                            row.CreateCell(productCounter + 1).SetCellValue(currentDate.ToString());

                            counter++;

                        }
                    }

                    row = excelSheet.CreateRow(counter);

                    productCounter = 4;
                    foreach (var product in products)
                    {
                        int sumCount = await this._salesService.ProductCountSumById(product.Id, regionId);
                        row.CreateCell(productCounter).SetCellValue(sumCount);
                        productCounter++;
                    }

                    counter++;
                    row = excelSheet.CreateRow(counter);


                    productCounter = 4;
                    foreach (var product in products)
                    {
                        int sumCount = await this._salesService.ProductCountSumById(product.Id, regionId);
                        double productRevenue = sumCount * product.Price;
                        row.CreateCell(productCounter).SetCellValue(productRevenue);
                        productCounter++;
                    }

                    row.CreateCell(productCounter).SetCellValue(allSalesSum);

                }

                workbook.Write(fs);

            }

            await using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {

                await stream.CopyToAsync(memory);

            }

            memory.Position = 0;

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        //[Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm] SalesBulkInputModel salesBulkInput)
        {
            
            string dateFromClient = salesBulkInput.Date;

            DateTime dateForDb = DateTime.ParseExact(dateFromClient, "dd-MM-yyyy", null);

            string distributorName = salesBulkInput.Distributor;
            
            IFormFile file = salesBulkInput.ImageFile;

            string folderName = "UploadExcel";

            string webRootPath = _hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();
            var productIdsForCheck = await _productsService.GetProductsCheck();

            if (!Directory.Exists(newPath))

            {
                Directory.CreateDirectory(newPath);
            }

            if (file.Length > 0)

            {

                string sFileExtension = Path.GetExtension(file.FileName)?.ToLower();

                ISheet sheet;

                string fullPath = Path.Combine(newPath, file.FileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {

                    await file.CopyToAsync(stream);

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

                    // IRow headerRow = sheet.GetRow(0); //Get Header Row
                    //
                    // int cellCount = headerRow.LastCellNum;
                    //
                    // for (int j = 0; j < cellCount; j++)
                    // {
                    //     ICell cell = headerRow.GetCell(j);
                    //
                    //     if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    //
                    // }

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newSale = new SaleInputModel
                        {
                            Date = dateForDb
                        };
                        
                        switch (distributorName)
                        {
                            case Brandex:

                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale,
                                    errorDictionary, Brandex, 0, 3, 5, 4);

                                await this._salesService.CreateSale(newSale, Brandex);

                                break;
                            
                            case Phoenix:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Phoenix,16,0,2,14);

                                await this._salesService.CreateSale(newSale, Phoenix);
                                
                                break;
                            
                            case Sting:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Sting,0,1,6,11);

                                await this._salesService.CreateSale(newSale, Sting);
                                
                                break;
                            
                            case Pharmnet:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Pharmnet,9,2,4,11);

                                await this._salesService.CreateSale(newSale, Pharmnet);
                                
                                break;
                            
                            case Sopharma:
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Sopharma,0,2,5,11);

                                await this._salesService.CreateSale(newSale, Sopharma);
                                
                                break;
                            
                        }
                    }

                }

            }

            var outputModel = new SalesBulkOutputModel {
                Date = dateFromClient,
                Errors = errorDictionary
            };

            string outputSerialized = JsonConvert.SerializeObject(outputModel);

            return outputSerialized;

        }

        // [Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<string> Check([FromForm] SalesBulkInputModel salesBulkInput)
        {

            string dateFromClient = salesBulkInput.Date;

            DateTime dateForDb = DateTime.ParseExact(dateFromClient, "dd-MM-yyyy", null);

            string distributorName = salesBulkInput.Distributor;
            
            IFormFile file = salesBulkInput.ImageFile;

            string folderName = "UploadExcel";

            string webRootPath = _hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();
            var productIdsForCheck = await _productsService.GetProductsCheck();

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (file.Length > 0)

            {

                string sFileExtension = Path.GetExtension(file.FileName)?.ToLower();

                string fullPath = Path.Combine(newPath, file.FileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
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

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newSale = new SaleInputModel
                        {
                            Date = dateForDb
                        };
                        
                        switch (distributorName)
                        {
                            case Brandex:

                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale,
                                    errorDictionary, Brandex, 0, 3, 5, 4);

                                // await this._salesService.CreateSale(newSale, Brandex);

                                break;
                            
                            case Phoenix:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Phoenix,16,0,2,14);

                                // await this._salesService.CreateSale(newSale, Phoenix);
                                
                                break;
                            
                            case Sting:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Sting,0,1,6,11);

                                // await this._salesService.CreateSale(newSale, Sting);
                                
                                break;
                            
                            case Pharmnet:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Pharmnet,9,2,4,11);

                                // await this._salesService.CreateSale(newSale, Pharmnet);
                                
                                break;
                            
                            case Sopharma:
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Sopharma,0,2,5,11);

                                // await this._salesService.CreateSale(newSale, Sopharma);
                                
                                break;
                            
                        }
                    }

                }

            }

            var outputModel = new SalesBulkOutputModel {
                Date = dateFromClient,
                Errors = errorDictionary
            };

            string outputSerialized = JsonConvert.SerializeObject(outputModel);

            return outputSerialized;

        }

        // [Authorize]
        [HttpPost]
        public async Task<string> Upload([FromBody]SaleSingleInputModel saleSingleInputModel)
        {

            if(await this._salesService.UploadIndividualSale(
                   saleSingleInputModel.PharmacyId, 
                   saleSingleInputModel.ProductId, 
                   saleSingleInputModel.Date, 
                   saleSingleInputModel.Count, 
                   saleSingleInputModel.Distributor))
            {
                var saleOutputModel = new SaleOutputModel
                {
                    ProductName = await this._productsService.NameById(saleSingleInputModel.ProductId, saleSingleInputModel.Distributor),
                    PharmacyName = await this._pharmaciesService.NameById(saleSingleInputModel.PharmacyId, saleSingleInputModel.Distributor),
                    Count = saleSingleInputModel.Count,
                    Date = saleSingleInputModel.Date,
                    Distributor = saleSingleInputModel.Distributor
                };
                
                string outputSerialized = JsonConvert.SerializeObject(saleOutputModel);

                return outputSerialized;
                
                // return this.View(saleOutputModel);
            }

            return "END";
        }

        private static DateTime? ResolveDate(string? inputDate, string inputDistributor)
        {
            if (inputDate==null)
            {
                return null;
            }
            
            switch (inputDistributor)
            {
                case Brandex:
                    return DateTime.ParseExact(inputDate, "yyyy-MM", null);
                case Phoenix:
                    return DateTime.Parse(inputDate);
                case Sting:
                    return DateTime.ParseExact(inputDate, "yyyy-MM", null);
                case Pharmnet:
                    return DateTime.Parse(inputDate);
                case Sopharma:
                    return DateTime.ParseExact(inputDate, "MM.yyyy", null);
                default:
                    return null;
            }       
        }

        public int ResolveProductId(string inputProductId, string inputDistributor, List<ProductCheckModel> productsToCheck)
        {
            if (inputProductId == null)
            {
                return 0;
            }
            
            var productId = 0;

            if (inputDistributor==Sopharma)
            {
                productId = productsToCheck.Where(p => p.SopharmaId == inputProductId).Select(p => p.Id).FirstOrDefault();
                return productId != 0 ? productId : 0;
            }

            if (!_numbersChecker.WholeNumberCheck(inputProductId))
            {
                return productId;
            }

            int inputProductIdInt = int.Parse(inputProductId);

            switch (inputDistributor)
            {
                case Brandex:
                    productId = productsToCheck.Where(p => p.BrandexId == inputProductIdInt).Select(p => p.Id).FirstOrDefault();
                    return productId != 0 ? productId : 0;
                case Phoenix:
                    productId = productsToCheck.Where(p => p.PhoenixId == inputProductIdInt).Select(p => p.Id).FirstOrDefault();
                    return productId != 0 ? productId : 0;
                case Sting:
                    productId = productsToCheck.Where(p => p.StingId == inputProductIdInt).Select(p => p.Id).FirstOrDefault();
                    return productId != 0 ? productId : 0;
                case Pharmnet:
                    productId = productsToCheck.Where(p => p.PharmnetId == inputProductIdInt).Select(p => p.Id).FirstOrDefault();
                    return productId != 0 ? productId : 0;
            }

            return 0;
            
        }

        public int? ResolveSaleCount(string inputSaleCount)
        {
            if (inputSaleCount==null)
            {
                return null;
            }
            if (this._numbersChecker.NegativeNumberIncludedCheck(inputSaleCount))
            {
                return int.Parse(inputSaleCount);
            }

            return null;

        }

        public int ResolvePharmacyId(string inputPharmacyId, string inputDistributor, List<PharmacyCheckModel> pharmaciesToCheck)
        {
            if (inputPharmacyId == null)
            {
                return 0;
            }

            var pharmacyId = 0;

            if (this._numbersChecker.WholeNumberCheck(inputPharmacyId))
            {
                int inputPharmacyIdInt = int.Parse(inputPharmacyId);
                
                switch (inputDistributor)
                {
                    case Brandex:
                        pharmacyId = pharmaciesToCheck.Where(p => p.BrandexId == inputPharmacyIdInt).Select(p => p.Id).FirstOrDefault();
                        return pharmacyId != 0 ? pharmacyId : 0;
                    case Phoenix:pharmacyId = pharmaciesToCheck.Where(p => p.PhoenixId == inputPharmacyIdInt).Select(p => p.Id).FirstOrDefault();
                        return pharmacyId != 0 ? pharmacyId : 0;
                    case Sting:pharmacyId = pharmaciesToCheck.Where(p => p.StingId == inputPharmacyIdInt).Select(p => p.Id).FirstOrDefault();
                        return pharmacyId != 0 ? pharmacyId : 0;
                    case Pharmnet:pharmacyId = pharmaciesToCheck.Where(p => p.PharmnetId == inputPharmacyIdInt).Select(p => p.Id).FirstOrDefault();
                        return pharmacyId != 0 ? pharmacyId : 0;
                    case Sopharma:pharmacyId = pharmaciesToCheck.Where(p => p.SopharmaId == inputPharmacyIdInt).Select(p => p.Id).FirstOrDefault();
                        return pharmacyId != 0 ? pharmacyId : 0;
                }

            }

            return pharmacyId;
        }
        
        private void CreateSaleInputModel(
            List<ProductCheckModel> productIdsForCheck,
            List<PharmacyCheckModel> pharmacyIdsForCheck,
            IRow row,
            int i,
            SaleInputModel newSale, 
            Dictionary<int, string> errorDictionary,
            string distributor,
            int dateColumn,
            int productIdColumn,
            int pharmacyIdColumn,
            int saleCountColumn)
        {
            
            var dateRow = row.GetCell(dateColumn);
            
            if (dateRow.CellType == CellType.Numeric)
            {
                newSale.Date = DateTime.FromOADate(dateRow.NumericCellValue);
            }

            int productRow = ResolveProductId(row.GetCell(productIdColumn).ToString()?.TrimEnd(), distributor, productIdsForCheck);

            if (productRow != 0)
            {
                newSale.ProductId = productRow;
            }
                                
            else
            {
                errorDictionary[i+1] = IncorrectProductId + " "+productRow;
            }
            int pharmacyIdRow = ResolvePharmacyId(row.GetCell(pharmacyIdColumn).ToString()?.TrimEnd(), distributor, pharmacyIdsForCheck);

            if (pharmacyIdRow != 0)
            {
                newSale.PharmacyId = pharmacyIdRow;
            }
            else
            {
                errorDictionary[i+1] = IncorrectPharmacyId;
            }
            
            var saleCountRow = ResolveSaleCount(row.GetCell(saleCountColumn).ToString()?.TrimEnd());

            if (saleCountRow!=null)
            {
                newSale.Count = (int)saleCountRow;
            }

            else
            {
                errorDictionary[i+1] = IncorrectSalesCount;
            }
            
        }
        
        private async Task<int> CreateHeaderColumnsAsync(IRow row, int counter)
        {
            var products = await this._productsService.GetProductsNames();
            row.CreateCell(0).SetCellValue("Pharmacy Name");
            row.CreateCell(1).SetCellValue("Pharmacy Address");
            row.CreateCell(2).SetCellValue("Pharmacy Class");
            row.CreateCell(3).SetCellValue("Region");

            foreach (var product in products)
            {


                row.CreateCell(counter).SetCellValue(product);
                counter++;
            }

            row.CreateCell(counter).SetCellValue("SumSale");

            return counter;
        }
        
    }
}

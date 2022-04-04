using System.Globalization;

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

    using Newtonsoft.Json;
    
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    
    using Models.Pharmacies;
    using Models.Products;
    using Models.Sales;
    
    using Services;
    using Services.Distributor;
    using Services.Pharmacies;
    using Services.Products;
    using Services.Sales;
    
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
        
        
        private const int BrandexDateColumn = 0;
        private const int BrandexProductIdColumn = 1;
        private const int BrandexCountColumn = 2;
        private const int BrandexPharmacyIdColumn = 3;
        
        private const int ProductCounter = 4;


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
            _hostEnvironment = hostEnvironment;
            _salesService = salesService;
            _numbersChecker = numbersChecker;
            _productsService = productsService;
            _pharmaciesService = pharmaciesService;
            _distributorService = distributorService;

        }

        //[Authorize]
        // [HttpGet]
        // public ActionResult Index()
        // {
        //     return this.View();
        // }
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GenerateSummed([FromBody] SalesRegionDateInputModel inputModel)
        {
            var dateBeginString = inputModel.DateBegin;
            var dateEndString = inputModel.DateEnd;

            DateTime.TryParseExact(dateBeginString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateBegin);
            
            if (!DateTime.TryParseExact(dateEndString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateEnd))
            {
                dateEnd = DateTime.MaxValue;
            }
            
            var regionId = inputModel.RegionId;
            
            var sWebRootFolder = _hostEnvironment.WebRootPath;
            var sFileName = @"Sales.xlsx";

            var memory = new MemoryStream();

            await using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();

                var excelSheet = workbook.CreateSheet("sales");

                var row = excelSheet.CreateRow(0);
                
                var products = await _productsService.GetProductsIdPrices();
                
                
                foreach (var product in products)
                {
                    var sumCount = await _salesService.ProductCountSumByIdDate(product.Id, dateBegin, dateEnd, regionId);
                    row.CreateCell(ProductCounter +row.Cells.Count()).SetCellValue(sumCount);
      
                }
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum+1);
                
                foreach (var product in products)
                {
                    var sumCount = await _salesService.ProductCountSumByIdDate(product.Id, dateBegin, dateEnd, regionId);
                    var productRevenue = sumCount * product.Price;
                    row.CreateCell(row.Cells.Count()+ProductCounter).SetCellValue(productRevenue);
                }


                row = excelSheet.CreateRow(excelSheet.LastRowNum+1);

                await CreateHeaderColumnsAsync(row);

                var collectionPharmacies = await _pharmaciesService.GetPharmaciesExcelModel(dateBegin, dateEnd, regionId);

                foreach (var pharmacy in collectionPharmacies)
                {
                    row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

                    row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.Name);
                    row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.Address);
                    row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.PharmacyClass.ToString());
                    row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.Region);


                    foreach (var product in products)
                    {
                        int sumCount = pharmacy.Sales.Where(i => i.ProductId == product.Id).Sum(b => b.Count);

                        row.CreateCell(row.Cells.Count()).SetCellValue(sumCount);
                    }

                    row.CreateCell(row.Cells.Count())
                        .SetCellValue(pharmacy.Sales.Select(p => p.ProductPrice * p.Count).Sum());
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
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GenerateSeparated([FromBody] SalesRegionDateInputModel inputModel)
        {
            var dateBeginString = inputModel.DateBegin;
            var dateEndString = inputModel.DateEnd;

            DateTime.TryParseExact(dateBeginString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateBegin);
            
            
            if (!DateTime.TryParseExact(dateEndString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateEnd))
            {
                dateEnd = DateTime.MaxValue;
            }
            
            var regionId = inputModel.RegionId;
            
            var sWebRootFolder = _hostEnvironment.WebRootPath;
            var sFileName = @"Sales.xlsx";

            var memory = new MemoryStream();
            
            var products = await _productsService.GetProductsIdPrices();

            await using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create,
                             FileAccess.Write))
            {

                List<PharmacyExcelModel> collectionPharmacies;

                IWorkbook workbook = new XSSFWorkbook();

                var excelSheet = workbook.CreateSheet("sales");

                var row = excelSheet.CreateRow(0);

                foreach (var product in products)
                {
                    var sumCount =
                        await _salesService.ProductCountSumByIdDate(product.Id, dateBegin, dateEnd, regionId);
                    row.CreateCell(ProductCounter + row.Cells.Count()).SetCellValue(sumCount);

                }

                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

                foreach (var product in products)
                {
                    var sumCount =
                        await _salesService.ProductCountSumByIdDate(product.Id, dateBegin, dateEnd, regionId);
                    var productRevenue = sumCount * product.Price;
                    row.CreateCell(row.Cells.Count() + ProductCounter).SetCellValue(productRevenue);
                }


                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

                await CreateHeaderColumnsAsync(row);

                row.CreateCell(row.Cells.Count()).SetCellValue("Date");

                var dates = await _salesService.GetDistinctDatesByMonths();

                foreach (var currentDate in dates)
                {
                    collectionPharmacies = await _pharmaciesService.GetPharmaciesExcelModel(dateBegin, dateEnd, regionId);

                    foreach (var pharmacy in collectionPharmacies)
                    {

                        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

                        row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.Name);
                        row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.Address);
                        row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.PharmacyClass.ToString());
                        row.CreateCell(row.Cells.Count()).SetCellValue(pharmacy.Region);


                        foreach (var product in products)
                        {
                            int sumCount = pharmacy.Sales
                                .Where(i => i.ProductId == product.Id && i.Date==currentDate)
                                .Sum(b => b.Count);

                            row.CreateCell(row.Cells.Count()).SetCellValue(sumCount);
                        }

                        row.CreateCell(row.Cells.Count())
                            .SetCellValue(pharmacy.Sales.Select(p => p.ProductPrice * p.Count).Sum());
                        
                        row.CreateCell(row.Cells.Count()).SetCellValue(currentDate.ToString("yyyy/MM/dd"));
                    }
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
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm] SalesBulkInputModel salesBulkInput)
        {
            
            var dateFromClient = salesBulkInput.Date;

            var dateForDb = DateTime.ParseExact(dateFromClient, "dd-MM-yyyy", null);

            var distributorName = salesBulkInput.Distributor;
            
            var file = salesBulkInput.ImageFile;

            var folderName = "UploadExcel";

            var webRootPath = _hostEnvironment.WebRootPath;

            var newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            var validSalesList = new List<SaleInputModel>();

            var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();
            var productIdsForCheck = await _productsService.GetProductsCheck();

            if (!Directory.Exists(newPath))

            {
                Directory.CreateDirectory(newPath);
            }

            if (file.Length > 0)

            {

                var sFileExtension = Path.GetExtension(file.FileName)?.ToLower();

                ISheet sheet;

                var fullPath = Path.Combine(newPath, file.FileName);

                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                stream.Position = 0;

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

                for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                {

                    var row = sheet.GetRow(i);

                    if (row == null) continue;

                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    var newSale = new SaleInputModel
                    {
                        Date = dateForDb,
                        DistributorId = await _distributorService.IdByName(salesBulkInput.Distributor)
                    };
                        
                    switch (distributorName)
                    {
                        case Brandex:

                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale,
                                errorDictionary, Brandex, BrandexDateColumn, BrandexProductIdColumn, BrandexPharmacyIdColumn, BrandexCountColumn);

                            // await _salesService.CreateSale(newSale, Brandex);

                            break;
                            
                        case Phoenix:
                                
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Phoenix,16,0,2,14);

                            // await _salesService.CreateSale(newSale, Phoenix);
                                
                            break;
                            
                        case Sting:
                                
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Sting,0,1,6,11);

                            // await _salesService.CreateSale(newSale, Sting);
                                
                            break;
                            
                        case Pharmnet:
                                
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Pharmnet,9,2,4,11);

                            // await _salesService.CreateSale(newSale, Pharmnet);
                                
                            break;
                            
                        case Sopharma:
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Sopharma,0,2,5,11);

                            // await _salesService.CreateSale(newSale, Sopharma);
                                
                            break;
                            
                    }

                    if (newSale.PharmacyId != 0
                        && newSale.ProductId != 0
                        && newSale.Count != 0
                        && newSale.DistributorId != 0)
                    {
                        validSalesList.Add(newSale);
                    }
                }
            }

            var arrayWithSubtractedNumbersForPython = new List<int>();


            await _salesService.UploadBulk(validSalesList);
            
            foreach(var entry in errorDictionary)
            {
                (arrayWithSubtractedNumbersForPython).Add(entry.Key - 2);
            }

            var outputModel = new SalesBulkOutputModel
            {
                Date = dateFromClient,
                Errors = errorDictionary,
                ErrorsArray = arrayWithSubtractedNumbersForPython.ToArray()

            };

            var outputSerialized = JsonConvert.SerializeObject(outputModel);

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

                    var newSale = new SaleInputModel
                    {
                        Date = dateForDb
                    };
                        
                    switch (distributorName)
                    {
                        case Brandex:

                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale,
                                errorDictionary, Brandex, BrandexDateColumn, BrandexProductIdColumn, BrandexPharmacyIdColumn, BrandexCountColumn);

                            break;
                            
                        case Phoenix:
                                
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Phoenix,16,0,2,14);

                            break;
                            
                        case Sting:
                                
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Sting,0,1,6,11);

                            break;
                            
                        case Pharmnet:
                                
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Pharmnet,9,2,4,11);

                            break;
                            
                        case Sopharma:
                            CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                errorDictionary,Sopharma,0,2,5,11);

                            break;
                            
                    }
                }
            }

            var arrayWithSubtractedNumbersForPython = new List<int>();
            
            
            foreach(KeyValuePair<int, string> entry in errorDictionary)
            {
                (arrayWithSubtractedNumbersForPython).Add(entry.Key - 2);
            }
                


            var outputModel = new SalesBulkOutputModel
            {
                Date = dateFromClient,
                Errors = errorDictionary,
                ErrorsArray = arrayWithSubtractedNumbersForPython.ToArray()

            };

            string outputSerialized = JsonConvert.SerializeObject(outputModel);

            return outputSerialized;

        }

        // [Authorize]
        [HttpPost]
        public async Task<string> Upload([FromBody]SaleSingleInputModel saleSingleInputModel)
        {

            if(await _salesService.UploadIndividualSale(
                   saleSingleInputModel.PharmacyId, 
                   saleSingleInputModel.ProductId, 
                   saleSingleInputModel.Date, 
                   saleSingleInputModel.Count, 
                   saleSingleInputModel.Distributor))
            {
                var saleOutputModel = new SaleOutputModel
                {
                    ProductName = await _productsService.NameById(saleSingleInputModel.ProductId, saleSingleInputModel.Distributor),
                    PharmacyName = await _pharmaciesService.NameById(saleSingleInputModel.PharmacyId, saleSingleInputModel.Distributor),
                    Count = saleSingleInputModel.Count,
                    Date = saleSingleInputModel.Date,
                    Distributor = saleSingleInputModel.Distributor
                };
                
                var outputSerialized = JsonConvert.SerializeObject(saleOutputModel);

                return outputSerialized;
            }

            return "END";
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
            if (_numbersChecker.NegativeNumberIncludedCheck(inputSaleCount))
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

            if (!_numbersChecker.WholeNumberCheck(inputPharmacyId)) return pharmacyId;
            var inputPharmacyIdInt = int.Parse(inputPharmacyId);
                
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

            var productRow = ResolveProductId(row.GetCell(productIdColumn).ToString()?.TrimEnd(), distributor, productIdsForCheck);

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
            
            // var saleCountRow = ResolveSaleCount(row.GetCell(saleCountColumn).ToString()?.TrimEnd());

            var saleCountRow = row.GetCell(saleCountColumn);
            if (saleCountRow!=null)
            {
                if (int.TryParse(saleCountRow.ToString()?.TrimEnd(), out var saleCountInt))
                {
                    newSale.Count = saleCountInt;
                }
                
                else
                {
                    errorDictionary[i+1] = IncorrectSalesCount;
                }
            }

        }
        
        private async Task CreateHeaderColumnsAsync(IRow row)
        {
            var products = await _productsService.GetProductsNames();
            row.CreateCell(0).SetCellValue("Pharmacy Name");
            row.CreateCell(1).SetCellValue("Pharmacy Address");
            row.CreateCell(2).SetCellValue("Pharmacy Class");
            row.CreateCell(3).SetCellValue("Region");

            // int cellsCount = row.Cells.Count();

            foreach (var product in products)
            {


                row.CreateCell(row.Cells.Count()).SetCellValue(product);
                // counter++;
            }

            row.CreateCell(row.Cells.Count()).SetCellValue("SumSale");

            // return counter;
        }
        
    }
}

namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Models.Brandex;
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
        private readonly IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly ISalesService salesService;
        private readonly IProductsService productsService;
        private readonly IPharmaciesService pharmaciesService;
        private readonly IDistributorService distributorService;

        // user service


        // universal Services
        private readonly INumbersChecker numbersChecker;

        public SalesController(
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

            var pharmacyIdsForCheck = await pharmaciesService.GetPharmaciesCheck();
            var productIdsForCheck = await productsService.GetProductsCheck();

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

                        var newSale = new SaleInputModel
                        {
                            Date = dateForDb
                        };
                        
                        switch (distributorName)
                        {
                            case Brandex:

                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale,
                                    errorDictionary, Brandex, 0, 3, 5, 4);

                                await this.salesService.CreateSale(newSale, Brandex);

                                break;
                            
                            case Phoenix:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Phoenix,16,0,2,14);

                                await this.salesService.CreateSale(newSale, Phoenix);
                                
                                break;
                            
                            case Sting:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Sting,0,1,6,11);

                                await this.salesService.CreateSale(newSale, Sting);
                                
                                break;
                            
                            case Pharmnet:
                                
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Pharmnet,9,2,4,11);

                                await this.salesService.CreateSale(newSale, Pharmnet);
                                
                                break;
                            
                            case Sopharma:
                                CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row,i,newSale,
                                    errorDictionary,Sopharma,0,2,5,11);

                                await this.salesService.CreateSale(newSale, Sopharma);
                                
                                break;
                            
                        }
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

        // [Authorize]
        [HttpPost]
        public async Task<string> Check(IFormFile imageFile)
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

        // [Authorize]
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

        private static DateTime? ResolveDate(string inputDate, string inputDistributor)
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

            if (!numbersChecker.WholeNumberCheck(inputProductId))
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
            if (this.numbersChecker.NegativeNumberIncludedCheck(inputSaleCount))
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

            if (this.numbersChecker.WholeNumberCheck(inputPharmacyId))
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
           
            var dateRow = ResolveDate(row.GetCell(dateColumn).ToString()?.TrimEnd(), distributor); 
            
            if (dateRow != null)
            {
                newSale.Date = (DateTime)dateRow;
            }

            else
            {
                errorDictionary[i+1] = IncorrectDateFormat;
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
                errorDictionary[i+1] = IncorrectSalesCount + " "+saleCountRow;
            }
            
        }
        
    }
}

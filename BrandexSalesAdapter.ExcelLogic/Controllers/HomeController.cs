namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using BrandexSalesAdapter.ExcelLogic.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using BrandexSalesAdapter.ExcelLogic.Models.Pharmacies;
    using System;
    using BrandexSalesAdapter.ExcelLogic.Services.Products;
    using BrandexSalesAdapter.ExcelLogic.Services.Sales;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.Regions;
    using Microsoft.AspNetCore.Authorization;
    using BrandexSalesAdapter.ExcelLogic.Models.Regions;

    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // db Services
        private readonly IProductsService _productsService;
        private readonly ISalesService _salesService;
        private readonly IPharmaciesService _pharmaciesService;
        private readonly IRegionsService _regionsService;

        public HomeController(
            IWebHostEnvironment hostEnvironment,
            IProductsService productsService,
            ISalesService salesService,
            IPharmaciesService pharmaciesService,
            IRegionsService regionsService)

        {

            this._hostEnvironment = hostEnvironment;
            this._productsService = productsService;
            this._salesService = salesService;
            this._pharmaciesService = pharmaciesService;
            this._regionsService = regionsService;

        }

        public async Task<IActionResult> Index()
        {
            var inputFilter = new SaleFiltersExcelInputModel();

            inputFilter.Options =  await this._regionsService.RegionsForSelect();

            return View(inputFilter);
        }

        [HttpPost]
        public async Task<IActionResult> Export(string date = null)
        {
            return await GenerateSalesFile(date, null);
        }

        // Method Version for Angular Adaptation
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ByCity([FromBody] SalesRegionDateInputModel input)
        {
            var regionId = input.RegionId;
            var date = input.Date;

            return await GenerateSalesFile(date, regionId);
        }

        // Method Version for Basic Asp Core MVC Logic
        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //public async Task<IActionResult> ByCity([FromBody] SalesRegionDateInputModel input)
        //{
        //    var regionId = input.RegionId;
        //    var date = input.Date;

        //    return await GenerateSalesFile(date, regionId);
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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


        private async Task<FileStreamResult> GenerateSalesFile(string date, int? regionId = null)
        {
            string sWebRootFolder = _hostEnvironment.WebRootPath;
            string sFileName = @"Sales.xlsx";

            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            var memory = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {

                var collectionPhamracies = new List<PharmacyExcelModel>();

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
                    collectionPhamracies = await this._pharmaciesService.GetPharmaciesExcelModel(currRowDate, regionId);

                    foreach (var pharmacy in collectionPhamracies)
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
                        collectionPhamracies = await this._pharmaciesService.GetPharmaciesExcelModel(currentDate, regionId);

                        foreach (var pharmacy in collectionPhamracies)
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
    }
}

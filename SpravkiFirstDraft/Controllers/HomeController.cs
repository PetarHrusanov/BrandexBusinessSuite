namespace SpravkiFirstDraft.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Models;
    using SpravkiFirstDraft.Models.Sales;
    using SpravkiFirstDraft.Models.Pharmacies;
    using System;
    using SpravkiFirstDraft.Services.Products;

    public class HomeController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly SpravkiDbContext context;
        private readonly IProductsService productsService;

        public HomeController(IWebHostEnvironment hostEnvironment,
            SpravkiDbContext context,
            IProductsService productsService)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;
            this.productsService = productsService;

        }

        public IActionResult Index()
        {
            var inputFilter = new SaleFiltersExcelInputModel();

            inputFilter.Options = context.Regions.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Name
                                  }).ToList();

            return View(inputFilter);
        }

        public async Task<IActionResult> Export(string date = null)
        {
            string sWebRootFolder = hostEnvironment.WebRootPath;
            string sFileName = @"Employees.xlsx";

            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            var memory = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {

                var collectionPhamracies = new List<PharmacyExcelModel>();

                IWorkbook workbook;

                workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("employee");
                IRow row = excelSheet.CreateRow(0);

                if (date != null)
                {
                    var currRowDate = DateTime.ParseExact(date, "MM/yyyy", null);
                    collectionPhamracies = context.Pharmacies.Select(p => new PharmacyExcelModel
                    {
                        Name = p.Name,
                        Address = p.Address,
                        PharmacyClass = p.PharmacyClass,
                        Region = p.Region.Name,
                        Sales = p.Sales
                            .Where(d => d.Date.Month == currRowDate.Month && d.Date.Year == currRowDate.Year)
                            .Select(s => new SaleExcelOutputModel
                        {
                            Name = s.Product.Name,
                            Count = s.Count,
                            Date = currRowDate
                        }).ToList()
                    }).ToList();

                    row.CreateCell(0).SetCellValue("Pharmacy Name");
                    row.CreateCell(1).SetCellValue("Pharmacy Address");
                    row.CreateCell(2).SetCellValue("Pharmacy Class");
                    row.CreateCell(3).SetCellValue("Region");

                    var products = await this.productsService.GetProductsNames();

                    int productCounter = 4;
                    foreach (var product in products)
                    {


                        row.CreateCell(productCounter).SetCellValue(product);
                        productCounter++;
                    }

                    int counter = 1;

                    foreach (var pharmacy in collectionPhamracies)
                    {
                        row = excelSheet.CreateRow(counter);

                        row.CreateCell(0).SetCellValue(pharmacy.Name);
                        row.CreateCell(1).SetCellValue(pharmacy.Address);
                        row.CreateCell(2).SetCellValue(pharmacy.PharmacyClass.ToString());
                        row.CreateCell(3).SetCellValue(pharmacy.Region);

                        productCounter = 4;
                        foreach (var product in products)
                        {

                            row.CreateCell(productCounter).SetCellValue(pharmacy.Sales.Where(i => i.Name == product).Sum(b => b.Count));
                            productCounter++;
                        }

                        counter++;

                    }
                }
                else
                {
                    var datesRough = this.context.Sales.Select(s => s.Date).Distinct().ToList();
                    var dates = datesRough.Select(t => new DateTime(t.Year, t.Month, 1)).Distinct().ToList();

                    row.CreateCell(0).SetCellValue("Pharmacy Name");
                    row.CreateCell(1).SetCellValue("Pharmacy Address");
                    row.CreateCell(2).SetCellValue("Pharmacy Class");
                    row.CreateCell(3).SetCellValue("Region");
                    row.CreateCell(4).SetCellValue("Date");

                    var products = await this.productsService.GetProductsNames();

                    int productCounter = 5;

                    foreach (var product in products)
                    {


                        row.CreateCell(productCounter).SetCellValue(product);
                        productCounter++;
                    }

                    int counter = 1;

                    foreach (var currentDate in dates)
                    {
                        // da se mahat li tezi bez prodajbi
                        collectionPhamracies = context.Pharmacies.Where(p => p.Sales.Count>0).Select(p => new PharmacyExcelModel
                        {
                            Name = p.Name,
                            Address = p.Address,
                            PharmacyClass = p.PharmacyClass,
                            Region = p.Region.Name,
                            Sales = p.Sales
                            //.Where(s => s.)
                            .Where(d => d.Date.Month == currentDate.Month && d.Date.Year == currentDate.Year)
                            .Select(s => new SaleExcelOutputModel
                            {
                                Name = s.Product.Name,
                                Count = s.Count,
                                Date = currentDate
                            }).ToList()
                        }).ToList();

                        foreach (var pharmacy in collectionPhamracies)
                        {
                            row = excelSheet.CreateRow(counter);

                            row.CreateCell(0).SetCellValue(pharmacy.Name);
                            row.CreateCell(1).SetCellValue(pharmacy.Address);
                            row.CreateCell(2).SetCellValue(pharmacy.PharmacyClass.ToString());
                            row.CreateCell(3).SetCellValue(pharmacy.Region);
                            row.CreateCell(4).SetCellValue(currentDate.ToString());

                            productCounter = 5;
                            foreach (var product in products)
                            {

                                row.CreateCell(productCounter).SetCellValue(pharmacy.Sales.Where(i => i.Name == product).Sum(b => b.Count));
                                productCounter++;
                            }

                            counter++;

                        }
                    }



                    
                }
 
                workbook.Write(fs);
 
            }
 
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
 
                await stream.CopyToAsync(memory);
 
            }

            memory.Position = 0;

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

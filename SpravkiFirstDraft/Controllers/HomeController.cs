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
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models;
    using SpravkiFirstDraft.Models.Sales;

    public class HomeController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;

        public HomeController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Export()
        {
            string sWebRootFolder = hostEnvironment.WebRootPath;
            string sFileName = @"Employees.xlsx";

            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            var memory = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {

                IWorkbook workbook;
 
                workbook = new XSSFWorkbook();
 
                ISheet excelSheet = workbook.CreateSheet("employee");
 
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("Pharmacy Name");

                row.CreateCell(1).SetCellValue("Pharmacy Address");

                row.CreateCell(2).SetCellValue("Pharmacy Class");

                row.CreateCell(3).SetCellValue("Bland");

                row.CreateCell(4).SetCellValue("Sleep");

                row.CreateCell(5).SetCellValue("Laxal");

                row.CreateCell(6).SetCellValue("Flexen");

                row.CreateCell(7).SetCellValue("ForFlex");

                row.CreateCell(8).SetCellValue("GinkgoVin");

                row.CreateCell(9).SetCellValue("GinkgoVin + Centella");

                row.CreateCell(10).SetCellValue("Ceget+");

                row.CreateCell(11).SetCellValue("EnzyMill 60");

                row.CreateCell(12).SetCellValue("EnzyMill 15");

                row.CreateCell(13).SetCellValue("CystiRen");

                row.CreateCell(14).SetCellValue("ProstaRen");

                row.CreateCell(15).SetCellValue("DetoxiFive");

                row.CreateCell(16).SetCellValue("LadyHarmonia");

                row.CreateCell(17).SetCellValue("DiabeFor Gluco");

                row.CreateCell(18).SetCellValue("DiabeFor Protect");

                row.CreateCell(19).SetCellValue("ZinSeD");

                //var collectionPharmacies = context.Pharmacies.Where(p => p.Active == true);

                var collectionPharmacies = context.Pharmacies.Select(p => new {
                    Name = p.Name,
                    Address = p.Address,
                    PharmacyClass = p.PharmacyClass,
                    Sales =
                    context.Sales.Where(id => id.PharmacyId == p.Id).Select(s => new SaleExcelOutputModel
                    {
                        Name = s.Product.Name,
                        Count = s.Count
                    }).ToList()

                }).ToList();

                int counter = 1;

                foreach (var item in collectionPharmacies)
                {
                    row = excelSheet.CreateRow(counter);

                    row.CreateCell(0).SetCellValue(item.Name);
                    row.CreateCell(1).SetCellValue(item.Address);
                    row.CreateCell(2).SetCellValue(item.PharmacyClass.ToString());

                    row.CreateCell(3).SetCellValue(item.Sales.Where(i => i.Name == "Бланд 30 табл.").Sum(b => b.Count));

                    //row.CreateCell(3).SetCellValue(item.Sales.Where(i => i.Product.Id == 20).Sum(b => b.Count));

                    row.CreateCell(4).SetCellValue(item.Sales.Where(i => i.Name == "Слийп 30 табл.").Sum(b => b.Count));
                    row.CreateCell(5).SetCellValue(item.Sales.Where(i => i.Name == "Лаксал (псилиум)").Sum(b => b.Count));
                    row.CreateCell(6).SetCellValue(item.Sales.Where(i => i.Name == "Флексен").Sum(b => b.Count));
                    row.CreateCell(7).SetCellValue(item.Sales.Where(i => i.Name == "Форфлекс").Sum(b => b.Count));
                    row.CreateCell(8).SetCellValue(item.Sales.Where(i => i.Name == "Гинко Вин").Sum(b => b.Count));
                    row.CreateCell(9).SetCellValue(item.Sales.Where(i => i.Name == "Гинко Вин+ Центела").Sum(b => b.Count));
                    row.CreateCell(10).SetCellValue(item.Sales.Where(i => i.Name == "Цегет+ ( с добавен Селен)").Sum(b => b.Count));
                    row.CreateCell(11).SetCellValue(item.Sales.Where(i => i.Name == "Ензи-Мил").Sum(b => b.Count));
                    row.CreateCell(12).SetCellValue(item.Sales.Where(i => i.Name == "Ензи-мил компакт").Sum(b => b.Count));
                    row.CreateCell(13).SetCellValue(item.Sales.Where(i => i.Name == "Цистирен").Sum(b => b.Count));
                    row.CreateCell(14).SetCellValue(item.Sales.Where(i => i.Name == "ПростаРен").Sum(b => b.Count));
                    row.CreateCell(15).SetCellValue(item.Sales.Where(i => i.Name == "ДетоксиФайв").Sum(b => b.Count));
                    row.CreateCell(16).SetCellValue(item.Sales.Where(i => i.Name == "Лейди Хармония").Sum(b => b.Count));
                    row.CreateCell(17).SetCellValue(item.Sales.Where(i => i.Name == "ДиабеФор Глюко").Sum(b => b.Count));
                    row.CreateCell(18).SetCellValue(item.Sales.Where(i => i.Name == "ДиабеФор Протект").Sum(b => b.Count));
                    row.CreateCell(19).SetCellValue(item.Sales.Where(i => i.Name == "ЗинСеД").Sum(b => b.Count));

                    counter++;
                  
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

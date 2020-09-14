namespace SpravkiFirstDraft.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models.Pharmnet;
    using SpravkiFirstDraft.Models.Sales;
    using SpravkiFirstDraft.Models.Sopharma;
    using SpravkiFirstDraft.Models.Sting;
    using SpravkiFirstDraft.Services.Sales;

    public class StingController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;
        private readonly ISalesService salesService;

        public StingController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context, ISalesService salesService)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;
            this.salesService = salesService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ImportAsync(StingInputModel stingInput)
        {

            IFormFile file = Request.Form.Files[0];

            DateTime dateForDb = DateTime.ParseExact(stingInput.Date, "dd-MM-yyyy", null);

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

                            switch (j)
                            {
                                case 0:

                                    var currRowDate = DateTime.ParseExact(currentRow, "yyyy-MM", null);
                                    if (currentRow != null)
                                    {
                                        newSale.Date = currRowDate;
                                    }
                                    break;
                                case 1:
                                    if(this.context.Products.Where(c=> c.StingId == int.Parse(currentRow)).Any())
                                    {
                                        var producId = this.context.Products.Where(c => c.StingId == int.Parse(currentRow)).Select(p => p.Id).First();
                                        newSale.ProductId = producId;
                                    }

                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;
                                case 6:
                                    int rowPharmacytId = int.Parse(currentRow);
                                    if (this.context.Pharmacies.Where(c => c.StingId == rowPharmacytId).Any())
                                    {
                                        var pharmacyId = this.context.Pharmacies.Where(c => c.StingId == rowPharmacytId).Select(p => p.Id).First();
                                        newSale.PharmacyId = pharmacyId;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;
                                case 11:
                                    int countProduct = int.Parse(currentRow);
                                    newSale.Count = countProduct;
                                    break;

                            }


                        }

                        await salesService.CreateSale(newSale, "Sting");


                    }

                }

            }

            var stingOtuptModel = new StingOutputModel {
                Date = stingInput.Date,
                Errors = errorDictionary
            };

            return this.View(stingOtuptModel);

        }
    }
}

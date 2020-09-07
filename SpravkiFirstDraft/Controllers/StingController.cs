namespace SpravkiFirstDraft.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models.Pharmnet;
    using SpravkiFirstDraft.Models.Sopharma;
    using SpravkiFirstDraft.Models.Sting;

    public class StingController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;

        public StingController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Import(StingInputModel stingInput)
        {

            IFormFile file = Request.Form.Files[0];

            DateTime dateForDb = DateTime.ParseExact(stingInput.Date, "yyyy-MM-dd", null);

            string folderName = "UploadExcel";

            string webRootPath = hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            StringBuilder sb = new StringBuilder();

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

                    sb.Append("<table class='table table-bordered'><tr>");

                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = headerRow.GetCell(j);

                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                        sb.Append("<th>" + cell.ToString() + "</th>");

                    }

                    sb.Append("</tr>");

                    sb.AppendLine("<tr>");

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        Sale newSale = new Sale();
                        newSale.Date = dateForDb;


                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {

                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
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

                        if(newSale.PharmacyId!=0
                            && newSale.ProductId!=0
                            && newSale.Date !=null
                            && newSale.Count != 0)
                        {
                            var stingId = context.Distributors
                           .Where(n => n.Name == "Sting")
                           .Select(i => i.Id)
                           .FirstOrDefault();
                            newSale.DistributorId = stingId;
                            context.Sales.Add(newSale);
                            context.Sales.Add(newSale);
                            context.SaveChanges();
                        }

                        

                        sb.AppendLine("</tr>");

                    }

                    sb.Append("</table>");

                }

            }

            var stingOtuptModel = new StingOutputModel();

            stingOtuptModel.Date = stingInput.Date;

            stingOtuptModel.Table = sb.ToString();

            stingOtuptModel.Errors = errorDictionary;

            return this.View(stingOtuptModel);

        }
    }
}

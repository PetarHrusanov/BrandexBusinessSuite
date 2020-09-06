namespace SpravkiFirstDraft.Controllers
{
    using System;
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
    using SpravkiFirstDraft.Data.Enums;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models.Phoenix;

    public class PhoenixController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;

        public PhoenixController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Import(PhoenixInputModel phoenixInput)
        {

            IFormFile file = Request.Form.Files[0];

            DateTime dateForDb = DateTime.ParseExact(phoenixInput.Date, "yyyy-MM-dd", null);

            string folderName = "UploadExcel";

            string webRootPath = hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

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

                            if (j == 0)
                            {
                                int rowProductId = int.Parse(currentRow);
                                var productId = this.context.Products.Where(c => c.PhoenixId == rowProductId).Select(p => p.Id).First();
                                newSale.ProductId = productId;
                            }
                            if (j == 2)
                            {
                                int rowPharmacytId = int.Parse(currentRow);
                                var pharmacyId = this.context.Pharmacies.Where(c => c.PhoenixId == rowPharmacytId).Select(p => p.Id).First();
                                newSale.PharmacyId = pharmacyId;

                            }

                            if (j == 14)
                            {
                                int countProduct = int.Parse(currentRow);
                                newSale.Count = countProduct;
                            }

                            if (j == 16)
                            {
                                var currRowDate = DateTime.Parse(currentRow);

                                if (currentRow != null)
                                {
                                    newSale.Date = currRowDate;
                                }

 
                            }


                        }

                        var phoenixId = context.Distributors
                            .Where(n => n.Name == "Phoenix")
                            .Select(i => i.Id)
                            .FirstOrDefault();
                        newSale.DistributorId = phoenixId;
                        context.Sales.Add(newSale);
                        context.SaveChanges();

                        sb.AppendLine("</tr>");

                    }

                    sb.Append("</table>");

                }

            }

            var phoenixOutput = new PhoenixOutputModel();

            phoenixOutput.Date = phoenixInput.Date;

            phoenixOutput.Table = sb.ToString();

            return this.View(phoenixOutput);

        }
    }
}

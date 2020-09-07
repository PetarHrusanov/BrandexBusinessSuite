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

    public class PharmacyDetailsController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;

        public PharmacyDetailsController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {

            IFormFile file = Request.Form.Files[0];

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

                        Pharmacy newPharmacy = new Pharmacy();

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
                                newPharmacy.BrandexId = int.Parse(currentRow);
                            }
                            if (j == 2)
                            {
                                if(currentRow== "")
                                {
                                    newPharmacy.PharmacyClass = PharmacyClass.Other;
                                }

                                else
                                {
                                    newPharmacy.PharmacyClass = (PharmacyClass)Enum.Parse(typeof(PharmacyClass), currentRow, true);
                                }
          
                            }
                            if (j == 3)
                            {
                                if (currentRow == "1")
                                {
                                    newPharmacy.Active = true;
                                }
                                else
                                {
                                    newPharmacy.Active = false;
                                }
                            }
                            if (j == 4)
                            {
                                int companyId = context.Companies
                                    .Where(x => x.Name.ToLower()
                                    .TrimEnd().Contains(currentRow.ToLower().TrimEnd()))
                                    .Select(x => x.Id).FirstOrDefault();
                                newPharmacy.CompanyId = companyId;
                            }
                            if (j == 5)
                            {
                                newPharmacy.Name = currentRow;
                            }
                            if (j == 6)
                            {
                                int chainId = context.PharmacyChains
                                    .Where(x => x.Name.ToLower().TrimEnd() == currentRow.ToLower().TrimEnd())
                                    .Select(x => x.Id).FirstOrDefault();
                                newPharmacy.PharmacyChainId = chainId;
                            }
                            if (j == 7)
                            {
                                newPharmacy.Address = currentRow;
                            }

                            if (j == 9)
                            {
                                int regionId = context.Regions
                                    .Where(x => x.Name.ToLower()
                                    .TrimEnd().Contains(currentRow.ToLower().TrimEnd()))
                                    .Select(x => x.Id).FirstOrDefault();
                                newPharmacy.RegionId = regionId;
                            }

                            if (j == 15)
                            {
                                if (currentRow != "")
                                {
                                    newPharmacy.PharmnetId = int.Parse(currentRow);
                                }
                            }

                            if (j == 16)
                            {
                                if (currentRow != "")
                                {
                                    newPharmacy.PhoenixId = int.Parse(currentRow);
                                }
                            }
                            if (j == 17)
                            {
                                if (currentRow != "")
                                {
                                    newPharmacy.SopharmaId = int.Parse(currentRow);
                                }
                            }
                            if (j == 18)
                            {
                                if (currentRow != "")
                                {
                                    newPharmacy.StingId = int.Parse(currentRow);
                                }
                            }
                            if (j == 21)
                            {
                                int cityId = context.Cities
                                    .Where(x => x.Name.ToLower()
                                    .TrimEnd().Contains(currentRow.ToLower().TrimEnd()))
                                    .Select(x => x.Id).FirstOrDefault();
                                newPharmacy.CityId = cityId;
                            }


                        }

                        context.Pharmacies.Add(newPharmacy);
                        context.SaveChanges();

                        sb.AppendLine("</tr>");

                    }

                    sb.Append("</table>");

                }

            }

            return this.Content(sb.ToString());

        }
    }
}

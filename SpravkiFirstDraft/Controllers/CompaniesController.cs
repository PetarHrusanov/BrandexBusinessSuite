﻿namespace SpravkiFirstDraft.Controllers
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

    public class CompaniesController :Controller
    {
        private IWebHostEnvironment hostEnvironment;

        private readonly SpravkiDbContext context;

        public CompaniesController(IWebHostEnvironment hostEnvironment, SpravkiDbContext context)

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

                        Company newCompany = new Company();

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
                                newCompany.Name = currentRow;
                            }
                            if (j == 1)
                            {
                                newCompany.VAT = currentRow;
                            }


                        }

                        context.Companies.Add(newCompany);
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
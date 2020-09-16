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
    using SpravkiFirstDraft.Data.Enums;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models;
    using SpravkiFirstDraft.Models.Companies;
    using SpravkiFirstDraft.Services;
    using SpravkiFirstDraft.Services.Companies;

    public class CompaniesController :Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly SpravkiDbContext context;
        private readonly ICompaniesService companiesService;

        private readonly INumbersChecker numbersChecker;

        public CompaniesController(IWebHostEnvironment hostEnvironment,
            SpravkiDbContext context,
            INumbersChecker numbersChecker,
            ICompaniesService companiesService)

        {

            this.hostEnvironment = hostEnvironment;
            this.context = context;
            this.numbersChecker = numbersChecker;
            this.companiesService = companiesService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> ImportAsync()
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

                        var newCompany = new CompanyInputModel();

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
                                    if (currentRow != null)
                                    {
                                        newCompany.Name = currentRow;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;
                                case 1:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        newCompany.VAT = currentRow;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                            }
                        }

                        await this.companiesService.UploadCompany(newCompany);
                        
                     
                    }
                }
            }

            var errorsCombined = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            return this.View(errorsCombined);

        }

        public async Task<ActionResult> Upload(string companyName, string vat, string ownerName)
        {
            if (companyName != null)
            {
                var inputCity = new CompanyInputModel
                {
                    Name = companyName,
                    VAT = vat,
                    Owner = ownerName
                };

                if(await companiesService.UploadCompany(inputCity))
                {
                    var outputCity = new CompanyOutputModel
                    {
                        Name = inputCity.Name,
                        VAT = inputCity.VAT,
                        Owner = inputCity.Owner
                    };

                    return this.View(outputCity);
                }
                else
                {
                    return Redirect("Index");
                }
                
            }

            else
            {
                return Redirect("Index");
            }
        }
    }
}

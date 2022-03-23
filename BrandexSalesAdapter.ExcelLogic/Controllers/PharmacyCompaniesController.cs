using BrandexSalesAdapter.ExcelLogic.Services.PharmacyCompanies;

namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using BrandexSalesAdapter.ExcelLogic.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Companies;
    using BrandexSalesAdapter.ExcelLogic.Services;
    using Microsoft.AspNetCore.Authorization;
    
    using Newtonsoft.Json;
    
    using static Common.InputOutputConstants.SingleStringConstants;


    public class PharmacyCompaniesController :Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // db Services
        private readonly IPharmacyCompaniesService _pharmacyCompaniesService;

        private readonly INumbersChecker _numbersChecker;

        public PharmacyCompaniesController(
            IWebHostEnvironment hostEnvironment,
            INumbersChecker numbersChecker,
            IPharmacyCompaniesService pharmacyCompaniesService)

        {

            this._hostEnvironment = hostEnvironment;
            this._numbersChecker = numbersChecker;
            this._pharmacyCompaniesService = pharmacyCompaniesService;

        }

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // [Authorize]
        [HttpPost]
        public async Task<ActionResult> ImportAsync()
        {

            IFormFile file = Request.Form.Files[0];

            string folderName = "UploadExcel";

            string webRootPath = _hostEnvironment.WebRootPath;

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
                                    newCompany.VAT = currentRow;
                                    break;

                            }
                        }

                        await this._pharmacyCompaniesService.UploadCompany(newCompany);
                        
                     
                    }
                }
            }

            var errorsCombined = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            return this.View(errorsCombined);

        }

        // [Authorize]
        [HttpPost]
        public async Task<string> Upload([FromBody]SingleStringInputModel singleStringInputModel)
        {
            // if (singleStringInputModel.SingleStringValue != null)
            // {
            //     await this._pharmacyCompaniesService.UploadCompany(singleStringInputModel.SingleStringValue);
            // }
            
            string outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);

            outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

            return outputSerialized;
            
        }
    }
}


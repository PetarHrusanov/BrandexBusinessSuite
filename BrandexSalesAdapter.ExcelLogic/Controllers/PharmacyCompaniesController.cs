﻿namespace BrandexSalesAdapter.ExcelLogic.Controllers
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
    
    using Models;
    using Models.PharmacyCompanies;
    
    using Services;
    using Services.PharmacyCompanies;

    using Newtonsoft.Json;

    using static Common.InputOutputConstants.SingleStringConstants;
    using static Common.DataConstants.ExcelLineErrors;
    
    
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

            _hostEnvironment = hostEnvironment;
            _numbersChecker = numbersChecker;
            _pharmacyCompaniesService = pharmacyCompaniesService;

        }
        

        // [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm]IFormFile file)
        {

            var folderName = "UploadExcel";

            var webRootPath = _hostEnvironment.WebRootPath;

            var newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();


            if (!Directory.Exists(newPath))
            {

                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)
            {

                var sFileExtension = Path.GetExtension(file.FileName)?.ToLower();

                if (file.FileName != null)
                {
                    var fullPath = Path.Combine(newPath, file.FileName);

                    await using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    stream.Position = 0;

                    ISheet sheet;
                    if (sFileExtension == ".xls")

                    {
                    
                        var hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  

                    }

                    else

                    {

                        var hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

                    }

                    var headerRow = sheet.GetRow(0); //Get Header Row

                    int cellCount = headerRow.LastCellNum;

                    for (var j = 0; j < cellCount; j++)
                    {
                        var cell = headerRow.GetCell(j);

                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;


                    }

                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {

                        var row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newCompany = new PharmacyCompanyInputModel();
                    
                        var companyName = row.GetCell(0).ToString()?.TrimEnd();

                        if (!string.IsNullOrEmpty(companyName))
                        {
                            newCompany.Name = companyName;
                        }
                        
                        else
                        {
                            errorDictionary[i+1] = IncorrectPharmacyCompanyName;
                            continue;
                        }

                        var vatRow = row.GetCell(1);
                        if (vatRow!=null)
                        {
                            newCompany.VAT = vatRow.ToString()?.TrimEnd();
                        }
                    

                        await _pharmacyCompaniesService.UploadCompany(newCompany);
                        
                     
                    }
                }
            }

            var errorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            string outputSerialized = JsonConvert.SerializeObject(errorModel);

            return outputSerialized;
            
        }
        
        
        [HttpPost]
        public async Task<string> Upload([FromBody]PharmacyCompanyInputModel pharmacyCompanyInputModel)
        {
            if (pharmacyCompanyInputModel.Name != null)
            {
                await _pharmacyCompaniesService.UploadCompany(pharmacyCompanyInputModel);
            }

            var outputModel = new PharmacyCompanyOutputModel();
            outputModel.Name = pharmacyCompanyInputModel.Name;
            outputModel.VAT = pharmacyCompanyInputModel.VAT;
            outputModel.Owner = pharmacyCompanyInputModel.Owner;
            
            string outputSerialized = JsonConvert.SerializeObject(outputModel);

            outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

            return outputSerialized;

        }
    }
}

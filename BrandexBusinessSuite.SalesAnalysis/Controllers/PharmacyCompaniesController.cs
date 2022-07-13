namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models;
using Infrastructure;
using Models.PharmacyCompanies;
using Services.PharmacyCompanies;

using static BrandexBusinessSuite.Common.InputOutputConstants.SingleStringConstants;
using static BrandexBusinessSuite.Common.ExcelDataConstants.ExcelLineErrors;

public class PharmacyCompaniesController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IPharmacyCompaniesService _pharmacyCompaniesService;

    public PharmacyCompaniesController(IWebHostEnvironment hostEnvironment,
        IPharmacyCompaniesService pharmacyCompaniesService)

    {
        _hostEnvironment = hostEnvironment;
        _pharmacyCompaniesService = pharmacyCompaniesService;
    }

    // [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {
        var newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

        var pharmacyCompaniesCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesCheck();

        var validPharmacyCompanyNames = new List<PharmacyCompanyInputModel>();
        var pharmacyCompaniesEditted = new List<PharmacyCompanyInputModel>();

        if (file.Length > 0)
        {
            var sFileExtension = Path.GetExtension(file.FileName).ToLower();
            var fullPath = Path.Combine(newPath, file.FileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            stream.Position = 0;

            ISheet sheet;
            if (sFileExtension == ".xls")

            {
                var hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                sheet = hssfwb.GetSheetAt(0);
            }

            else
            {
                var hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  

                sheet = hssfwb.GetSheetAt(0);
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
                    errorDictionary[i + 1] = IncorrectPharmacyCompanyName;
                    continue;
                }

                var vatRow = row.GetCell(1);
                if (vatRow != null)
                {
                    newCompany.VAT = vatRow.ToString()?.TrimEnd();
                }

                // Consider implementing check for Company changes, but clean ERP database first.

                if (pharmacyCompaniesCheck.All(p =>
                        !string.Equals(p.Name.TrimEnd(), newCompany.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    validPharmacyCompanyNames.Add(newCompany);
                }
            }

            await _pharmacyCompaniesService.UploadBulk(validPharmacyCompanyNames);
        }

        var errorModel = new CustomErrorDictionaryOutputModel { Errors = errorDictionary };

        string outputSerialized = JsonConvert.SerializeObject(errorModel);

        return outputSerialized;
    }

    [HttpPost]
    public async Task<string> Upload([FromBody] PharmacyCompanyInputModel pharmacyCompanyInputModel)
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
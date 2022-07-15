namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;

using Infrastructure;
using Models.PharmacyCompanies;
using Services.PharmacyCompanies;

using static Methods.ExcelMethods;

using static Common.InputOutputConstants.SingleStringConstants;
using static Common.ExcelDataConstants.ExcelLineErrors;

public class PharmacyCompaniesController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    private readonly IPharmacyCompaniesService _pharmacyCompaniesService;

    public PharmacyCompaniesController(IWebHostEnvironment hostEnvironment,
        IPharmacyCompaniesService pharmacyCompaniesService)
    {
        _hostEnvironment = hostEnvironment;
        _pharmacyCompaniesService = pharmacyCompaniesService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {
        var errorDictionary = new List<string>();

        var pharmacyCompaniesCheck = await _pharmacyCompaniesService.GetPharmacyCompaniesCheck();

        var validPharmacyCompanyNames = new List<PharmacyCompanyInputModel>();

        if (!CheckXlsx(file, errorDictionary)) return JsonConvert.SerializeObject(errorDictionary.ToArray());
        
        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var newCompany = new PharmacyCompanyInputModel();

            var companyName = row.GetCell(0).ToString()?.TrimEnd();
            
            if (string.IsNullOrEmpty(companyName))
            {
                errorDictionary.Add($"{i} Line: {IncorrectPharmacyCompanyName}");
                continue;
            }

            newCompany.Name = companyName;
            
            var vatRow = row.GetCell(1);
            if (vatRow != null) newCompany.VAT = vatRow.ToString()?.TrimEnd();

            // Consider implementing check for Company changes, but clean ERP database first.
            if (pharmacyCompaniesCheck.All(p =>
                    !string.Equals(p.Name.TrimEnd(), newCompany.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                validPharmacyCompanyNames.Add(newCompany);
            }
        }

        await _pharmacyCompaniesService.UploadBulk(validPharmacyCompanyNames);

        var outputSerialized = JsonConvert.SerializeObject(errorDictionary);

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

        var outputSerialized = JsonConvert.SerializeObject(outputModel);
        outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

        return outputSerialized;
    }
}
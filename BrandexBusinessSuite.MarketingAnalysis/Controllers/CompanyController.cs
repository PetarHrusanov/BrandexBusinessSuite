namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Services;
using Infrastructure;
using Models.Companies;
using Services.Companies;
using Common;

using static Methods.ExcelMethods;

public class CompanyController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ICompaniesService _companiesService;

    public CompanyController(IWebHostEnvironment hostEnvironment, ICompaniesService companiesService)
    {
        _hostEnvironment = hostEnvironment;
        _companiesService = companiesService;
    }
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Import([FromForm] IFormFile file)
    {

        // var errorDictionary = new List<string>();

        var companiesCheck = await _companiesService.GetCheckModels();

        var uniqueCompanies = new List<CompaniesInputModel>();

        if (!CheckXlsx(file)) return BadRequest(Constants.Errors.IncorrectFileFormat);

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var name = row.GetCell(0);

            if (name == null) continue;

            var companyName = name.ToString()!.TrimEnd();

            var erpIdRow = row.GetCell(1);
            
            if (erpIdRow == null) continue;

            var erpId = erpIdRow.ToString()!.TrimEnd();

            if (companiesCheck.Any(c => string.Equals(c.Name, companyName, StringComparison.CurrentCultureIgnoreCase))) continue;
            
            var companyNew = new CompaniesInputModel()
            {
                Name = companyName,
                ErpId = erpId
            };
            uniqueCompanies.Add(companyNew);
        }

        await _companiesService.UploadBulk(uniqueCompanies);
        
        return Result.Success;

        // return JsonConvert.SerializeObject(errorDictionary.ToArray());
    }
    
}
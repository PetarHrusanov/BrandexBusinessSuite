namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

using BrandexBusinessSuite.Controllers;
using Services.Companies;
using Infrastructure;
using Models.AdMedias;
using Services.AdMedias;

using static Methods.ExcelMethods;
using static Common.Constants;

public class AdMediaController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IAdMediasService _adMediasService;
    private readonly ICompaniesService _companiesService;
    public AdMediaController(IWebHostEnvironment hostEnvironment, IAdMediasService adMediasService, ICompaniesService companiesService)

    {
        _hostEnvironment = hostEnvironment;
        _adMediasService = adMediasService;
        _companiesService = companiesService;
    }
    
    [HttpGet]
    public async Task<List<AdMediaCheckModel>> GetAdMedias()
    {
        return await _adMediasService.GetCheckModels();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Import([FromForm] IFormFile file)
    {

        var errorDictionary = new List<string>();

        var adMediasCheck = await _adMediasService.GetCheckModels();
        var companies = await _companiesService.GetCheckModels();

        var uniqueMedias = new List<AdMediaInputModel>();

        if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);

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

            var newAdMedia = new AdMediaInputModel();

            var nameRow = row.GetCell(0);

            newAdMedia.Name = nameRow.ToString()?.TrimEnd().ToUpper() ?? throw new InvalidOperationException();

            var companyName = row.GetCell(1);

            if (companyName != null)
            {
                var companyNameConverted = companyName.ToString()?.TrimEnd().ToUpper();
                var companyId = companies.Where(c =>
                    string.Equals(c.Name, companyNameConverted, StringComparison.CurrentCultureIgnoreCase))
                    .Select(i => i.Id).FirstOrDefault();

                newAdMedia.CompanyId = companyId;
            }

            if (adMediasCheck.All(c =>
                    !string.Equals(c.Name, newAdMedia.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                uniqueMedias.Add(newAdMedia);
            }
        }

        await _adMediasService.UploadBulk(uniqueMedias);

        return JsonConvert.SerializeObject(errorDictionary.ToArray());
    }
    
}
namespace BrandexBusinessSuite.MarketingAnalysis.Controllers;

using BrandexBusinessSuite.Controllers;

using Infrastructure;

using BrandexBusinessSuite.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using static Methods.ExcelMethods;
using static Common.Constants;

public class CompanyController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    // private readonly IAdMediasService _adMediasService;

    public CompanyController(IWebHostEnvironment hostEnvironment)
    {
        hostEnvironment = hostEnvironment;
    }
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Import([FromForm] IFormFile file)
    {

        var errorDictionary = new List<string>();

        // var adMediasCheck = await _adMediasService.GetCheckModels();

        var uniqueCompanies = new List<string>();

        if (!CheckXlsx(file)) return BadRequest(Constants.Errors.IncorrectFileFormat);

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

 

            var name = row.GetCell(0);

            if (name != null)
            {
                uniqueCompanies.Add(name.ToString()!.TrimEnd());
            }
            

            if (name != null)
            {
                // if (adMediasCheck.All(c =>
                //         !string.Equals(c.Name, newAdMedia.Name, StringComparison.CurrentCultureIgnoreCase)))
                // {
                //     uniqueMedias.Add(newAdMedia);
                // }
            }

            
        }

        // await _adMediasService.UploadBulk(uniqueMedias);

        return JsonConvert.SerializeObject(errorDictionary.ToArray());
    }
    
}
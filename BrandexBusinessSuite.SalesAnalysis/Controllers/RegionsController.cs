namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.Controllers;
using Infrastructure;
using Models.Regions;
using Services.Regions;

using static Methods.ExcelMethods;

using static Common.InputOutputConstants.SingleStringConstants;
using static Common.ExcelDataConstants.ExcelLineErrors;

public class RegionsController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IRegionsService _regionService;

    public RegionsController(
        IWebHostEnvironment hostEnvironment,
        IRegionsService regionService)

    {
        _hostEnvironment = hostEnvironment;
        _regionService = regionService;
    }

    [HttpGet]
    public async Task<RegionOutputModel[]> GetRegions()
    {
        return await _regionService.AllRegions();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {
        var errorDictionary = new List<string>();

        if (!CheckXlsx(file, errorDictionary)) return JsonConvert.SerializeObject(errorDictionary.ToArray());
        
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

            var regionName = row.GetCell(0)?.ToString()?.TrimEnd();
            if (!string.IsNullOrEmpty(regionName))
            {
                await _regionService.UploadRegion(regionName);
                continue;
            }
            
            errorDictionary.Add($"{i} Line: {IncorrectRegion}");
                
        }

        return JsonConvert.SerializeObject(errorDictionary);
    }

    [HttpPost]
    public async Task<string> Upload([FromBody] SingleStringInputModel singleStringInputModel)
    {
        await _regionService.UploadRegion(singleStringInputModel.SingleStringValue);

        var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);
        outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

        return outputSerialized;
    }
}
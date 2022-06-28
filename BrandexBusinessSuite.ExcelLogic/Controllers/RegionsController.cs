namespace BrandexBusinessSuite.ExcelLogic.Controllers;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
    
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Infrastructure;
using Models.Regions;
using Services.Regions;

using static BrandexBusinessSuite.Common.InputOutputConstants.SingleStringConstants;
using static BrandexBusinessSuite.Common.ExcelDataConstants.ExcelLineErrors;

public class RegionsController: AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
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

    //[Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm]IFormFile file)
    {
            

        string newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

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

                IRow headerRow = sheet.GetRow(0); //Get Header Row

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

                    var regionName = row.GetCell(0).ToString()?.TrimEnd();
                    if (!string.IsNullOrEmpty(regionName))
                    {
                        await _regionService.UploadRegion(regionName);
                    }
                        
                    else
                    {
                        errorDictionary[i+1] = IncorrectRegion;
                    }
                        
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
    public async Task<string> Upload([FromBody]SingleStringInputModel singleStringInputModel)
    {
        if (singleStringInputModel.SingleStringValue != null)
        {
            await _regionService.UploadRegion(singleStringInputModel.SingleStringValue);
        }
            
        string outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);

        outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

        return outputSerialized;
            
    }
}
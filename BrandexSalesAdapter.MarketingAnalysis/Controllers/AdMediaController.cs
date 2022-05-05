using BrandexSalesAdapter.Models;

namespace BrandexSalesAdapter.MarketingAnalysis.Controllers;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using NPOI.XSSF.UserModel;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using BrandexSalesAdapter.Controllers;

public class AdMediaController : ApiController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    // private readonly ICitiesService _citiesService;

    public AdMediaController(
        IWebHostEnvironment hostEnvironment
        // ICitiesService citiesService
        
    )

    {
        _hostEnvironment = hostEnvironment;
        // _citiesService = citiesService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {

        string folderName = "UploadExcel";

        string webRootPath = _hostEnvironment.WebRootPath;

        string newPath = Path.Combine(webRootPath, folderName);

        var errorDictionary = new Dictionary<int, string>();

        // var citiesCheck = await _citiesService.GetCitiesCheck();

        var uniqueCities = new List<string>();
        
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

                    IRow row = sheet.GetRow(i);

                    if (row == null) continue;

                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;


                    var cityRow = row.GetCell(0).ToString()?.TrimEnd();
                    if (!string.IsNullOrEmpty(cityRow))
                    {
                        // if (citiesCheck.All(c =>
                        //         !string.Equals(c.Name, cityRow, StringComparison.CurrentCultureIgnoreCase)))
                        // {
                        //     uniqueCities.Add(cityRow.ToUpper());
                        // }

                    }

                    else
                    {
                        // errorDictionary[i + 1] = IncorrectCityName;
                    }

                }

                // await _citiesService.UploadBulk(uniqueCities);

            }
        }

        var errorModel = new CustomErrorDictionaryOutputModel
        {
            Errors = errorDictionary
        };

        var outputSerialized = JsonConvert.SerializeObject(errorModel);

        return outputSerialized;

    }

    // [HttpPost]
    // public async Task<string> Upload([FromBody] SingleStringInputModel singleStringInputModel)
    // {
    //     if (singleStringInputModel.SingleStringValue != null)
    //     {
    //         await _citiesService.UploadCity(singleStringInputModel.SingleStringValue);
    //     }
    //
    //     var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);
    //
    //     outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);
    //
    //     return outputSerialized;
    //
    // }

}
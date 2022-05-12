namespace BrandexSalesAdapter.MarketingAnalysis.Controllers;

using BrandexSalesAdapter.Controllers;
using Infrastructure;
using Models.AdMedias;
using Services.AdMedias;
using BrandexSalesAdapter.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class MarketingActivityController : ApiController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IAdMediasService _adMediasService;

    public MarketingActivityController(
        IWebHostEnvironment hostEnvironment,
            IAdMediasService adMediasService
    )

    {
        _hostEnvironment = hostEnvironment;
        _adMediasService = adMediasService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {

        string newPath = CreateExcelFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

        var adMediasCheck = await _adMediasService.GetCheckModels();

        var uniqueMedias = new List<AdMediaInputModel>();
        

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

                    var newAdMedia = new AdMediaInputModel();

                    var nameRow = row.GetCell(0);

                    if (nameRow!=null)
                    {
                        newAdMedia.Name = nameRow.ToString()?.TrimEnd().ToUpper() ?? throw new InvalidOperationException();
                    }
                    
                    var typeRow = row.GetCell(1);
                    
                    if (typeRow!=null)
                    {
                        // newAdMedia.MediaType = (MediaType)Enum.Parse(typeof(MediaType), typeRow.ToString()!.TrimEnd(), true);
                    }
                    
                    
                    
                    if (newAdMedia.Name!=null && newAdMedia.MediaType !=null)
                    {
                        if (adMediasCheck.All(c =>
                                !string.Equals(c.Name, newAdMedia.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            uniqueMedias.Add(newAdMedia);
                        }
                        
                    }

                    else
                    { 
                        errorDictionary[i + 1] = "Incorrect Ad Media";
                    }

                }

                await _adMediasService.UploadBulk(uniqueMedias);

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
namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Collections.Generic;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    
    using Models;
    using Services.Cities;
    using Newtonsoft.Json;
    
    using static Common.InputOutputConstants.SingleStringConstants;
    using static Common.DataConstants.ExcelLineErrors;
    
    using BrandexSalesAdapter.Services.Identity;

    public class CitiesController :ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // db Services
        private readonly ICitiesService _citiesService;
        
        private readonly ICurrentUserService _currentUser;

        public CitiesController(
            IWebHostEnvironment hostEnvironment,
            ICitiesService citiesService,
            ICurrentUserService currentUser
            
            )

        {
            _hostEnvironment = hostEnvironment;
            _citiesService = citiesService;
            _currentUser = currentUser;
        }

        // [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm]IFormFile file)
        {

            string folderName = "UploadExcel";

            string webRootPath = _hostEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            var citiesCheck = await _citiesService.GetCitiesCheck();

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
                            if (citiesCheck.All(c => !string.Equals(c.Name, cityRow, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                uniqueCities.Add(cityRow.ToUpper());
                            }
                            
                        }
                        
                        else
                        {
                            errorDictionary[i+1] = IncorrectCityName;
                        }

                    }

                    await _citiesService.UploadBulk(uniqueCities);

                }
            }

            var errorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            var outputSerialized = JsonConvert.SerializeObject(errorModel);

            return outputSerialized;

        }
        
        [HttpPost]
        // [Authorize]
        public async Task<string> Upload([FromBody]SingleStringInputModel singleStringInputModel)
        {
            if (singleStringInputModel.SingleStringValue != null)
            {
                await _citiesService.UploadCity(singleStringInputModel.SingleStringValue);
            }
            
            var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);

            outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

            return outputSerialized;
            
        }
    }
}

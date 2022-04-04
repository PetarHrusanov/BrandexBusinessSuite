namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    
    using Models;
    using Services.PharmacyChains;
    
    using Newtonsoft.Json;
    
    using static Common.InputOutputConstants.SingleStringConstants;
    using static Common.DataConstants.ExcelLineErrors;


    public class PharmacyChainsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // db Services
        private readonly IPharmacyChainsService _pharmacyChainsService;

        public PharmacyChainsController(
            IWebHostEnvironment hostEnvironment,
            IPharmacyChainsService pharmacyChainsService)

        {

            _hostEnvironment = hostEnvironment;
            _pharmacyChainsService = pharmacyChainsService;

        }
        

        // [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<string> Import([FromForm]IFormFile file)
        {

            const string folderName = "UploadExcel";

            var webRootPath = _hostEnvironment.WebRootPath;

            var newPath = Path.Combine(webRootPath, folderName);

            var errorDictionary = new Dictionary<int, string>();

            var pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
            var uniquePharmacyChains = new List<string>();

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

                        var row = sheet.GetRow(i);

                        if (row == null) continue;
                        
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        
                        
                        var chainName = row.GetCell(0);
                        if (chainName!=null)
                        {
                            var chainNameString = chainName.ToString().ToUpper().TrimEnd();
                            
                            if (pharmacyChainsCheck.All(c => !string.Equals(c.Name, chainNameString, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                uniquePharmacyChains.Add(chainNameString);
                            }
                            
                            // await _pharmacyChainsService.UploadPharmacyChain(chainName.ToString()?.TrimEnd());
                        }
                        
                        else
                        {
                            errorDictionary[i+1] = IncorrectPharmacyChainName;
                            
                        }

                    }
                    
                    await _pharmacyChainsService.UploadBulk(uniquePharmacyChains);
                }
            }

            var errorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            var outputSerialized = JsonConvert.SerializeObject(errorModel);

            return outputSerialized;

        }

        // [Authorize]
        [HttpPost]
        public async Task<string> Upload([FromBody]SingleStringInputModel singleStringInputModel)
        {
            if (singleStringInputModel.SingleStringValue != null)
            {
                await _pharmacyChainsService.UploadPharmacyChain(singleStringInputModel.SingleStringValue);
            }
            
            string outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);

            outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

            return outputSerialized;
            
        }
    }
}

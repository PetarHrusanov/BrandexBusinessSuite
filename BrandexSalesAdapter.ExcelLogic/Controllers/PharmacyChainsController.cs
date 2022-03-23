namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
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

        //[Authorize]
        public IActionResult Index()
        {
            return View();
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

            if (!Directory.Exists(newPath))

            {

                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)

            {

                string sFileExtension = Path.GetExtension(file.FileName).ToLower();

                ISheet sheet;

                string fullPath = Path.Combine(newPath, file.FileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))

                {

                    file.CopyTo(stream);

                    stream.Position = 0;

                    if (sFileExtension == ".xls")

                    {

                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  

                    }

                    else

                    {

                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  

                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row

                    int cellCount = headerRow.LastCellNum;

                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = headerRow.GetCell(j);

                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                    }

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        // Distribut newCity = new City();
                        
                        var chainName = row.GetCell(0).ToString()?.TrimEnd();
                        if (!string.IsNullOrEmpty(chainName))
                        {
                            await this._pharmacyChainsService.UploadPharmacyChain(chainName);
                        }
                        
                        else
                        {
                            errorDictionary[i] = "Wrong Pharmacy Chain";
                            continue;
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

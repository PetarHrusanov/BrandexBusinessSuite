namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using BrandexSalesAdapter.ExcelLogic.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Regions;
    using BrandexSalesAdapter.ExcelLogic.Services.Regions;
    using Microsoft.AspNetCore.Authorization;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;

    public class RegionController: Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly IRegionsService regionService;

        public RegionController(
            IWebHostEnvironment hostEnvironment,
            IRegionsService regionService)

        {
            this.hostEnvironment = hostEnvironment;
            this.regionService = regionService;
        }

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<RegionOutputModel[]> GetRegions()
        {
            return await regionService.AllRegions();
    }

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult> Import(IFormFile ImageFile)
        {

            IFormFile file = Request.Form.Files[0];

            string folderName = "UploadExcel";

            string webRootPath = hostEnvironment.WebRootPath;

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
                        

                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {
                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                                await this.regionService.UploadRegion(currentRow);
                            }
                            else
                            {
                                errorDictionary[i] = currentRow;
                                continue;
                            }

                        }

                    }

                }

            }

            var citiesErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            return this.View(citiesErrorModel);

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(string regionName)
        {
            if (await this.regionService.UploadRegion(regionName) != "")
            {
                var regionOutputModel = new RegionOutputModel
                {
                    Name = regionName
                };

                return this.View(regionOutputModel);
            }

            else
            {
                return Redirect("Index");
            }
        }
    }
}


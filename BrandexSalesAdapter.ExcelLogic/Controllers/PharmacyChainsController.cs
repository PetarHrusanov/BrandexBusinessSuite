namespace BrandexSalesAdapter.ExcelLogic.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using BrandexSalesAdapter.ExcelLogic.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.PharmacyChains;
    using BrandexSalesAdapter.ExcelLogic.Services.PharmacyChains;
    using Microsoft.AspNetCore.Authorization;

    public class PharmacyChainsController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly IPharmacyChainsService pharmacyChainsService;

        public PharmacyChainsController(
            IWebHostEnvironment hostEnvironment,
            IPharmacyChainsService pharmacyChainsService)

        {

            this.hostEnvironment = hostEnvironment;
            this.pharmacyChainsService = pharmacyChainsService;

        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Import()
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

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newPharmacyChain = new PharmacyChainsInputModel();

                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {
                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                                await this.pharmacyChainsService.UploadPharmacyChain(currentRow);
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

            var pharmacyChainsErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            return this.View(pharmacyChainsErrorModel);

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(string pharmacyChainName)
        {
            if (pharmacyChainName != null)
            {
                await this.pharmacyChainsService.UploadPharmacyChain(pharmacyChainName);
                var pharmacyChainOutputModel = new PharmacyChainOutputModel
                {
                    Name = pharmacyChainName
                };

                return this.View(pharmacyChainOutputModel);
            }

            else
            {
                return Redirect("Index");
            }
        }
    }
}

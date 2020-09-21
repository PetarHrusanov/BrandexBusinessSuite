namespace SpravkiFirstDraft.Controllers
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
    using SpravkiFirstDraft.Models;
    using SpravkiFirstDraft.Models.Products;
    using SpravkiFirstDraft.Services;
    using SpravkiFirstDraft.Services.Products;

    public class ProductsController :Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly IProductsService productsService;

        private readonly INumbersChecker numbersChecker;


        public ProductsController(
            IWebHostEnvironment hostEnvironment,
            IProductsService productsService,
            INumbersChecker numbersChecker)

        {

            this.hostEnvironment = hostEnvironment;
            this.productsService = productsService;
            this.numbersChecker = numbersChecker;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> ImportAsync()
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

                        var newProduct = new ProductInputModel();

                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {
                            string currentRow = string.Empty;

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                            }

                            switch (j)
                            {
                                case 0:
                                    if(currentRow!= "")
                                    {
                                        newProduct.Name = currentRow;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
             
                                    break;

                                case 1:
                                    if (currentRow != "")
                                    {
                                        newProduct.ShortName = currentRow;
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 2:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        newProduct.BrandexId = int.Parse(currentRow);
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 3:
                                    if (currentRow != "")
                                    {
                                        newProduct.PhoenixId = int.Parse(currentRow);
                                    }
                                    break;

                                case 4:
                                    if (currentRow != "")
                                    {
                                        newProduct.PharmnetId = int.Parse(currentRow);
                                    }
                                    break;

                                case 5:
                                    if (currentRow != "")
                                    {
                                        newProduct.StingId = int.Parse(currentRow);
                                    }
                                    break;

                                case 6:
                                    if (currentRow != "")
                                    {
                                        newProduct.SopharmaId = currentRow;
                                    }
                                    break;

                                case 7:
                                    if (numbersChecker.NegativeNumberIncludedCheck(currentRow))
                                    {
                                        newProduct.Price = double.Parse(currentRow);
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;
                            }

                        }

                        await this.productsService.CreateProduct(newProduct);

                    }
                    
                }

            }

            var productsErrorModel = new CustomErrorDictionaryOutputModel
            {
                Errors = errorDictionary
            };

            return this.View(productsErrorModel);

        }

        public async Task<ActionResult> Upload(string productName,
            string productShortName,
            double productPrice,
            int brandexId,
            int? pharmnetId,
            int? phoenixId,
            string sopharmaId,
            int? stingId)
        {
            if (productName != null &&
                productShortName != null &&
                productPrice != 0 &&
                brandexId != 0)
            {
                var newProduct = new ProductInputModel
                {
                    Name = productName,
                    ShortName = productShortName,
                    Price = productPrice,
                    BrandexId = brandexId,
                    PharmnetId = pharmnetId,
                    PhoenixId = phoenixId,
                    SopharmaId = sopharmaId,
                    StingId = stingId
                };

                if(await this.productsService.CreateProduct(newProduct)!= "")
                {
                    var outputProduct = new ProductOutputModel
                    {
                        Name = productName,
                        ShortName = productShortName,
                        Price = productPrice,
                        BrandexId = brandexId
                    };

                    return this.View(outputProduct);
                }
                else
                {
                    return Redirect("Index");
                }
            }

            else
            {
                return Redirect("Index");
            }
        }
    }
}

﻿namespace BrandexSalesAdapter.ExcelLogic.Controllers
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
    using BrandexSalesAdapter.ExcelLogic.Models.Phoenix;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using BrandexSalesAdapter.ExcelLogic.Services;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.Products;
    using BrandexSalesAdapter.ExcelLogic.Services.Sales;
    using static BrandexSalesAdapter.ExcelLogic.Common.DataConstants.Ditributors;
    using Microsoft.AspNetCore.Authorization;

    public class PhoenixController : Controller
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly ISalesService salesService;
        private readonly IProductsService productsService;
        private readonly IPharmaciesService pharmaciesService;

        // universal Services
        private readonly INumbersChecker numbersChecker;

        public PhoenixController(
            IWebHostEnvironment hostEnvironment,
            ISalesService salesService,
            INumbersChecker numbersChecker,
            IProductsService productsService,
            IPharmaciesService pharmaciesService
            )

        {

            this.hostEnvironment = hostEnvironment;
            this.salesService = salesService;
            this.numbersChecker = numbersChecker;
            this.productsService = productsService;
            this.pharmaciesService = pharmaciesService;

        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ImportAsync(PhoenixInputModel phoenixInput)
        {

            IFormFile file = Request.Form.Files[0];

            DateTime dateForDb = DateTime.ParseExact(phoenixInput.Date, "dd-MM-yyyy", null);

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

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File

                    {

                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        var newSale = new SaleInputModel();
                        newSale.Date = dateForDb;

                        for (int j = row.FirstCellNum; j < cellCount; j++)

                        {

                            string currentRow = "";

                            if (row.GetCell(j) != null)
                            {
                                currentRow = row.GetCell(j).ToString().TrimEnd();
                            }

                            switch (j)
                            {
                                case 0:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        var productId = await this.productsService.ProductIdByDistributor(currentRow, Phoenix);

                                        if (productId!=0)
                                        {
                                            //var productId = await this.productsService.ProductIdByDistributor(currentRow, Phoenix);
                                            newSale.ProductId = productId;
                                        }
                                        else
                                        {
                                            errorDictionary[i] = currentRow;
                                        }
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 2:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        var pharmacyId = await this.pharmaciesService.PharmacyIdByDistributor(currentRow, Phoenix);
                                        if (pharmacyId!=0)
                                        {
                                            //var pharmacyId = await this.pharmaciesService.PharmacyIdByDistributor(currentRow, Phoenix);
                                            newSale.PharmacyId = pharmacyId;
                                        }
                                        else
                                        {
                                            errorDictionary[i] = currentRow;
                                        }
                                    }
                                    else
                                    {
                                        errorDictionary[i] = currentRow;
                                    }
                                    break;

                                case 14:
                                    if (this.numbersChecker.NegativeNumberIncludedCheck(currentRow))
                                    {
                                        int countProduct = int.Parse(currentRow);
                                        newSale.Count = countProduct;
                                    }
                                    break;

                                case 16:
                                    var currRowDate = DateTime.Parse(currentRow);
                                    if (currentRow != null)
                                    {
                                        newSale.Date = currRowDate;
                                    }
                                    break;

                            }


                        }

                        await salesService.CreateSale(newSale, Phoenix);

                    }

                }

            }

            var phoenixOutput = new PhoenixOutputModel();

            phoenixOutput.Date = phoenixInput.Date;

            phoenixOutput.Errors = errorDictionary;

            return this.View(phoenixOutput);

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(string pharmacyId, string productId, string date, int count)
        {
            if (await this.salesService.UploadIndividualSale(pharmacyId, productId, date, count, Phoenix))
            {
                var saleOutputModel = new SaleOutputModel
                {
                    ProductName = await this.productsService.NameById(productId, Phoenix),
                    PharmacyName = await this.pharmaciesService.NameById(pharmacyId, Phoenix),
                    Count = count,
                    Date = date,
                    DistributorName = Phoenix
                };
                return this.View(saleOutputModel);

            }

            return Redirect("Index");
        }
    }
}

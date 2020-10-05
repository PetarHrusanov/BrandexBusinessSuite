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
    using BrandexSalesAdapter.ExcelLogic.Models.Brandex;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using BrandexSalesAdapter.ExcelLogic.Services;
    using BrandexSalesAdapter.ExcelLogic.Services.Distributor;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.Products;
    using BrandexSalesAdapter.ExcelLogic.Services.Sales;
    using static Common.DataConstants.Ditributors;
    using Microsoft.AspNetCore.Authorization;

    public class BrandexController : HandleController
    {
        private IWebHostEnvironment hostEnvironment;

        // db Services
        private readonly ISalesService salesService;
        private readonly IProductsService productsService;
        private readonly IPharmaciesService pharmaciesService;
        private readonly IDistributorService distributorService;

        // user service


        // universal Services
        private readonly INumbersChecker numbersChecker;

        public BrandexController(
            IWebHostEnvironment hostEnvironment,
            ISalesService salesService,
            INumbersChecker numbersChecker,
            IProductsService productsService,
            IPharmaciesService pharmaciesService,
            IDistributorService distributorService
            )

        {

            this.hostEnvironment = hostEnvironment;
            this.salesService = salesService;
            this.numbersChecker = numbersChecker;
            this.productsService = productsService;
            this.pharmaciesService = pharmaciesService;
            this.distributorService = distributorService;
            
        }

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Import(BrandexInputModel brandexInput)
        {

            IFormFile file = Request.Form.Files[0];

            DateTime dateForDb = DateTime.ParseExact(brandexInput.Date, "dd-MM-yyyy", null);

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

                                    var currRowDate = DateTime.ParseExact(currentRow, "yyyy-MM", null);
                                    if (currentRow != null)
                                    {
                                        newSale.Date = currRowDate;
                                    }
                                    break;

                                case 3:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        var producId = await this.productsService.ProductIdByDistributor(currentRow, Brandex);

                                        if (producId!=0)
                                        {   
                                            newSale.ProductId = producId;
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

                                case 4:
                                    if (currentRow != "")
                                    {
                                        if (this.numbersChecker.NegativeNumberIncludedCheck(currentRow))
                                        {
                                            int countProduct = int.Parse(currentRow);
                                            newSale.Count = countProduct;
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

                                case 5:
                                    if (this.numbersChecker.WholeNumberCheck(currentRow))
                                    {
                                        int rowPharmacytId = int.Parse(currentRow);

                                        var pharmacyId = await this.pharmaciesService.PharmacyIdByDistributor(currentRow, Brandex);

                                        if (await this.pharmaciesService.CheckPharmacyByDistributor(currentRow, Brandex))
                                        {
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

                            }

                        }

                        await this.salesService.CreateSale(newSale, Brandex);
                    }

                }

            }

            var brandexOutputModel = new BrandexOutputModel {
                Date = brandexInput.Date,
                Errors = errorDictionary
            };

            return this.View(brandexOutputModel);

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(string pharmacyId, string productId, string date, int count)
        {
            if(await this.salesService.UploadIndividualSale(pharmacyId, productId, date, count, Brandex))
            {
                var saleOutputModel = new SaleOutputModel
                {
                    ProductName = await this.productsService.NameById(productId, Brandex),
                    PharmacyName = await this.pharmaciesService.NameById(pharmacyId, Brandex),
                    Count = count,
                    Date = date,
                    DistributorName = Brandex
                };
                return this.View(saleOutputModel);
            }

            return Redirect("Index");
        }
    }
}
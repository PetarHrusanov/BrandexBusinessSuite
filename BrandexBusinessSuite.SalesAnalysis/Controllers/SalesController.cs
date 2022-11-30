namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Services;
using Data.Enums;

using Infrastructure;
using Models.Pharmacies;
using Models.Products;
using Models.Sales;

using Services.Cities;
using Services.Distributor;
using Services.Pharmacies;
using Services.Products;
using Services.Regions;
using Services.Sales;
using Services.PharmacyChains;
using Services.PharmacyCompanies;

using static Methods.ExcelMethods;

using static Common.ErpConstants;
using static Common.ExcelDataConstants.Ditributors;
using static Common.ExcelDataConstants.ExcelLineErrors;
using static Common.Constants;
using static Requests.RequestsMethods;

public class SalesController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    
    private readonly ErpUserSettings _erpUserSettings;
    private static readonly HttpClient Client = new();

    private readonly ISalesService _salesService;
    private readonly IProductsService _productsService;
    private readonly IPharmaciesService _pharmaciesService;
    private readonly IDistributorService _distributorService;
    private readonly ICitiesService _citiesService;
    private readonly IPharmacyChainsService _pharmacyChainsService;
    private readonly IPharmacyCompaniesService _pharmacyCompaniesService;
    private readonly IRegionsService _regionsService;

    private const int BrandexDateColumn = 0;
    private const int BrandexProductIdColumn = 1;
    private const int BrandexCountColumn = 2;
    private const int BrandexPharmacyIdColumn = 3;

    private const int ProductCounter = 4;

    public SalesController(IWebHostEnvironment hostEnvironment,
        IOptions<ErpUserSettings> erpUserSettings,
        ISalesService salesService,
        IProductsService productsService, IPharmaciesService pharmaciesService, IDistributorService distributorService,
        ICitiesService citiesService, IPharmacyChainsService pharmacyChainsService, IPharmacyCompaniesService pharmacyCompaniesService,
        IRegionsService regionsService
        )

    {
        _hostEnvironment = hostEnvironment;
        _salesService = salesService;
        _productsService = productsService;
        _pharmaciesService = pharmaciesService;
        _distributorService = distributorService;
        _citiesService = citiesService;
        _erpUserSettings = erpUserSettings.Value;
        _pharmacyChainsService = pharmacyChainsService;
        _pharmacyCompaniesService = pharmacyCompaniesService;
        _regionsService = regionsService;
    }

    [HttpGet]
    public async Task<List<BasicCheckModel>> GetDistributors()
        => await _distributorService.GetDistributors();

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Generate([FromBody] SalesRegionDateInputModel inputModel)
    {

        DateTime.TryParseExact(inputModel.DateBegin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out var dateBegin);

        if (!DateTime.TryParseExact(inputModel.DateEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateEnd))
        {
            dateEnd = DateTime.MaxValue;
        }

        var regionId = inputModel.RegionId;

        var sWebRootFolder = _hostEnvironment.WebRootPath;
        const string sFileName = @"Sales.xlsx";

        var memory = new MemoryStream();

        await using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            var excelSheet = workbook.CreateSheet("sales");
            var row = excelSheet.CreateRow(0);

            var products = await _productsService.GetProductsIdPrices();

            foreach (var product in products)
            {
                var sumCount = await _salesService.ProductCountSumByIdDate(product.Id, dateBegin, dateEnd, regionId);
                row.CreateCell(ProductCounter + row.Cells.Count()).SetCellValue(sumCount);
            }

            row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

            foreach (var product in products)
            {
                var sumCount = await _salesService.ProductCountSumByIdDate(product.Id, dateBegin, dateEnd, regionId);
                var productRevenue = sumCount * product.Price;
                row.CreateCell(row.Cells.Count() + ProductCounter).SetCellValue(productRevenue);
            }

            row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

            await CreateHeaderColumnsAsync(row);

            var collectionPharmacies = await _pharmaciesService.GetPharmaciesExcelModel(dateBegin, dateEnd, regionId);

            switch (inputModel.Summed)
            {
                case "Summed":
                    CreatePharmacySalesRow(excelSheet, collectionPharmacies, products, null);
                    break;

                case "Separated":
                    row.CreateCell(row.Cells.Count()).SetCellValue("Date");
                    var dates = await _salesService.GetDistinctDatesByMonths();
                    foreach (var currentDate in dates)
                    {
                        CreatePharmacySalesRow(excelSheet, collectionPharmacies, products, currentDate);
                    }
                    break;
            }

            workbook.Write(fs);
        }

        await using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }

        memory.Position = 0;

        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Import([FromForm] SalesBulkInputModel salesBulkInput)
    {
        var dateFromClient = salesBulkInput.Date;
        var dateForDb = DateTime.ParseExact(dateFromClient, "dd-MM-yyyy", null);

        var distributorName = salesBulkInput.Distributor;

        var file = salesBulkInput.ImageFile;

        var errorDictionary = new Dictionary<int, string>();

        var validSalesList = new List<SaleInputModel>();

        var pharmacyIdsForCheck = await _pharmaciesService.GetPharmaciesCheck();
        var productIdsForCheck = await _productsService.GetProductsCheck();

        if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);
        
        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var newSale = new SaleInputModel
            {
                Date = dateForDb, DistributorId = await _distributorService.IdByName(salesBulkInput.Distributor)
            };

            switch (distributorName)
            {
                case Brandex:
                    CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale, errorDictionary,
                        Brandex, BrandexDateColumn, BrandexProductIdColumn, BrandexPharmacyIdColumn,
                        BrandexCountColumn);
                    break;
                case Phoenix:
                    CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale, errorDictionary,
                        Phoenix, 16, 0, 2, 14);
                    break;
                case Sting:
                    CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale, errorDictionary,
                        Sting, 0, 1, 6, 11);
                    break;
                case Pharmnet:
                    CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale, errorDictionary,
                        Pharmnet, 9, 2, 4, 11);
                    break;
                case Sopharma:
                    CreateSaleInputModel(productIdsForCheck, pharmacyIdsForCheck, row, i, newSale, errorDictionary,
                        Sopharma, 0, 2, 5, 11);
                    break;
            }

            if (newSale.PharmacyId != 0 && newSale.ProductId != 0 && newSale.Count != 0 && newSale.DistributorId != 0)
            {
                validSalesList.Add(newSale);
            }
        }

        if (errorDictionary.Count == 0) await _salesService.UploadBulk(validSalesList); 
        
        var outputModel = new SalesBulkOutputModel 
        {
            Date = dateFromClient,
            Errors = errorDictionary,
            ErrorsArray = errorDictionary.Select(entry => entry.Key - 2).ToArray()
        };

        return JsonConvert.SerializeObject(outputModel);
    }

    public int ResolveProductId(string inputProductId, string inputDistributor, List<ProductCheckModel> productsToCheck)
    {
        if (inputDistributor == Sopharma) return productsToCheck.Where(p => p.SopharmaId == inputProductId)
            .Select(p => p.Id).FirstOrDefault();
        
        if (!int.TryParse(inputProductId, out var productId)) return productId;

        return inputDistributor switch
        {
            Brandex => productsToCheck.Where(p => p.BrandexId == productId).Select(p => p.Id).FirstOrDefault(),
            Phoenix => productsToCheck.Where(p => p.PhoenixId == productId).Select(p => p.Id).FirstOrDefault(),
            Sting => productsToCheck.Where(p => p.StingId == productId).Select(p => p.Id).FirstOrDefault(),
            Pharmnet => productsToCheck.Where(p => p.PharmnetId == productId).Select(p => p.Id).FirstOrDefault(),
            _ => 0
        };
    }

    public int ResolvePharmacyId(string inputPharmacyId, string inputDistributor,
        List<PharmacyCheckModel> pharmaciesToCheck)
    {
        if (!int.TryParse(inputPharmacyId, out var pharmacyId)) return pharmacyId;

        return inputDistributor switch
        {
            Brandex => pharmaciesToCheck.Where(p => p.BrandexId == pharmacyId).Select(p => p.Id).FirstOrDefault(),
            Phoenix => pharmaciesToCheck.Where(p => p.PhoenixId == pharmacyId).Select(p => p.Id).FirstOrDefault(),
            Sting => pharmaciesToCheck.Where(p => p.StingId == pharmacyId).Select(p => p.Id).FirstOrDefault(),
            Pharmnet => pharmaciesToCheck.Where(p => p.PharmnetId == pharmacyId).Select(p => p.Id).FirstOrDefault(),
            Sopharma => pharmaciesToCheck.Where(p => p.SopharmaId == pharmacyId).Select(p => p.Id).FirstOrDefault(),
            _ => 0
        };
    }

    private void CreateSaleInputModel(List<ProductCheckModel> productIdsForCheck,
        List<PharmacyCheckModel> pharmacyIdsForCheck, IRow row, int i, SaleInputModel newSale,
        IDictionary<int, string> errorDictionary, string distributor, int dateColumn, int productIdColumn,
        int pharmacyIdColumn, int saleCountColumn)
    {
        var dateRow = row.GetCell(dateColumn);

        if (dateRow.CellType == CellType.Numeric) newSale.Date = DateTime.FromOADate(dateRow.NumericCellValue);

        var productRow = ResolveProductId(row.GetCell(productIdColumn).ToString()?.TrimEnd(), distributor,
            productIdsForCheck);
        var pharmacyIdRow = ResolvePharmacyId(row.GetCell(pharmacyIdColumn).ToString()?.TrimEnd(), distributor,
            pharmacyIdsForCheck);

        if (productRow == 0)
        {
            errorDictionary[i + 1] = IncorrectProductId;
            return;
        }

        if (pharmacyIdRow == 0)
        {
            errorDictionary[i + 1] = IncorrectPharmacyId;
            return;
        }

        var saleCountRow = row.GetCell(saleCountColumn);

        if (saleCountRow == null || !int.TryParse(saleCountRow.ToString()?.TrimEnd(), out var saleCountInt))
        {
            errorDictionary[i + 1] = IncorrectSalesCount;
            return;
        }

        newSale.ProductId = productRow;
        newSale.PharmacyId = pharmacyIdRow;
        newSale.Count = saleCountInt;
    }

    private async Task CreateHeaderColumnsAsync(IRow row)
    {
        var products = await _productsService.GetProductsNames();
        row.CreateCell(0).SetCellValue("Pharmacy Name");
        row.CreateCell(1).SetCellValue("Pharmacy Address");
        row.CreateCell(2).SetCellValue("Pharmacy Class");
        row.CreateCell(3).SetCellValue("Region");

        foreach (var product in products)
        {
            row.CreateCell(row.Cells.Count()).SetCellValue(product);
        }

        row.CreateCell(row.Cells.Count()).SetCellValue("SumSale");
    }

    private static void CreatePharmacySalesRow(ISheet excelSheet, IEnumerable<PharmacyExcelModel> collectionPharmacies,
        IList<ProductShortOutputModel> products, DateTime? currentDate)
    {
        foreach (var pharmacy in collectionPharmacies)
        {
            var row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);

            row.CreateCell(row.Cells.Count).SetCellValue(pharmacy.Name);
            row.CreateCell(row.Cells.Count).SetCellValue(pharmacy.Address);
            row.CreateCell(row.Cells.Count).SetCellValue(pharmacy.PharmacyClass.ToString());
            row.CreateCell(row.Cells.Count).SetCellValue(pharmacy.Region);

            if (currentDate != null)
            {
                foreach (var product in products)
                {
                    var sumCount = pharmacy.Sales.Where(i => i.ProductId == product.Id && i.Date == currentDate)
                        .Sum(b => b.Count);
                    row.CreateCell(row.Cells.Count()).SetCellValue(sumCount);
                }

                row.CreateCell(row.Cells.Count())
                    .SetCellValue(pharmacy.Sales.Select(p => p.ProductPrice * p.Count).Sum());

                row.CreateCell(row.Cells.Count()).SetCellValue(currentDate.ToString());
            }
            else
            {
                foreach (var product in products)
                {
                    var sumCount = pharmacy.Sales.Where(i => i.ProductId == product.Id).Sum(b => b.Count);
                    row.CreateCell(row.Cells.Count()).SetCellValue(sumCount);
                }

                row.CreateCell(row.Cells.Count())
                    .SetCellValue(pharmacy.Sales.Select(p => p.ProductPrice * p.Count).Sum());
            }
        }
    }
}
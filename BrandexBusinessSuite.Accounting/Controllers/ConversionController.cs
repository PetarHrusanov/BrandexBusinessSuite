namespace BrandexBusinessSuite.Accounting.Controllers;

using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Accounting.Data.Models;
using BrandexBusinessSuite.Models.ErpDocuments;
using Data;
using Services;
using Infrastructure;
using Models;

using static  Common.Constants;
using static Common.ErpConstants;
using static Common.MarketingDataConstants;

using static Methods.ExcelMethods;
using static Requests.RequestsMethods;

public class ConversionController : ApiController
{

    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ErpUserSettings _userSettings;

    private static readonly HttpClient Client = new();

    private readonly AccountingDbContext _context;

    private static readonly Regex RegexDate = new(@"([0-9]{4}-[0-9]{2}-[0-9]{2})");
    private static readonly Regex PriceRegex = new(@"[0-9]+[.,][0-9]*\s*€$");

    private static readonly Regex FacebookInvoiceRegex = new(@"FBADS-[0-9]{3}-[0-9]{9}");

    public ConversionController(IWebHostEnvironment hostEnvironment, IOptions<ErpUserSettings> userSettings,
        AccountingDbContext context)
    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> ConvertFacebookPdfForAccounting([FromForm] IFormFile file)
    {
        var newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) return BadRequest();

        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        var rawText = PdfText(fullPath);

        var dateString = RegexDate.Matches(file.FileName)[0].Value;
        var date = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var products = await _context.Products.ToListAsync();
        var productsPrices = await ProductPriceDictionaryFromText(rawText, products);

        var productsWithPrices = products.Where(product => productsPrices.ContainsKey(product.FacebookName));
        var productCodesPrices = productsWithPrices
            .GroupBy(product => product.AccountingName)
            .ToDictionary(
                group => group.Key,
                group => new AccountingModel(
                    group.Key,
                    group.First().AccountingErpNumber,
                    Math.Round(group.Sum(product => productsPrices[product.FacebookName]), 2)
                )
            );

        var facebookInvoiceNumber = FacebookInvoiceRegex.Matches(rawText)[0].ToString();

        var primaryDocument = new LogisticsProcurementReceivingOrder(facebookInvoiceNumber, date);

        var erpLines =
            from product in productCodesPrices
            select new ErpOrderLinesAccounting(
                "General_Products_Products(ee6e5c65-6dc7-41d7-9d57-ba87b19aa56c)",
                product.Value.Price,
                1,
                "Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)");

        primaryDocument.Lines.AddRange(erpLines);

        var jsonPostString = JsonConvert.SerializeObject(primaryDocument, Formatting.Indented);

        AuthenticateUserBasicHeader(Client, _userSettings.User, _userSettings.Password);
        var responseContentJObj = await JObjectByUriPostRequest(Client, $"{ErpRequests.BaseUrl}Logistics_Procurement_ReceivingOrders/", jsonPostString);
        
        var primaryDocumentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();
        await ChangeStateToRelease(Client, primaryDocumentId);

        responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}Logistics_Procurement_PurchaseInvoices?$top=2&$filter=DocumentNo%20eq%20'{primaryDocument.InvoiceDocumentNo}'%20and%20Void%20eq%20false&$select=Id,Lines&$expand=Lines($expand=PurchaseInvoice($select=Id),QuantityUnit($select=Id),ReceivingOrderLine($select=Id);$select=CustomProperty_%D0%92%D0%A0%D0%9C_u002D%D0%BF%D0%BE%D0%BA%D1%83%D0%BF%D0%BA%D0%B8,CustomProperty_%D0%9F%D1%80%D0%BE%D0%B4%D1%83%D0%BA%D1%82_u002D%D0%BF%D0%BE%D0%BA%D1%83%D0%BF%D0%BA%D0%B8,Id,LineAmount,LineNo,Product,ProductName,Quantity,QuantityBase,QuantityUnit,StandardQuantityBase,UnitPrice)");

        var invoiceId = responseContentJObj[ErpDocuments.ValueLower]?[0]?[ErpDocuments.ODataId]!.ToString();
        var invoiceLines = responseContentJObj[ErpDocuments.ValueLower]?[0]?["Lines"]?.ToObject<List<ErpInvoiceLinesAccounting>>();

        foreach (var line in invoiceLines!)
        {
            var unitPrice = line.UnitPrice.Value;

            var (_, value) = productCodesPrices.FirstOrDefault(k => k.Value.Price == unitPrice);

            line.CustomProperty_Продукт_u002Dпокупки = new ErpCharacteristicValueDescriptionBg(
                value.AccountingErpNumber, new ErpCharacteristicValueDescriptionBg._Description(value.AccountingName));
            line.CustomProperty_ВРМ_u002Dпокупки = new ErpCharacteristicValueDescriptionBg("83",
                new ErpCharacteristicValueDescriptionBg._Description("Фейсбук"));

            var uri = new Uri($"{ErpRequests.BaseUrl}Logistics_Procurement_PurchaseInvoiceLines({line.Id})");
            jsonPostString = JsonConvert.SerializeObject(line, Formatting.Indented);
            var content = new StringContent(jsonPostString, Encoding.UTF8, RequestConstants.ApplicationJson);

            await Client.PutAsync(uri, content);
        }

        await ChangeStateToRelease(Client, invoiceId!);

        return Result.Success;
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> ConvertFacebookPdfForMarketing([FromForm] IFormFile file)
    {
        var dateString = RegexDate.Matches(file.FileName)[0];
        var date = DateTime.ParseExact(dateString.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) return BadRequest();
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        var rawText = PdfText(fullPath);

        var products = await _context.Products.ToListAsync();

        var productsPrices = await ProductPriceDictionaryFromText(rawText, products);
        var facebookActivity =
            await _context.MarketingActivityDetails.Where(c => c.Name == Facebook).FirstOrDefaultAsync();

        AuthenticateUserBasicHeader(Client, _userSettings.User, _userSettings.Password);
        foreach (var (key, value) in productsPrices)
        {
            await PostMarketingActivitiesToErp(facebookActivity!, key, Math.Round((double)value, 2), date);
        }

        return Result.Success;
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> ConvertGoogleForMarketing([FromForm] IFormFile file)
    {

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        var products = await _context.Products.ToListAsync();

        var googleActivity = await _context.MarketingActivityDetails.Where(c => c.Name == Google).FirstOrDefaultAsync();
        
        AuthenticateUserBasicHeader(Client, _userSettings.User, _userSettings.Password);
        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var (product, price) = GetProductAndPriceFromGoogleRow(row, products);
            if (product==null || price==null ) continue;

            var dateRow = row.GetCell(0).ToString()?.TrimEnd();
            var date = DateTime.ParseExact(dateRow!, "MMM d, yyyy", CultureInfo.InvariantCulture);

            await PostMarketingActivitiesToErp(googleActivity!, product.GoogleName, (double)price, date);
        }

        return Result.Success;
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> ConvertGoogleForAccounting([FromForm] IFormFile file)
    {

        var fullPath = CreateFileDirectories.CreateExcelFilesInputCompletePath(_hostEnvironment, file);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        stream.Position = 0;

        if (!CheckXlsx(file)) return BadRequest(Errors.IncorrectFileFormat);

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        var products = await _context.Products.ToListAsync();
        var productPriceDictionary = new Dictionary<string, double>();

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var (product, price) = GetProductAndPriceFromGoogleRow(row, products);
            if (product==null || price==null ) continue;

            if (!productPriceDictionary.ContainsKey(product.AccountingName)) productPriceDictionary.Add(product.AccountingName, 0);

            productPriceDictionary[product.AccountingName] += (double)price;
        }

        const string sFileName = @"Google.xlsx";

        var memory = new MemoryStream();

        var sWebRootFolder = _hostEnvironment.WebRootPath;

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            var excelSheet = workbook.CreateSheet("Google");
            var row = excelSheet.CreateRow(0);

            foreach (var (key, value) in productPriceDictionary)
            {
                row.CreateCell(row.Cells.Count()).SetCellValue(key);
                row.CreateCell(row.Cells.Count()).SetCellValue(value);
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
            }

            workbook.Write(fs);
        }

        await using (var streamOutput = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {
            await streamOutput.CopyToAsync(memory);
        }

        memory.Position = 0;
        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
    }

    private static string PdfText(string path)
    {
        var reader = new PdfReader(path);
        var text = string.Empty;
        for (var page = 1; page <= reader.NumberOfPages; page++)
        {
            text += PdfTextExtractor.GetTextFromPage(reader, page);
            text += Environment.NewLine;
        }

        reader.Close();
        return text;
    }

    private static (Product? product, double? price) GetProductAndPriceFromGoogleRow(IRow row, List<Product> products)
    {

        var productRow = row.GetCell(2)?.ToString()?.TrimEnd();
        var product = products.FirstOrDefault(e => productRow?.Contains(e.GoogleName, StringComparison.CurrentCultureIgnoreCase) ?? false);

        var priceRow = row.GetCell(4)?.ToString()?.TrimEnd();
        var price = priceRow != null && double.TryParse(PriceRegex.Match(priceRow).Value, out var p) ? p : (double?)null;

        return (product, price);
    }

    private async Task<Dictionary<string, decimal>> ProductPriceDictionaryFromText(string rawText, List<Product> products)
    {
        var rawTextSplit = rawText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        var euro = await _context.Currencies.Where(c => c.Name == Euro).Select(c => c.Value).FirstOrDefaultAsync();
        var numberFormat = new NumberFormatInfo { NumberDecimalSeparator = "," };
    
        var productsPrices = rawTextSplit
            .Where(x => products.Any(y => x.Contains(y.FacebookName)))
            .Select(line =>
            {
                var productName = products.First(x => line.Contains(x.FacebookName)).FacebookName;
                var priceMatches = PriceRegex.Matches(line);
            
                if (priceMatches.Count == 0) return null;
            
                var priceMatch = priceMatches[priceMatches.Count - 1]; // Take the last match
                var priceString = priceMatch.Value.Replace("€", "").Trim(); // Remove '€' symbol and any trailing or leading white spaces
            
                if (!decimal.TryParse(priceString, NumberStyles.Number, numberFormat, out var price))
                {
                    // Handle the error
                    return null;
                }
            
                return new { ProductName = productName, Price = price };
            })
            .Where(x => x != null)
            .GroupBy(x => x.ProductName)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(x => x.Price * (decimal)euro)
            );
    
        return productsPrices;
    }

    


    private async Task PostMarketingActivitiesToErp(MarketingActivityDetails activity, string product, double price,
        DateTime date)
    {
        if (product == "General Audience") product = "Botanic";

        var activityObject = new MarketingActivityCm(activity.Subject, date, activity.PartyId,
            activity.Measure, activity.Type, activity.Media, activity.Type, price, product);

        var jsonPostString = JsonConvert.SerializeObject(activityObject, Formatting.Indented);
        var responseContentJObj = await JObjectByUriPostRequest(Client, $"{ErpRequests.BaseUrl}General_Contacts_Activities/", jsonPostString);
        var documentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();
        await ChangeStateToRelease(Client, documentId);
    }
    
}
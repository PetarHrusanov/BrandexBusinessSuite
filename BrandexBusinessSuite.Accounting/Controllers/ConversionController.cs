namespace BrandexBusinessSuite.Accounting.Controllers;

using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using BrandexBusinessSuite.Controllers;
using Services;
using BrandexBusinessSuite.Models.ErpDocuments;
using Infrastructure;
using Models;

using static Common.ProductConstants;
using static  Common.Constants;
using static Common.ErpConstants;

using static Methods.ExcelMethods;
using static Requests.RequestsMethods;
using static Methods.FieldsValuesMethods;

public class ConversionController : ApiController
{

    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ErpUserSettings _userSettings;

    private static readonly HttpClient Client = new();
    
    private const string FacebookEng = "Facebook";
    private const string FacebookBgCapital = "Фейсбук";
    private const string FacebookBgLower = "фейсбук";

    private const string Google = "Google";
    private const string GoogleAdWordsLower = "google adwords";
    private const string GoogleAdWordsCapital = "Ad Words";

    private const string Click = "клик";
    private const string Impressions = "впечатления";

    private const double EuroRate = 1.9894;
    
    private static readonly Regex RegexDate = new(@"([0-9]{4}-[0-9]{2}-[0-9]{2})");
    private static readonly Regex PriceRegex = new (@"[0-9]+[.,][0-9]*");
    private static readonly Regex FacebookInvoiceRegex = new (@"FBADS-[0-9]{3}-[0-9]{9}");

    public ConversionController(IWebHostEnvironment hostEnvironment, IOptions<ErpUserSettings> userSettings)
    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
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

        var dateString = RegexDate.Matches(file.FileName)[0];
        var date = DateTime.ParseExact(dateString.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var productsPrices = ProductPriceDictionaryFromText(rawText);

        var productNames = typeof(Facebook).GetFields().Select(field => field.Name).ToList();

        var productCodesPrices = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var product in productNames)
        {
            var valueFacebook = ReturnValueByClassAndName(typeof(Facebook), product);
            var valueErp = ReturnValueByClassAndName(typeof(ERP_Accounting), product);
            var valueErpCode = ReturnValueByClassAndName(typeof(ErpCodesNumber), product);

            if (!productsPrices.ContainsKey(valueFacebook)) continue;

            var priceDouble = productsPrices[valueFacebook];
            var price = (double)priceDouble * EuroRate;
            var priceRounded = Math.Round(price, 2);

            RenameKey(productsPrices, valueFacebook, valueErp);

            productCodesPrices.Add(valueErp, new Dictionary<string, decimal>() { { valueErpCode, (decimal)priceRounded } });
        }

        var facebookInvoiceNumber = FacebookInvoiceRegex.Matches(rawText)[0].ToString();
        var primaryDocument = new LogisticsProcurementReceivingOrder(facebookInvoiceNumber, date);

        foreach (var erpLine in productsPrices.Select(product => (double)product.Value * EuroRate)
                     .Select(price => Math.Round(price, 2))
                     .Select(priceRounded => new ErpOrderLinesAccounting(
                         new ErpCharacteristicId("General_Products_Products(ee6e5c65-6dc7-41d7-9d57-ba87b19aa56c)"),
                         new ErpCharacteristicLineAmount((decimal)priceRounded), new ErpCharacteristicValueNumber(1),
                         new ErpCharacteristicId("Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)"))))
        {
            primaryDocument.Lines.Add(erpLine);
        }

        var jsonPostString = JsonConvert.SerializeObject(primaryDocument, Formatting.Indented);

        var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.User}:{_userSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await JObjectByUriPostRequest(Client, $"{ErpRequests.BaseUrl}Logistics_Procurement_ReceivingOrders/", jsonPostString);

        var primaryDocumentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();

        await ChangeStateToRelease(Client, primaryDocumentId);

        responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}Logistics_Procurement_PurchaseInvoices?$filter=equalnull(DocumentNo,'{primaryDocument.InvoiceDocumentNo}')%20and%20Void%20eq%20false");

        var invoice = responseContentJObj[ErpDocuments.ValueLower];
        var invoiceId = Convert.ToString(invoice![0]![ErpDocuments.ODataId]);

        responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}Logistics_Procurement_PurchaseInvoiceLines?$filter=PurchaseInvoice%20eq%20'{invoiceId}'");

        var invoiceLinesString = Convert.ToString(responseContentJObj[ErpDocuments.ValueLower]);
        var invoiceLines = JsonConvert.DeserializeObject<List<ErpInvoiceLinesAccounting>>(invoiceLinesString!);

        foreach (var line in invoiceLines!)
        {
            var unitPrice = line.UnitPrice.Value;

            var productName = productCodesPrices.Where(k => k.Value.Any(s => s.Value == unitPrice))
                .Select(k => k.Key)
                .FirstOrDefault();

            var productCode = productCodesPrices[productName!].Keys.FirstOrDefault();

            line.CustomProperty_Продукт_u002Dпокупки = new ErpCharacteristicValueDescriptionBg(productCode, new ErpCharacteristicValueDescriptionBg._Description(productName));
            line.CustomProperty_ВРМ_u002Dпокупки = new ErpCharacteristicValueDescriptionBg("83", new ErpCharacteristicValueDescriptionBg._Description(FacebookBgCapital));

            var uri = new Uri($"{ErpRequests.BaseUrl}Logistics_Procurement_PurchaseInvoiceLines({line.Id})");
            jsonPostString = JsonConvert.SerializeObject(line, Formatting.Indented);
            var content = new StringContent(jsonPostString, Encoding.UTF8, RequestConstants.ApplicationJson);

            await Client.PutAsync(uri, content);
        }
        
        await ChangeStateToRelease(Client, invoiceId);

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

        var productsPrices = ProductPriceDictionaryFromText(rawText);

        foreach (var (key, value) in productsPrices)
        {
            var price = (double)value * EuroRate;
            var priceRounded = Math.Round(price, 2);

            await PostMarketingActivitiesToErp(FacebookEng, key, priceRounded, date);
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

        var fieldsValues = new List<string>();
        var fieldsGoogle = typeof(GoogleMarketing).GetFields(BindingFlags.Public | BindingFlags.Static);
        fieldsValues.AddRange(fieldsGoogle.Select(field => (string)field.GetValue(null)!));

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);

            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var productRow = row.GetCell(2);
            if (productRow == null) continue;

            var product = productRow.ToString()?.TrimEnd();
            if (string.IsNullOrEmpty(product)) continue;
            if (!fieldsValues.Contains(product)) continue;

            foreach (var field in typeof(GoogleMarketing).GetFields())
            {
                if ((string)field.GetValue(null)! != product) continue;
                var fieldName = field.Name;
                var fieldErp =
                    typeof(GoogleMarketingErp).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                product = (string)fieldErp!.GetValue(null)!;
            }

            var priceRow = row.GetCell(4).ToString()?.TrimEnd();
            var priceString = PriceRegex.Matches(priceRow)[0].ToString();
            var price = double.Parse(priceString);

            var dateRow = row.GetCell(0).ToString().TrimEnd();
            var date = DateTime.ParseExact(dateRow, "MMM d, yyyy", CultureInfo.InvariantCulture);

            await PostMarketingActivitiesToErp(Google, product, price, date);
        }

        return Result.Success;
    }

    private static string PdfText(string path)
    {
        var reader = new PdfReader(path);
        var text = string.Empty;
        for(var page = 1; page <= reader.NumberOfPages; page++)
        {
            text += PdfTextExtractor.GetTextFromPage(reader,page);
            text += Environment.NewLine;
        }
        reader.Close();
        return text;
    }

    private static Dictionary<string, decimal> ProductPriceDictionaryFromText(string rawText)
    {
        var productsPrices = new Dictionary<string, decimal>();

        var rawTextSplit = rawText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

        var fieldsFacebook = typeof(Facebook).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var productField in fieldsFacebook)
        {
            var product = (string)productField.GetValue(null)!;

            var lines = rawTextSplit.Where(element => element.Contains(product)).ToList();

            if (lines.Count==0) continue;
            
            foreach (var line in lines)
            {
                if (!productsPrices.ContainsKey(product))
                {
                    productsPrices.Add(product,0);
                }

                var priceString = PriceRegex.Matches(line)[0].ToString();
                var price = decimal.Parse(priceString, new NumberFormatInfo { NumberDecimalSeparator = "," });
                
                productsPrices[product] += price;
            }
        }

        return productsPrices;

    }

    private static void RenameKey<TKey, TValue>(IDictionary<TKey, TValue> dic,
        TKey fromKey, TKey toKey)
    {
        var value = dic[fromKey];
        dic.Remove(fromKey);
        dic[toKey] = value;
    }

    private async Task PostMarketingActivitiesToErp(string digital, string product, double price, DateTime date)
    {

        var monthErp = ReturnValueByClassAndName(typeof(ErpMonths), date.ToString("MMMM"));

        var yearErp = date.ToString("yyyy");

        if (product == "General Audience") product = "Botanic"; 
        
        var activityObject = new MarketingActivityCm();

        switch (digital)
        {
            case FacebookEng:
                activityObject = new MarketingActivityCm(
                    "Задача / FACEBOOK IRELAND LIMITED", 
                    date, 
                    "b21c6bc3-a4d8-43b9-a3df-b2d39ddf552f", 
                    monthErp, 
                    yearErp,
                    Impressions,
                    FacebookBgCapital,
                    FacebookBgLower,
                    FacebookEng, 
                    price, 
                    product
                );
                break;
            case Google:
                activityObject = new MarketingActivityCm(
                    "Задача / GOOGLE IRELAND LIMITED", 
                    date, 
                    "e5a6cfc4-d407-4424-a22e-d479136a28aa", 
                    monthErp, 
                    yearErp,
                    Click,
                    GoogleAdWordsLower,
                    Google,
                    GoogleAdWordsCapital, 
                    price, 
                    product
                );
                break;
        }

        var jsonPostString = JsonConvert.SerializeObject(activityObject, Formatting.Indented);

        var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.User}:{_userSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await  JObjectByUriPostRequest(Client, $"{ErpRequests.BaseUrl}General_Contacts_Activities/", jsonPostString);

        var documentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();

        await ChangeStateToRelease(Client, documentId);

    }
}
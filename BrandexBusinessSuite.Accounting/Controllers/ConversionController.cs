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
using Microsoft.AspNetCore.Cors;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.ErpDocuments;
using Infrastructure;
using Models;
using Requests;

using static Common.ProductConstants;
using static  Common.Constants;
using static Common.ErpConstants;

using static BrandexBusinessSuite.Requests.RequestsMethods;
using static Methods.FieldsValuesMethods;

public class ConversionController : ApiController
{

    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ErpUserSettings _userSettings;
    private readonly ApiSettings _apiSettings;
    
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

    public ConversionController(IWebHostEnvironment hostEnvironment,
        IOptions<ErpUserSettings> userSettings,
        IOptions<ApiSettings> apiSettings
        )
    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _apiSettings = apiSettings.Value;
    }

    [HttpPost]
    [EnableCors()]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertFacebookPdfForAccounting([FromForm] IFormFile file)
    {
        
        var newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) throw new ArgumentNullException();
        
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        var rawText = PdfText(fullPath);
        
        var dateString = RegexDate.Matches(file.FileName)[0];
        var date = DateTime.ParseExact(dateString.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        
        var productsPrices = ProductPriceDictionaryFromText(rawText);

        var sWebRootFolder = _hostEnvironment.WebRootPath;
        const string sFileName = @"Facebook_Accounting.xlsx";

        var memory = new MemoryStream();

        var productNames = typeof(Facebook).GetFields()
            .Select(field => field.Name)
            .ToList();

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

            productCodesPrices.Add(valueErp, new Dictionary<string, decimal>()
            {
                {  valueErpCode,(decimal)priceRounded }
            });
        }
        

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            var excelSheet = workbook.CreateSheet("Products Summed");

            var row = excelSheet.CreateRow(0);

            var facebookInvoiceNumber = FacebookInvoiceRegex.Matches(rawText)[0].ToString();

            var primaryDocument = new LogisticsProcurementReceivingOrder(facebookInvoiceNumber, date);

            foreach (var (key, value) in productsPrices)
            {
                row = excelSheet.CreateRow(excelSheet.LastRowNum+1);
                row.CreateCell(row.Cells.Count()).SetCellValue(key);
                row.CreateCell(row.Cells.Count()).SetCellValue((double)value);
                row.CreateCell(row.Cells.Count()).SetCellValue((double)value*EuroRate);
                
                var price = (double)value * EuroRate;
                var priceRounded = Math.Round(price, 2);

                var erpLine = new ErpOrderLinesAccounting(
                    new ErpCharacteristicId("General_Products_Products(ee6e5c65-6dc7-41d7-9d57-ba87b19aa56c)"),
                    new ErpCharacteristicLineAmount((decimal)priceRounded),
                    new ErpCharacteristicValueNumber(1),
                    new ErpCharacteristicId("Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)")
                );
                
                primaryDocument.Lines.Add(erpLine);

            }
            
            var jsonPostString = JsonConvert.SerializeObject(primaryDocument, Formatting.Indented);

            var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.User}:{_userSettings.Password}");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var responseContentJObj = await 
                JObjectByUriPostRequest(Client, _apiSettings.LogisticsProcurementReceivingOrders, jsonPostString);

            var primaryDocumentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();
            
            await ChangeStateToRelease(Client, primaryDocumentId);

            responseContentJObj =
                await JObjectByUriGetRequest(Client,
                    $"{_apiSettings.GeneralRequest}Logistics_Procurement_PurchaseInvoices?$filter=equalnull(DocumentNo,'{primaryDocument.InvoiceDocumentNo}')%20and%20Void%20eq%20false");
            
            var invoice = responseContentJObj[ErpDocuments.ValueLower];
            var invoiceId = Convert.ToString(invoice![0]![ErpDocuments.ODataId]);

            responseContentJObj =
                await JObjectByUriGetRequest(Client,
                    $"{_apiSettings.GeneralRequest}Logistics_Procurement_PurchaseInvoiceLines?$filter=PurchaseInvoice%20eq%20'{invoiceId}'");
            
            
            var invoiceLinesString = Convert.ToString(responseContentJObj[ErpDocuments.ValueLower]);
            var invoiceLines = JsonConvert.DeserializeObject<List<ErpInvoiceLinesAccounting>>(invoiceLinesString!);
            
            foreach (var line in invoiceLines!)
            {
                var unitPrice = line.UnitPrice.Value;
                
                var productName = productCodesPrices
                    .Where(k => k.Value.Any(s => s.Value == unitPrice))
                    .Select(k => k.Key).FirstOrDefault();

                var productCode = productCodesPrices[productName!].Keys.FirstOrDefault();

                line.CustomProperty_Продукт_u002Dпокупки =
                    new ErpCharacteristicValueDescriptionBg(productCode, new ErpCharacteristicValueDescriptionBg._Description(productName));
                line.CustomProperty_ВРМ_u002Dпокупки =
                    new ErpCharacteristicValueDescriptionBg("83", new ErpCharacteristicValueDescriptionBg._Description("Фейсбук"));
                
                var uri = new Uri($"{_apiSettings.GeneralRequest}Logistics_Procurement_PurchaseInvoiceLines({line.Id})");
                jsonPostString = JsonConvert.SerializeObject(line, Formatting.Indented);
                var content = new StringContent(jsonPostString, Encoding.UTF8, RequestConstants.ApplicationJson);
                
                await Client.PutAsync(uri, content);
            }

            await ChangeStateToRelease(Client, invoiceId);
            
            workbook.Write(fs);

        }

        await using (var streamWrite = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {
            await streamWrite.CopyToAsync(memory);
        }

        memory.Position = 0;

        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        
    }
    
    [HttpPost]
    [EnableCors()]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertFacebookPdfForMarketing([FromForm] IFormFile file)
    {
        
        var dateString = RegexDate.Matches(file.FileName)[0];
        var date = DateTime.ParseExact(dateString.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var monthErp = ReturnValueByClassAndName(typeof(ErpMonths), date.ToString("MMMM"));
        var yearErp = date.ToString("yyyy");

        var newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) return BadRequest(Errors.IncorrectFileFormat);
        
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        var rawText = PdfText(fullPath);

        var productsPrices = ProductPriceDictionaryFromText(rawText);
        
        var sWebRootFolder = _hostEnvironment.WebRootPath;
        var sFileName = @"Facebook_Marketing.xlsx";

        var memory = new MemoryStream();

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            foreach (var (key, value) in productsPrices)
            {

                var price = (double)value * EuroRate;
                var priceRounded = Math.Round(price, 2);

                CreateErpMarketingXlsSheet(workbook,
                    key,
                    monthErp,
                    yearErp,
                    Impressions,
                    FacebookBgCapital,
                    FacebookBgLower,
                    FacebookEng,
                    priceRounded,
                    "",
                    key
                );

                await PostMarketingActivitiesToErp(FacebookEng, key, priceRounded, date);
                
            }
            
            workbook.Write(fs);

        }

        await using (var streamWrite = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {
            await streamWrite.CopyToAsync(memory);
        }

        memory.Position = 0;

        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertGoogleForMarketing([FromForm] IFormFile file)
    {
        var newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) return BadRequest();

        var sFileExtension = System.IO.Path.GetExtension(file.FileName)?.ToLower();
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

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


        var fieldsValues = new List<string>();
        var fieldsGoogle = typeof(GoogleMarketing).GetFields(BindingFlags.Public | BindingFlags.Static);
        fieldsValues.AddRange(fieldsGoogle.Select(field => (string)field.GetValue(null)!));

        var sWebRootFolder = _hostEnvironment.WebRootPath;
        var sFileName = @"Google_Marketing.xlsx";
        var memory = new MemoryStream();

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create,
                         FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

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
                    var fieldErp = typeof(GoogleMarketingErp).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                    product = (string)fieldErp!.GetValue(null)!;
                }

                var priceRow = row.GetCell(4).ToString()?.TrimEnd();
                var priceString = PriceRegex.Matches(priceRow)[0].ToString();
                var price = double.Parse(priceString);

                var dateRow = row.GetCell(0).ToString().TrimEnd();
                var date = DateTime.ParseExact(dateRow, "MMM d, yyyy", CultureInfo.InvariantCulture);

                var monthErp = ReturnValueByClassAndName(typeof(ErpMonths), date.ToString("MMMM"));

                var yearErp = date.ToString("yyyy");

                CreateErpMarketingXlsSheet(workbook,
                    product + date.ToString("dd-MM-yyyy"),
                    monthErp,
                    yearErp,
                    Click,
                    GoogleAdWordsLower,
                    Google,
                    GoogleAdWordsCapital,
                    price,
                    "",
                    product
                );

                await PostMarketingActivitiesToErp(Google, product, price, date);
            }

            workbook.Write(fs);
        }

        await using (var streamWrite = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {
            await streamWrite.CopyToAsync(memory);
        }

        memory.Position = 0;

        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);

    }

    private static string PdfText(string path)
    {
        var reader = new PdfReader(path);
        var text = string.Empty;
        for(int page = 1; page <= reader.NumberOfPages; page++)
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

            var lines = rawTextSplit
                .Where(element => element.Contains(product)).ToList();

            if (lines.Count==0) continue;
            
            foreach (var line in lines)
            {
                if (!productsPrices.ContainsKey(product))
                {
                    productsPrices.Add(product,0);
                }

                var priceString = PriceRegex.Matches(line)[0].ToString();

                decimal price = 1;

                var numberFormatWithComma = new NumberFormatInfo
                {
                    NumberDecimalSeparator = ","
                };
                price = decimal.Parse(priceString, numberFormatWithComma);
                
                productsPrices[product] += price;
            }
        }

        return productsPrices;

    }

    private static void RenameKey<TKey, TValue>(IDictionary<TKey, TValue> dic,
        TKey fromKey, TKey toKey)
    {
        TValue value = dic[fromKey];
        dic.Remove(fromKey);
        dic[toKey] = value;
    }

    private static void CreateErpMarketingXlsSheet(IWorkbook workbook,
        string sheetName,
        string month,
        string year,
        string measure,
        string type,
        string media,
        string publishType,
        double price,
        string trp,
        string product
        )
    {
        var excelSheet = workbook.CreateSheet(sheetName);
        var row = excelSheet.CreateRow(0);
        
        CreateErpMarketingXlsRow(excelSheet, "МЕСЕЦ", month);
        CreateErpMarketingXlsRow(excelSheet, "ГОДИНА", year);
        CreateErpMarketingXlsRow(excelSheet, "Размер", measure);
        CreateErpMarketingXlsRow(excelSheet, "тип реклама", type);
        CreateErpMarketingXlsRow(excelSheet, "Медиа", media);
        CreateErpMarketingXlsRow(excelSheet, "Издание", publishType);
        CreateErpMarketingXlsRow(excelSheet, "цена реклама", Math.Round(price, 2).ToString());
        CreateErpMarketingXlsRow(excelSheet, "ТРП", trp);
        CreateErpMarketingXlsRow(excelSheet, "ПРОДУКТ БРАНДЕКС", product);
    }

    private static void CreateErpMarketingXlsRow(ISheet excelSheet, string rowName, string rowValue)
    {
        var row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue(rowName);
        row.CreateCell(row.Cells.Count()).SetCellValue(rowValue);
    }

    private async Task PostMarketingActivitiesToErp(string digital, string product, double price, DateTime date)
    {
        
        var subject = string.Empty;
        var partyId = string.Empty;
        var measure = string.Empty;
        var type = string.Empty;
        var media = string.Empty;
        var publishType = string.Empty;

        var monthErp = ReturnValueByClassAndName(typeof(ErpMonths), date.ToString("MMMM"));

        var yearErp = date.ToString("yyyy");

        if (product == "General Audience")
        {
            product = "Botanic";
        }

        switch (digital)
        {
            case FacebookEng:
                subject = "Задача / FACEBOOK IRELAND LIMITED";
                partyId = "b21c6bc3-a4d8-43b9-a3df-b2d39ddf552f";
                measure = Impressions;
                type = FacebookBgCapital;
                media = FacebookBgLower;
                publishType = FacebookEng;
                break;
            case Google:
                subject = "Задача / GOOGLE IRELAND LIMITED";
                partyId = "e5a6cfc4-d407-4424-a22e-d479136a28aa";
                measure = Click;
                type = GoogleAdWordsLower;
                media = Google;
                publishType = GoogleAdWordsCapital;
                break;
        }

        var activityObject = new MarketingActivityCm(subject, date, partyId, monthErp, yearErp, measure, type, media,
            publishType, price, product);

        var jsonPostString = JsonConvert.SerializeObject(activityObject, Formatting.Indented);

        var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.User}:{_userSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await  JObjectByUriPostRequest(Client, _apiSettings.GeneralContactActivities, jsonPostString);

        var documentId = responseContentJObj[ErpDocuments.ODataId]!.ToString();

        await ChangeStateToRelease(Client, documentId);

    }
}
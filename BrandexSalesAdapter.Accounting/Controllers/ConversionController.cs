namespace BrandexSalesAdapter.Accounting.Controllers;

using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using BrandexSalesAdapter.Controllers;
using Infrastructure;
using Models;
using Requests;

using static Common.ProductConstants;
using static  Common.Constants;
using static Common.ErpConstants;


public class ConversionController : ApiController
{

    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserSettings _userSettings;
    private readonly ApiSettings _apiSettings;
    
    private static readonly HttpClient client = new HttpClient();
    
    private const string FacebookEng = "Facebook";
    private const string Impressions = "впечатления";
    private const string FacebookBgCapital = "Фейсбук";
    private const string FacebookBgLower = "фейсбук";

    private const double euroRate = 1.9894;
    
    private static readonly Regex regexDate = new(@"([0-9]{4}-[0-9]{2}-[0-9]{2})");
    private static readonly Regex priceRegex = new (@"[0-9]+[.,][0-9]*");

    public ConversionController(IWebHostEnvironment hostEnvironment,
        IOptions<UserSettings> userSettings,
        IOptions<ApiSettings> apiSettings
        )
    {
        _hostEnvironment = hostEnvironment;
        _userSettings = userSettings.Value;
        _apiSettings = apiSettings.Value;
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertFacebookPdfForAccounting([FromForm] IFormFile file)
    {

        string newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) throw new ArgumentNullException();
        
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        string rawText = PdfText(fullPath);

        var productsPrices = ProductPriceDictionaryFromText(rawText);
        
        var sWebRootFolder = _hostEnvironment.WebRootPath;
        var sFileName = @"Facebook_Accounting.xlsx";

        var memory = new MemoryStream();

        var productNames = typeof(Facebook).GetFields()
            .Select(field => field.Name)
            .ToList();

        foreach (var product in productNames)
        {
            var fieldFacebook = typeof(Facebook).GetField(product, BindingFlags.Public | BindingFlags.Static);
            var valueFacebook = (string)fieldFacebook.GetValue(null);
            
            var fieldERP = typeof(ERP_Accounting).GetField(product, BindingFlags.Public | BindingFlags.Static);
            var valueERP = (string)fieldERP.GetValue(null);

            if (productsPrices.ContainsKey(valueFacebook))
            {
                RenameKey(productsPrices, valueFacebook, valueERP);
            }
        }
        
        
        var sortedProducts = productsPrices.OrderBy(x => x.Key);

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            var excelSheet = workbook.CreateSheet("Products Summed");

            var row = excelSheet.CreateRow(0);

            foreach (var dictEntry in sortedProducts)
            {
                row = excelSheet.CreateRow(excelSheet.LastRowNum+1);
                row.CreateCell(row.Cells.Count()).SetCellValue(dictEntry.Key);
                row.CreateCell(row.Cells.Count()).SetCellValue((double)dictEntry.Value);
                row.CreateCell(row.Cells.Count()).SetCellValue((double)dictEntry.Value*euroRate);
            }

            workbook.Write(fs);

        }

        await using (var streatWrite = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {

            await streatWrite.CopyToAsync(memory);

        }

        memory.Position = 0;

        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        
    }
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertFacebookPdfForMarketing([FromForm] IFormFile file)
    {
        
        var dateString = regexDate.Matches(file.FileName)[0];
        
        var date = DateTime.ParseExact(dateString.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var monthErpField = typeof(ErpMonths).GetField(date.ToString("MMMM"), BindingFlags.Public | BindingFlags.Static);
        var monthErp = (string)monthErpField.GetValue(null);

        var yearErp = date.ToString("yyyy");

        var newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) throw new ArgumentNullException();
        
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        var rawText = PdfText(fullPath);

        var productsPrices = ProductPriceDictionaryFromText(rawText);
        
        var sWebRootFolder = _hostEnvironment.WebRootPath;
        var sFileName = @"Facebook_Marketing.xlsx";

        var memory = new MemoryStream();

        var sortedProducts = productsPrices.OrderBy(x => x.Key);

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            foreach (var product in sortedProducts)
            {

                var price = (double)product.Value * euroRate;
                var priceRounded = Math.Round(price, 2);

                CreateErpMarketingXlsSheet(workbook,
                    product.Key,
                    monthErp,
                    yearErp,
                    Impressions,
                    FacebookBgCapital,
                    FacebookBgLower,
                    FacebookEng,
                    priceRounded,
                    "",
                    product.Key
                );

                await PostMarketingActivitiesToErp(FacebookEng, product.Key, priceRounded, date);
                
            }
            
            workbook.Write(fs);

        }

        await using (var streatWrite = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        {

            await streatWrite.CopyToAsync(memory);

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
        string newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

                if (file.Length > 0)
        {

            var sFileExtension = System.IO.Path.GetExtension(file.FileName)?.ToLower();

            if (file.FileName != null)
            {
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

                // var headerRow = sheet.GetRow(0); //Get Header Row

                // int cellCount = headerRow.LastCellNum;

                // for (var j = 0; j < cellCount; j++)
                // {
                //     var cell = headerRow.GetCell(j);
                //
                //     if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                //
                // }

                var fieldsValues = new List<string>();
                for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                {

                    IRow row = sheet.GetRow(i);

                    if (row == null) continue;

                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    
                    var fieldsGoogle = typeof(Google_Marketing).GetFields(BindingFlags.Public | BindingFlags.Static);
                    fieldsValues.AddRange(fieldsGoogle.Select(field => (string)field.GetValue(null)));

                    var productRow = row.GetCell(2);
                    if (productRow==null) continue;
                    
                    var product = productRow.ToString()?.TrimEnd();
                    if (string.IsNullOrEmpty(product)) continue;
                    if (!fieldsValues.Contains(product)) continue;
                    
                    var productName = productRow;
                    var priceRow = row.GetCell(4).ToString()?.TrimEnd();
                    var priceString = priceRegex.Matches(priceRow)[0].ToString();
                    var price = decimal.Parse(priceString);
                        
                    var dateRow = row.GetCell(0).ToString().TrimEnd();
                        
                    var date = DateTime.ParseExact(dateRow, "MMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);;
                    
                }

            }
        }

        return BadRequest();
    }

    private static string PdfText(string path)
    {
        PdfReader reader = new PdfReader(path);
        string text = string.Empty;
        for(int page = 1; page <= reader.NumberOfPages; page++)
        {
            text += PdfTextExtractor.GetTextFromPage(reader,page);
            text += Environment.NewLine;
        }
        reader.Close();
        return text;
    }

    private Dictionary<string, decimal> ProductPriceDictionaryFromText(string rawText)
    {
        // Regex priceRegex = new Regex(@"[0-9]*\.[0-9]*");
        // Regex priceRegex = new Regex(@"[0-9]*\,[0-9]*");
        // Regex priceRegex = new Regex(@"\d+(?:[\.\,]\d{2})?");

        Dictionary<string, decimal> productsPrices = new Dictionary<string, decimal>();

        var rawTextSplit = rawText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

        var fieldsFacebook = typeof(Facebook).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var productField in fieldsFacebook)
        {
            var product = (string)productField.GetValue(null);
            // if (!rawTextSplit.Contains(product)) continue;
            
            var lines = rawTextSplit
                .Where(element => element.Contains(product)).ToList();

            if (lines.Count==0) continue;
            
            foreach (var line in lines)
            {
                if (!productsPrices.ContainsKey(product))
                {
                    productsPrices.Add(product,0);
                }

                var priceString = priceRegex.Matches(line)[0].ToString();

                decimal price = 1;

                // if (line.Contains(',') & line.Contains('.'))
                // {
                //     
                //     price = decimal.Parse(priceString);
                //     
                // }
                //
                // else
                // {
                //     NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
                //     numberFormatWithComma.NumberDecimalSeparator = ",";
                //     price = decimal.Parse(priceString, numberFormatWithComma);
                // }
                
                NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
                numberFormatWithComma.NumberDecimalSeparator = ",";
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

    private void CreateErpMarketingXlsSheet(IWorkbook workbook,
        string sheetName,
        string month,
        string year,
        string measure,
        string type,
        string media,
        string publishType,
        double price,
        string TRP,
        string product
        )
    {
        var excelSheet = workbook.CreateSheet(sheetName);
        var row = excelSheet.CreateRow(0);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("МЕСЕЦ");
        row.CreateCell(row.Cells.Count()).SetCellValue(month);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("ГОДИНА");
        row.CreateCell(row.Cells.Count()).SetCellValue(year);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("Размер");
        row.CreateCell(row.Cells.Count()).SetCellValue(measure);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("тип реклама");
        row.CreateCell(row.Cells.Count()).SetCellValue(type);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("Медиа");
        row.CreateCell(row.Cells.Count()).SetCellValue(media);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("Издание");
        row.CreateCell(row.Cells.Count()).SetCellValue(publishType);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("цена реклама");
        row.CreateCell(row.Cells.Count()).SetCellValue(Math.Round(price, 2));

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("ТРП");
        row.CreateCell(row.Cells.Count()).SetCellValue(TRP);

        row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
        row.CreateCell(row.Cells.Count()).SetCellValue("ПРОДУКТ БРАНДЕКС");
        row.CreateCell(row.Cells.Count()).SetCellValue(product);
    }

    private async Task PostMarketingActivitiesToErp(string digital, string product, double price, DateTime date)
    {

        var subject = string.Empty;
        var partyId = string.Empty;
        var measure = string.Empty;
        var type = string.Empty;
        var media = string.Empty;
        var publishType = string.Empty;

        var monthErpField = typeof(ErpMonths).GetField(date.ToString("MMMM"), BindingFlags.Public | BindingFlags.Static);
        var monthErp = (string)monthErpField.GetValue(null);

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
        }
        
        var activityObject = new MarketingActivityCm()
            {
                DocumentType = new MarketingActivityCm._DocumentType()
                {
                    Id = "General_DocumentTypes(59b265f7-391a-4226-8bcb-44e192ba5690)"
                },
                EnterpriseCompany = new MarketingActivityCm._EnterpriseCompany()
                {
                    Id = "General_EnterpriseCompanies(2c186d87-e81d-4318-9a7f-3cfb5399c0d0)"
                },
                EnterpriseCompanyLocation = new MarketingActivityCm._EnterpriseCompanyLocation()
                {
                    Id = "General_Contacts_CompanyLocations(902743f5-6076-4b5e-b725-2daa192c71f6)"
                },
                SystemType = "Task",
                Subject = subject,
                ResponsibleParty = new MarketingActivityCm._ResponsibleParty()
                {
                    Id = "General_Contacts_Parties(2469d153-839f-445a-b7c2-2e7cb955c491)"
                },
                ReferenceDate =  $"{date:yyyy-MM-dd}",
                DeadlineTime = $"{date:yyyy-MM-dd}",
                
                OwnerParty = new MarketingActivityCm._OwnerParty()
                {
                    Id = "General_Contacts_Parties(2469d153-839f-445a-b7c2-2e7cb955c491)"
                },
                
                ResponsiblePerson = new MarketingActivityCm._ResponsiblePerson()
                {
                    Id = "General_Contacts_Persons(623ed5c7-2eec-4e5b-a0c1-42c6faab3309)"
                },
                
                ToParty = new MarketingActivityCm._ToParty()
                {
                    Id = $"General_Contacts_Parties({partyId})"
                },
                
                TargetParty = new MarketingActivityCm._TargetParty()
                {
                    Id =$"General_Contacts_Parties({partyId})"
                },
                
                CustomProperty_МЕСЕЦ = new MarketingActivityCm._CustomProperty_МЕСЕЦ()
                {
                    Value = monthErp
                },
                
                CustomProperty_1579648 = new MarketingActivityCm._CustomProperty_1579648()
                {
                    Value = yearErp
                },
                CustomProperty_Размер = new MarketingActivityCm._CustomProperty_Размер()
                {
                    Value = measure
                },
                CustomProperty_тип_u0020реклама = new MarketingActivityCm._CustomProperty_тип_u0020реклама()
                {
                    Value = type
                },
                CustomProperty_ре = new MarketingActivityCm._CustomProperty_ре()
                {
                    Value = media
                },
                CustomProperty_novinar = new MarketingActivityCm._CustomProperty_novinar()
                {
                    Value = publishType
                },
                CustomProperty_цена_u0020реклама = new MarketingActivityCm._CustomProperty_цена_u0020реклама()
                {
                    Value = $"{price}"
                },
                CustomProperty_058 = new MarketingActivityCm._CustomProperty_058()
                {
                    Value = ""
                },
                CustomProperty_ПРОДУКТ_u0020БРАНДЕКС = new MarketingActivityCm._CustomProperty_ПРОДУКТ_u0020БРАНДЕКС()
                {
                    Value = product
                },

            };
        
        string json = JsonConvert.SerializeObject(activityObject, Formatting.Indented);
            
        var clientErp = new HttpClient();
            
        var byteArray = Encoding.ASCII.GetBytes($"{_userSettings.MarketingAccount}:{_userSettings.MarketingPassword}");
        clientErp.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
        var uri = new Uri(_apiSettings.GeneralContactActivities);
            
        var content = new StringContent(json, Encoding.UTF8, RequestConstants.ApplicationJson);

        var response = await clientErp.PostAsync(uri, content);

        var responceContent = await response.Content.ReadAsStringAsync();

        JObject obj = JObject.Parse(responceContent);

        var documentId = obj["@odata.id"].ToString();

        var uriChangeSrate = new Uri($"{_apiSettings.GeneralRequest}{documentId}/ChangeState");

        var newState = new { newState = "Released" };

        string newStateSerialized = JsonConvert.SerializeObject(newState);
        
        var stateContent =  new StringContent(newStateSerialized, Encoding.UTF8, RequestConstants.ApplicationJson);
        var responseState = await clientErp.PostAsync(uriChangeSrate, stateContent);

        Console.WriteLine(responseState);
    }
    
    
}
namespace BrandexSalesAdapter.Accounting.Controllers;

using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

using BrandexSalesAdapter.Controllers;
using Infrastructure;

using static Common.ProductConstants;
using static  Common.Constants;


public class FacebookCountingController : ApiController
{

    private readonly IWebHostEnvironment _hostEnvironment;
    
    public FacebookCountingController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertPdfForAccounting([FromForm] IFormFile file)
    {

        double euroRate = 1.9894;

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
        
        RenameKey(productsPrices, Facebook.ZinSeD, ERP_Accounting.ZinSeD);
        RenameKey(productsPrices, Facebook.EnzyMill, ERP_Accounting.EnzyMill);
        RenameKey(productsPrices, Facebook.CystiRen, ERP_Accounting.CystiRen);
        RenameKey(productsPrices, Facebook.LadyHarmonia, ERP_Accounting.LadyHarmonia);
        RenameKey(productsPrices, Facebook.LaxaL, ERP_Accounting.LaxaL);
        RenameKey(productsPrices, Facebook.Bland, ERP_Accounting.Bland);
        RenameKey(productsPrices, Facebook.DiabeForGluco, ERP_Accounting.DiabeForGluco);
        RenameKey(productsPrices, Facebook.GinkgoVin, ERP_Accounting.GinkgoVin);
        RenameKey(productsPrices, Facebook.Venaxin, ERP_Accounting.Venaxin);
        RenameKey(productsPrices, Facebook.ForFlex, ERP_Accounting.ForFlex);
        RenameKey(productsPrices, Facebook.ProstaRen, ERP_Accounting.ProstaRen);
        RenameKey(productsPrices, Facebook.Sleep, ERP_Accounting.Sleep);
        RenameKey(productsPrices, Facebook.DetoxiFive, ERP_Accounting.DetoxiFive);
        RenameKey(productsPrices, Facebook.GeneralAudience, ERP_Accounting.Botanic);
        
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
    public async Task<IActionResult> ConvertPdfForMarketing([FromForm] IFormFile file)
    {

        double euroRate = 1.9894;
        
        var regexMonth = new Regex(@"[0-9]{2}");
        var regexYear = new Regex(@"[0-9]{4}");

        string newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        if (file.Length <= 0) throw new ArgumentNullException();
        
        var fullPath = System.IO.Path.Combine(newPath, file.FileName);

        await using var streamRead = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(streamRead);

        string rawText = PdfText(fullPath);

        var productsPrices = ProductPriceDictionaryFromText(rawText);
        
        var sWebRootFolder = _hostEnvironment.WebRootPath;
        var sFileName = @"Facebook_Marketing.xlsx";

        var memory = new MemoryStream();
        
        RenameKey(productsPrices, Facebook.ZinSeD, ERP_Marketing.ZinSeD);
        RenameKey(productsPrices, Facebook.EnzyMill, ERP_Marketing.EnzyMill);
        RenameKey(productsPrices, Facebook.CystiRen, ERP_Marketing.CystiRen);
        RenameKey(productsPrices, Facebook.LadyHarmonia, ERP_Marketing.LadyHarmonia);
        RenameKey(productsPrices, Facebook.LaxaL, ERP_Marketing.LaxaL);
        RenameKey(productsPrices, Facebook.Bland, ERP_Marketing.Bland);
        RenameKey(productsPrices, Facebook.DiabeForGluco, ERP_Marketing.DiabeForGluco);
        RenameKey(productsPrices, Facebook.GinkgoVin, ERP_Marketing.GinkgoVin);
        RenameKey(productsPrices, Facebook.Venaxin, ERP_Marketing.Venaxin);
        RenameKey(productsPrices, Facebook.ForFlex, ERP_Marketing.ForFlex);
        RenameKey(productsPrices, Facebook.ProstaRen, ERP_Marketing.ProstaRen);
        RenameKey(productsPrices, Facebook.Sleep, ERP_Marketing.Sleep);
        RenameKey(productsPrices, Facebook.DetoxiFive, ERP_Marketing.DetoxiFive);
        RenameKey(productsPrices, Facebook.GeneralAudience, ERP_Marketing.Botanic);
        
        var sortedProducts = productsPrices.OrderBy(x => x.Key);

        var dateMonth = regexMonth.Matches(file.FileName)[2];
        var dateYear = regexYear.Matches(file.FileName)[0];

        await using (var fs = new FileStream(System.IO.Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();

            foreach (var product in sortedProducts)
            {

                var price = (double)product.Value * euroRate;
                
                var excelSheet = workbook.CreateSheet(product.Key);
                var row = excelSheet.CreateRow(0);
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("МЕСЕЦ");
                row.CreateCell(row.Cells.Count()).SetCellValue(dateMonth.ToString());
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("ГОДИНА");
                row.CreateCell(row.Cells.Count()).SetCellValue(dateYear.ToString());
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("Размер");
                row.CreateCell(row.Cells.Count()).SetCellValue("впечатления");
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("тип реклама");
                row.CreateCell(row.Cells.Count()).SetCellValue("Фейсбук");
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("Медиа");
                row.CreateCell(row.Cells.Count()).SetCellValue("фейсбук");
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("Издание");
                row.CreateCell(row.Cells.Count()).SetCellValue("Facebook");
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("цена реклама");
                row.CreateCell(row.Cells.Count()).SetCellValue(Math.Round(price, 2));
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("ТРП");
                row.CreateCell(row.Cells.Count()).SetCellValue("");
                
                row = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                row.CreateCell(row.Cells.Count()).SetCellValue("ПРОДУКТ БРАНДЕКС");
                row.CreateCell(row.Cells.Count()).SetCellValue(product.Key);
                
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
        Regex priceRegex = new Regex(@"[0-9]*\.[0-9]*");

        Dictionary<string, decimal> productsPrices = new Dictionary<string, decimal>();

        var rawTextSplit = rawText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

        var products = new string[]
        {
            Facebook.ZinSeD,
            Facebook.EnzyMill,
            Facebook.CystiRen,
            Facebook.LadyHarmonia,
            Facebook.LaxaL,
            Facebook.Bland,
            Facebook.DiabeForGluco,
            Facebook.GinkgoVin,
            Facebook.Venaxin,
            Facebook.ForFlex,
            Facebook.ProstaRen,
            Facebook.Sleep,
            Facebook.DetoxiFive,
            Facebook.GeneralAudience
        };

        foreach (var product in products)
        {
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

                var price = decimal.Parse(priceRegex.Matches(line)[0].ToString());

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
    
    
}
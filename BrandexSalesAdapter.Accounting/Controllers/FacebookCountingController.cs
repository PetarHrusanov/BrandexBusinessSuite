namespace BrandexSalesAdapter.Accounting.Controllers;

using System.IO;

using BrandexSalesAdapter.Controllers;
using BrandexSalesAdapter.Infrastructure;
using BrandexSalesAdapter.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

using iTextSharp.text.pdf.parser;

using System.Threading.Tasks;


public class FacebookCountingController : ApiController
{

    private readonly IWebHostEnvironment _hostEnvironment;
    
    public FacebookCountingController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Convert([FromForm] IFormFile file)
    {

        string newPath = CreateFileDirectories.CreatePDFFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

        if (file.Length <= 0) return "Error";
        var sFileExtension = System.IO.Path.GetExtension(file.FileName)?.ToLower();
        {
            var fullPath = System.IO.Path.Combine(newPath, file.FileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

        }



        var kur =
            "/Users/Petar/Documents/Documents – Petar’s MacBook Pro/Programiranka/Firmata/BrandexSalesAdapter/Server/BrandexSalesAdapter/BrandexSalesAdapter.Accounting/wwwroot/UploadExcel/2022-05-10T07-08 Transaction #5018559291594644-9749365.pdf";

        return PdfText(kur);
        
               

        return PdfText(newPath+sFileExtension);
    }

    private static string PdfText(string path)
    {
        PdfReader reader = new PdfReader(path);
        string text = string.Empty;
        for(int page = 1; page <= reader.NumberOfPages; page++)
        {
            text += PdfTextExtractor.GetTextFromPage(reader,page);
        }
        reader.Close();
        return text;
    }   
    
}
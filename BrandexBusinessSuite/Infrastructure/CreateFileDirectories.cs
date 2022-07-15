using Microsoft.AspNetCore.Http;

namespace BrandexBusinessSuite.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using static Common.Constants;

public abstract class CreateFileDirectories
{
    public static string CreateExcelFilesInputDirectory(IWebHostEnvironment hostEnvironment)
    {
        
        var webRootPath = hostEnvironment.WebRootPath;

        var newPath = Path.Combine(webRootPath, UploadExcel);
        
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        
        return newPath;
    }

    public static string CreateExcelFilesInputCompletePath(IWebHostEnvironment hostEnvironment, IFormFile file)
    {
        var newPath = CreateExcelFilesInputDirectory(hostEnvironment);
        return Path.Combine(newPath, file.FileName);
    }
    
    public static string CreatePDFFilesInputDirectory(IWebHostEnvironment hostEnvironment)
    {
        return CreateFileDirectoriesByType(hostEnvironment, UploadPdf);
    }

    private static string CreateFileDirectoriesByType(IWebHostEnvironment hostEnvironment, string type)
    {
        string webRootPath = hostEnvironment.WebRootPath;
        var newPath = "";

        switch (type)
        {
            case UploadExcel:
                newPath = Path.Combine(webRootPath, UploadExcel);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                return newPath;
            case UploadPdf:
                newPath = Path.Combine(webRootPath, UploadExcel);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                return newPath;
            default:
                return Path.Combine(webRootPath, "");
        }
        
    }
    
}
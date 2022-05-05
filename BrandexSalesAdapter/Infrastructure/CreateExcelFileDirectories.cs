namespace BrandexSalesAdapter.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using static Common.Constants;

public abstract class CreateExcelFileDirectories
{
    public static string CreateExcelFilesInputDirectory(IWebHostEnvironment hostEnvironment)
    {
        
        string webRootPath = hostEnvironment.WebRootPath;

        string newPath = Path.Combine(webRootPath, UploadExcel);
        
        if (!Directory.Exists(newPath))

        {

            Directory.CreateDirectory(newPath);

        }
        
        return newPath;
    }
}
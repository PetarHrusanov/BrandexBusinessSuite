using BrandexBusinessSuite.Common;
using BrandexBusinessSuite.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BrandexBusinessSuite.Methods;

using NPOI.SS.UserModel;

public class ExcelMethods
{
    public static int ConvertRowToInt(IRow row, int column)
    {
        return int.TryParse(row.GetCell(column)?.ToString()?.TrimEnd(), out var idInt) ? idInt : 0;
    }

    public static bool CheckXlsx(IFormFile file, IList<string> errors)
    {
        if (file.Length > 0 && Path.GetExtension(file.FileName).ToLower() == ".xlsx") return true;
        errors.Add(Constants.Errors.IncorrectFileFormat);
        return false;

    }
    
    
}
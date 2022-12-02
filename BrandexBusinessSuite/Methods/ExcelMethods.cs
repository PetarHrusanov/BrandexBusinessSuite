namespace BrandexBusinessSuite.Methods;

using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;

public static class ExcelMethods
{
    public static int ConvertRowToInt(IRow row, int column) 
        =>int.TryParse(row.GetCell(column)?.ToString()?.TrimEnd(), out var idInt) ? idInt : 0;

    public static bool CheckXlsx(IFormFile file) 
        => file.Length > 0 && Path.GetExtension(file.FileName).ToLower() == ".xlsx";

}
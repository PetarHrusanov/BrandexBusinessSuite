namespace BrandexBusinessSuite.SalesAnalysis.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models;
using Infrastructure;
using Services.PharmacyChains;

using static Common.InputOutputConstants.SingleStringConstants;
using static Common.ExcelDataConstants.ExcelLineErrors;

public class PharmacyChainsController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;

    // db Services
    private readonly IPharmacyChainsService _pharmacyChainsService;

    public PharmacyChainsController(
        IWebHostEnvironment hostEnvironment,
        IPharmacyChainsService pharmacyChainsService)

    {
        _hostEnvironment = hostEnvironment;
        _pharmacyChainsService = pharmacyChainsService;
    }
        
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm]IFormFile file)
    {

        var newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new List<string>();

        var pharmacyChainsCheck = await _pharmacyChainsService.GetPharmacyChainsCheck();
        var uniquePharmacyChains = new List<string>();

        if (file.Length > 0)
        {
            var sFileExtension = Path.GetExtension(file.FileName)?.ToLower();
            var fullPath = Path.Combine(newPath, file.FileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            stream.Position = 0;

            ISheet sheet;
            
            if (sFileExtension == ".xls")
            {
                var hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                sheet = hssfwb.GetSheetAt(0);
            }

            else
            {
                var hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                sheet = hssfwb.GetSheetAt(0);
            }

            for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)

            {
                var row = sheet.GetRow(i);

                if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                var chainName = row.GetCell(0);

                if (chainName == null)
                {
                    errorDictionary.Add($"{i} Line: {IncorrectPharmacyChainName}");
                    continue;
                }
                
                var chainNameString = chainName.ToString()!.ToUpper().TrimEnd();
                
                if (pharmacyChainsCheck.All(c =>
                        !string.Equals(c.Name, chainNameString, StringComparison.CurrentCultureIgnoreCase)))
                {
                    uniquePharmacyChains.Add(chainNameString);
                }
            }

            await _pharmacyChainsService.UploadBulk(uniquePharmacyChains);
        }

        var outputSerialized = JsonConvert.SerializeObject(errorDictionary.ToArray());

        return outputSerialized;

    }
    
    [HttpPost]
    public async Task<string> Upload([FromBody]SingleStringInputModel singleStringInputModel)
    {
        if (singleStringInputModel.SingleStringValue != null)
        {
            await _pharmacyChainsService.UploadPharmacyChain(singleStringInputModel.SingleStringValue);
        }
            
        var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);
        outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

        return outputSerialized;
            
    }
}
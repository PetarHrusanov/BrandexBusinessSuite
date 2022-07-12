﻿namespace BrandexBusinessSuite.ExcelLogic.Controllers;

using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models;

using Infrastructure;
using Services.Cities;

using static Common.InputOutputConstants.SingleStringConstants;
using static Common.Constants;

public class CitiesController : AdministrationController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ICitiesService _citiesService;

    public CitiesController(IWebHostEnvironment hostEnvironment, ICitiesService citiesService)

    {
        _hostEnvironment = hostEnvironment;
        _citiesService = citiesService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<string> Import([FromForm] IFormFile file)
    {
        var newPath = CreateFileDirectories.CreateExcelFilesInputDirectory(_hostEnvironment);

        var errorDictionary = new Dictionary<int, string>();

        var citiesCheck = await _citiesService.GetCitiesCheck();

        var uniqueCities = new List<string>();

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
            

            for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                var cityRow = row.GetCell(0).ToString()?.TrimEnd();
                
                if (!string.IsNullOrEmpty(cityRow)
                    && citiesCheck.All(c =>
                        !string.Equals(c.Name, cityRow, StringComparison.CurrentCultureIgnoreCase)))
                {
                    uniqueCities.Add(cityRow.ToUpper());
                }
                
            }

            await _citiesService.UploadBulk(uniqueCities);
        }

        var errorModel = new CustomErrorDictionaryOutputModel { Errors = errorDictionary };

        var outputSerialized = JsonConvert.SerializeObject(errorModel);

        return outputSerialized;
    }

    [HttpPost]
    [Authorize(Roles = AdministratorRoleName)]
    public async Task<string> Upload([FromBody] SingleStringInputModel singleStringInputModel)
    {
        if (singleStringInputModel.SingleStringValue != null)
        {
            await _citiesService.UploadCity(singleStringInputModel.SingleStringValue);
        }

        var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);

        outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);

        return outputSerialized;
    }
}
// using BrandexBusinessSuite.Common;
// using BrandexBusinessSuite.Infrastructure;
// using BrandexBusinessSuite.Models;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;
// using NPOI.SS.UserModel;
//
// namespace BrandexBusinessSuite.FuelReport.Controllers;
//
// using BrandexBusinessSuite.Controllers;
//
// public class CitiesController : AdministrationController
// {
//     private readonly IWebHostEnvironment _hostEnvironment;
//     public CitiesController(IWebHostEnvironment hostEnvironment)
//     {
//         _hostEnvironment = hostEnvironment;
//         // _citiesService = citiesService;
//     }
//     
//     
//     
//     
//     [HttpPost]
//     [Authorize(Roles = AdministratorRoleName)]
//     public async Task<string> Upload([FromBody] SingleStringInputModel singleStringInputModel)
//     {
//         // await _citiesService.UploadCity(singleStringInputModel.SingleStringValue);
//     
//         // var outputSerialized = JsonConvert.SerializeObject(singleStringInputModel);
//         // outputSerialized = outputSerialized.Replace(SingleStringValueCapital, SingleStringValueLower);
//     
//         return outputSerialized;
//     }
//     
// }
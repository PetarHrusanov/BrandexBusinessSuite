﻿namespace BrandexSalesAdapter.ExcelLogic.Services.Regions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IRegionsService
    {
        Task<string> UploadRegion(string regionName);

        Task<bool> CheckRegionByName(string regionName);

        Task<int> IdByName(string regionName);

        Task<List<SelectListItem>> RegionsForSelect();
    }
}
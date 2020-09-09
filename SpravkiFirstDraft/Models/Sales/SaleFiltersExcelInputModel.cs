namespace SpravkiFirstDraft.Models.Sales
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using SpravkiFirstDraft.Models.Enums;

    public class SaleFiltersExcelInputModel
    {
        public string Date { get; set; }

        public RegionInput Region { get; set; }

        public List<SelectListItem> Options { get; set; }

    }
}

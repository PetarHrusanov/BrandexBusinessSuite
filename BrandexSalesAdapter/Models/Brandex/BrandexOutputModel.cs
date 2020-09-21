namespace BrandexSalesAdapter.ExcelLogic.Models.Brandex
{
    using System;
    using System.Collections.Generic;

    public class BrandexOutputModel
    {
        public string Date { get; set; }

        public string Table { get; set; }

        public Dictionary<int, string> Errors { get; set; }
    }
}

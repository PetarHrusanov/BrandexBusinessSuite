namespace BrandexSalesAdapter.ExcelLogic.Models.Phoenix
{
    using System;
    using System.Collections.Generic;

    public class PhoenixOutputModel
    {
        public string Date { get; set; }

        public string Table { get; set; }

        public Dictionary<int, string> Errors { get; set; }
    }
}

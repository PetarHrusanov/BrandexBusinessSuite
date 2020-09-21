﻿namespace BrandexSalesAdapter.ExcelLogic.Models.Sopharma
{
    using System.Collections.Generic;

    public class SopharmaOutputModel
    {
        public string Date { get; set; }

        public string Table { get; set; }

        public Dictionary<int, string> Errors { get; set; }
    }
}

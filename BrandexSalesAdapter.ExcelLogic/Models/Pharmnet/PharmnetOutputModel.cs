namespace BrandexSalesAdapter.ExcelLogic.Models.Pharmnet
{
    using System.Collections.Generic;

    public class PharmnetOutputModel
    {
        public string Date { get; set; }

        public string Table { get; set; }

        public Dictionary<int, string> Errors { get; set; }
    }
}

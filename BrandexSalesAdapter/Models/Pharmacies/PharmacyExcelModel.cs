namespace BrandexSalesAdapter.ExcelLogic.Models.Pharmacies
{
    using System;
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Data.Enums;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;

    public class PharmacyExcelModel
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public PharmacyClass PharmacyClass { get; set; }

        public string Region { get; set; }

        public ICollection <SaleExcelOutputModel> Sales { get; set; }

        //public IEnumerable 
    }
}

using System;
namespace SpravkiFirstDraft.Models.Sales
{
    public class SaleOutputModel
    {
        public string PharmacyName { get; set; }

        public string ProductName { get; set; }

        public string DistributorName { get; set; }

        public string Date { get; set; }

        public int Count { get; set; }
    }
}

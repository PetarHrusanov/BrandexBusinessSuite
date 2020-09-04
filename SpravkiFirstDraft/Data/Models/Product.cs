namespace SpravkiFirstDraft.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Product
    {
        public Product()
        {
            this.Sales = new HashSet<Sale>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int BrandexId { get; set; }

        public int? PhoenixId { get; set; }

        public int? PharmnetId { get; set; }

        public int? StingId { get; set; }

        public string SopharmaId { get; set; }

        public double Price { get; set; }

        public double DiscountPrice
            => Price * 0.88;

        public virtual ICollection<Sale> Sales { get; set; }
    }
}

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

        public double Price { get; set; }

        public double DiscountPrice { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}

namespace BrandexSalesAdapter.ExcelLogic.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class PharmacyChain
    {
        public PharmacyChain()
        {
            this.Pharmacies = new HashSet<Pharmacy>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Pharmacy> Pharmacies { get; set; }
    }
}

namespace SpravkiFirstDraft.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Company
    {
        public Company()
        {
            this.Pharmacies = new HashSet<Pharmacy>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public string VAT { get; set; }

        public virtual ICollection<Pharmacy> Pharmacies { get; set; }

    }
}

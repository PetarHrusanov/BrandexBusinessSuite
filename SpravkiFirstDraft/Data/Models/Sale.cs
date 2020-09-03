namespace SpravkiFirstDraft.Data.Models
{
    using System;

    public class Sale
    {
        public int Id { get; set; }

        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }

    }
}

namespace SpravkiFirstDraft.Data.Models
{
    using System.Collections.Generic;

    public class City
    {
        public City()
        {
            this.Pharmacies = new HashSet<Pharmacy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Pharmacy> Pharmacies { get; set; }
    }
}

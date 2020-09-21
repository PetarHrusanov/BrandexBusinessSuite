namespace BrandexSalesAdapter.ExcelLogic.Data.Models
{
    using System.Collections.Generic;

    public class Distributor
    {

        public Distributor()
        {
            this.Sales = new HashSet<Sale>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}

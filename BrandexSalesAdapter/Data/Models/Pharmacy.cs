namespace BrandexSalesAdapter.ExcelLogic.Data.Models
{
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Data.Enums;

    public class Pharmacy
    {
        public Pharmacy()
        {
            this.Sales = new HashSet<Sale>();
        }

        public int Id { get; set; }

        public int BrandexId { get; set; }

        public string Name { get; set; }

        public PharmacyClass PharmacyClass{ get;set; }

        public bool Active { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int PharmacyChainId { get; set; }
        public virtual PharmacyChain PharmacyChain { get; set; }

        public string Address { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public int? PharmnetId { get; set; }

        public int? PhoenixId { get; set; }

        public int? SopharmaId { get; set; }

        public int? StingId { get; set; }

        public int RegionId { get; set; }
        public virtual Region Region { get; set; }   

        public virtual ICollection<Sale> Sales { get; set; }

    }
}

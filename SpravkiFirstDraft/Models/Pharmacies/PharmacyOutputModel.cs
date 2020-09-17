namespace SpravkiFirstDraft.Models.Pharmacies
{
    public class PharmacyOutputModel
    {
        public int BrandexId { get; set; }

        public string Name { get; set; }

        public string PharmacyClass { get; set; }

        public string CompanyName { get; set; }

        public string PharmacyChainName { get; set; }

        public string Address { get; set; }

        public string CityName { get; set; }

        public int? PharmnetId { get; set; }

        public int? PhoenixId { get; set; }

        public int? SopharmaId { get; set; }

        public int? StingId { get; set; }

        public string Region { get; set; }
    }
}

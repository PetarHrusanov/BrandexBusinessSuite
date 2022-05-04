namespace BrandexSalesAdapter.Common
{
    public class DataConstants
    {
        public class Ditributors
        {
            public const string Sopharma = "СОФАРМА";
            public const string Phoenix = "ФЬОНИКС";
            public const string Sting = "СТИНГ";
            public const string Brandex = "БРАНДЕКС";
            public const string Pharmnet = "ФАРМНЕТ";

            public const string SopharmaEng = "Sopharma";
            public const string PhoenixEng = "Phoenix";
            public const string StingEng = "Sting";
            public const string BrandexEng = "Brandex";
            public const string PharmnetEng = "Pharmnet";
        }

        public class SalesColumns
        {
            public const string Sales = "Sales";
            public const string PharmacyId = "PharmacyId";
            public const string ProductId = "ProductId";
            public const string DistributorId = "DistributorId";
            public const string Date = "Date";
            public const string Count = "Count";
        }

        public class PharmacyColumns
        {
            public const string Pharmacies = "Pharmacies";
            public const string BrandexId = "BrandexId";
            public const string Name = "Name";
            public const string PharmacyClass = "PharmacyClass";
            public const string Active = "Active";
            public const string CompanyId = "CompanyId";
            public const string PharmacyChainId = "PharmacyChainId";
            public const string Address = "Address";
            public const string CityId = "CityId";
            public const string PharmnetId = "PharmnetId";
            public const string PhoenixId = "PhoenixId";
            public const string SopharmaId = "SopharmaId";
            public const string StingId = "StingId";
            public const string RegionId = "RegionId";
            
        }
        
        public class CitiesColumns
        {
            public const string Cities = "Cities";
            public const string Name = "Name";
        }
        
        public class PharmacyChainsColumns
        {
            public const string PharmacyChains = "PharmacyChains";
            public const string Name = "Name";
        }
        
        public class PharmacyCompaniesColumns
        {
            public const string PharmacyCompanies = "Companies";
            public const string Name = "Name";
            public const string Owner = "Owner";
            public const string VAT = "VAT";
        }
        
        public class Regions
        {
            // public const string Burgas = "БУРГАС";
            // public const string Varna = "ВАРНА";
            // public const string Vidin = "ВИДИН";
            // public const string Neobslujvan = "НЕОБСЛУЖВАН";
            // public const string OnlineMagazin = "ОНЛАЙН МАГАЗИН";
            // public const string Vidin = "ВИДИН";
            // public const string Neobslujvan = "НЕОБСЛУЖВАН";
            // public const string OnlineMagazin = "ОНЛАЙН МАГАЗИН";
            
            
        }

        public class ExcelLineErrors
        {
            public const string IncorrectDateFormat = "Incorrect date format.";
            public const string IncorrectProductId = "Incorrect product id.";
            public const string IncorrectPharmacyId = "Incorrect pharmacy id.";
            public const string IncorrectSalesCount = "Incorrect sales count.";

            public const string IncorrectPharmacyCompanyId = "Incorrect pharmacy company ID.";
            public const string IncorrectPharmacyChainId = "Incorrect pharmacy chain ID.";
            public const string IncorrectRegionId = "Incorrect region ID.";
            public const string IncorrectCityId = "Incorrect city ID.";
            
            public const string IncorrectCityName = "Incorrect city name.";
            public const string IncorrectPharmacyChainName = "Incorrect pharmacy chain name.";
            public const string IncorrectPharmacyCompanyName = "Incorrect pharmacy company name.";
            public const string IncorrectRegion = "Incorrect region.";
            
            public const string IncorrectPrice = "Incorrect price.";
            
            public const string IncorrectProductBrandexId = "Incorrect product Brandex Id.";
            public const string IncorrectProductPhoenixId = "Incorrect product Phoenix Id.";
            public const string IncorrectProductPharmnetId = "Incorrect product Pharmnet Id.";
            public const string IncorrectProductSopharmaId = "Incorrect product Sopharma Id.";
            public const string IncorrectProductStingId = "Incorrect product Sting Id.";
        }
        
    }
}

namespace BrandexBusinessSuite.Accounting.Data.Seeding;

using BrandexBusinessSuite.Services.Data;
using Models;

using static Common.MarketingDataConstants;
using Common;
using static Common.ProductConstants;

public class ApplicationDbContextSeeder : ISeeder
{
    private readonly AccountingDbContext db;

    public ApplicationDbContextSeeder(AccountingDbContext db)
    {
        this.db = db;
    }
    
    public void SeedAsync()
    {
        if (!db.Products.Any())
        {
            foreach (var product in GetProducts())
            {
                db.Products.Add(product);
            }
            db.SaveChanges();
        }

        if (!db.Currencies.Any())
        {
            foreach (var currency in GetCurrencies())
            {
                db.Currencies.Add(currency);
            }
            db.SaveChanges();
        }
        
        if (db.MarketingActivityDetails.Any()) return;
        foreach (var media in GetMarketingActivityDetails())
        {
            db.MarketingActivityDetails.Add(media);
        }
        db.SaveChanges();

    }

    private static IEnumerable<Product> GetProducts() =>
        new List<Product>
        {
            new()
            {
                Name = MarketingNames.ZinSeD,
                FacebookName = ProductConstants.Facebook.ZinSeD,
                GoogleName = GoogleMarketing.ZinSeD,
                GoogleNameErp = GoogleMarketingErp.ZinSeD,
                AccountingName = ERP_Accounting.ZinSeD,
                AccountingErpNumber = ErpCodesNumber.ZinSeD
            },
            new()
            {
                Name = MarketingNames.EnzyMill,
                FacebookName = ProductConstants.Facebook.EnzyMill,
                GoogleName = GoogleMarketing.EnzyMill,
                GoogleNameErp = GoogleMarketingErp.EnzyMill,
                AccountingName = ERP_Accounting.EnzyMill,
                AccountingErpNumber = ErpCodesNumber.EnzyMill
            },
            new()
            {
                Name = MarketingNames.CystiRen,
                FacebookName = ProductConstants.Facebook.CystiRen,
                GoogleName = GoogleMarketing.CystiRen,
                GoogleNameErp = GoogleMarketingErp.CystiRen,
                AccountingName = ERP_Accounting.CystiRen,
                AccountingErpNumber = ErpCodesNumber.CystiRen
            },
            new()
            {
                Name = MarketingNames.LadyHarmonia,
                FacebookName = ProductConstants.Facebook.LadyHarmonia,
                GoogleName = GoogleMarketing.LadyHarmonia,
                GoogleNameErp = GoogleMarketingErp.LadyHarmonia,
                AccountingName = ERP_Accounting.LadyHarmonia,
                AccountingErpNumber = ErpCodesNumber.LadyHarmonia
            },
            new()
            {
                Name = MarketingNames.DetoxiFive,
                FacebookName = ProductConstants.Facebook.DetoxiFive,
                GoogleName = GoogleMarketing.DetoxiFive,
                GoogleNameErp = GoogleMarketingErp.DetoxiFive,
                AccountingName = ERP_Accounting.DetoxiFive,
                AccountingErpNumber = ErpCodesNumber.DetoxiFive
            },
            new()
            {
                Name = MarketingNames.LaxaL,
                FacebookName = ProductConstants.Facebook.LaxaL,
                GoogleName = GoogleMarketing.LaxaL,
                GoogleNameErp = GoogleMarketingErp.LaxaL,
                AccountingName = ERP_Accounting.LaxaL,
                AccountingErpNumber = ErpCodesNumber.LaxaL
            },
            new()
            {
                Name = MarketingNames.Bland,
                FacebookName = ProductConstants.Facebook.Bland,
                GoogleName = GoogleMarketing.Bland,
                GoogleNameErp = GoogleMarketingErp.Bland,
                AccountingName = ERP_Accounting.Bland,
                AccountingErpNumber = ErpCodesNumber.Bland
            },
            new()
            {
                Name = MarketingNames.DiabeForGluco,
                FacebookName = ProductConstants.Facebook.DiabeForGluco,
                GoogleName = GoogleMarketing.DiabeForGluco,
                GoogleNameErp = GoogleMarketingErp.DiabeForGluco,
                AccountingName = ERP_Accounting.DiabeForGluco,
                AccountingErpNumber = ErpCodesNumber.DiabeForGluco
            },
            // new()
            // {
            //     Name = MarketingNames.DiabeForProtect,
            //     FacebookName = Facebook.DiabeForProtect,
            //     GoogleName = GenericNames.DiabeForProtect,
            //     AccountingName = ERP_Accounting.DiabeForProtect,
            //     AccountingErpNumber = ErpCodesNumber.DiabeForProtect
            // },
            new()
            {
                Name = MarketingNames.GinkgoVin,
                FacebookName = ProductConstants.Facebook.GinkgoVin,
                GoogleName = GoogleMarketing.GinkgoVin,
                GoogleNameErp = GoogleMarketingErp.GinkgoVin,
                AccountingName = ERP_Accounting.GinkgoVin,
                AccountingErpNumber = ErpCodesNumber.GinkgoVin
            },
            // new()
            // {
            //     Name = MarketingNames.GinkgoVinCentella,
            //     FacebookName = Facebook.GinkgoVinCentella,
            //     GoogleName = GenericNames.GinkgoVinCentella,
            //     AccountingName = ERP_Accounting.GinkgoVinCentella,
            //     AccountingErpNumber = ErpCodesNumber.GinkgoVinCentella
            // },
            new()
            {
                Name = MarketingNames.Venaxin,
                FacebookName = ProductConstants.Facebook.Venaxin,
                GoogleName = GoogleMarketing.Venaxin,
                GoogleNameErp = GoogleMarketingErp.Venaxin,
                AccountingName = ERP_Accounting.Venaxin,
                AccountingErpNumber = ErpCodesNumber.Venaxin
            },
            new()
            {
                Name = MarketingNames.ForFlex,
                FacebookName = ProductConstants.Facebook.ForFlex,
                GoogleName = GoogleMarketing.ForFlex,
                GoogleNameErp = GoogleMarketingErp.ForFlex,
                AccountingName = ERP_Accounting.ForFlex,
                AccountingErpNumber = ErpCodesNumber.ForFlex
            },
            // new()
            // {
            //     Name = MarketingNames.Flexen,
            //     FacebookName = Facebook.Flexen,
            //     GoogleName = GenericNames.Flexen,
            //     AccountingName = ERP_Accounting.Flexen,
            //     AccountingErpNumber = ErpCodesNumber.Flexen
            // },
            new()
            {
                Name = MarketingNames.ProstaRen,
                FacebookName = ProductConstants.Facebook.ProstaRen,
                GoogleName = GoogleMarketing.ProstaRen,
                GoogleNameErp = GoogleMarketingErp.ProstaRen,
                AccountingName = ERP_Accounting.ProstaRen,
                AccountingErpNumber = ErpCodesNumber.ProstaRen
            },
            new()
            {
                Name = MarketingNames.Sleep,
                FacebookName = ProductConstants.Facebook.Sleep,
                GoogleName = GoogleMarketing.Sleep,
                GoogleNameErp = GoogleMarketingErp.Sleep,
                AccountingName = ERP_Accounting.Sleep,
                AccountingErpNumber = ErpCodesNumber.Sleep
            },
            // new()
            // {
            //     Name = MarketingNames.Ceget,
            //     FacebookName = Facebook.Ceget,
            //     GoogleName = GenericNames.Ceget,
            //     AccountingName = ERP_Accounting.Ceget,
            //     AccountingErpNumber = ErpCodesNumber.Ceget
            // },
            // new()
            // {
            //     Name = MarketingNames.ViruFor,
            //     FacebookName = Facebook.ViruFor,
            //     GoogleName = GenericNames.ViruFor,
            //     AccountingName = ERP_Accounting.ViruFor,
            //     AccountingErpNumber = ErpCodesNumber.ViruFor
            // },
            new()
            {
                Name = MarketingNames.Botanic,
                FacebookName = ProductConstants.Facebook.GeneralAudience,
                GoogleName = GoogleMarketing.Botanic,
                GoogleNameErp = GenericNames.Botanic,
                AccountingName = ERP_Accounting.Botanic,
                AccountingErpNumber = ErpCodesNumber.Botanic
            }
        };
    
    private static IEnumerable<MarketingActivityDetails> GetMarketingActivityDetails() =>
        new List<MarketingActivityDetails>
        {
            new()
            {
                Name = "Facebook",
                Subject = "Задача / FACEBOOK IRELAND LIMITED",
                PartyId = "b21c6bc3-a4d8-43b9-a3df-b2d39ddf552f",
                Measure = "впечатления",
                Type = "Фейсбук",
                Media = "фейсбук",
                PublishType = "Facebook"
            },
            new()
            {
                Name = "Google",
                Subject = "Задача / GOOGLE IRELAND LIMITED",
                PartyId = "e5a6cfc4-d407-4424-a22e-d479136a28aa",
                Measure = "клик",
                Type = "google adwords",
                Media = "Google",
                PublishType = "Ad Words"
            }
        };
    
    private static IEnumerable<Currency> GetCurrencies() =>
        new List<Currency> { new() { Name = Euro, Value = 1.9894 }, };
}


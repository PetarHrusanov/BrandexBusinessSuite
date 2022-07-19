namespace BrandexBusinessSuite.OnlineShop.Data.Seeding;

using BrandexBusinessSuite.Services.Data;
using Models;

using static Common.ProductConstants;

public class ApplicationDbContextSeeder : ISeeder
{
    
    private readonly OnlineShopDbContext db;

    public ApplicationDbContextSeeder(OnlineShopDbContext db)
    {
        this.db = db;
    }
    
    public void SeedAsync()
    {
        if (db.Products.Any()) return;
        foreach (var product in GetProducts())
        {
            db.Products.Add(product);
        }
        db.SaveChanges();
    }

    private static IEnumerable<Product> GetProducts() =>
        new List<Product>
        {
            new()
            {
                Name = GenericNames.ZinSeD,
                ErpCode = ErpCodes.ZinSeD,
                ErpPriceCode = ErpPriceCodes.ZinSeD,
                ErpPriceNoVat = ErpPriceNoVat.ZinSeD,
                ErpLot = ErpLots.ZinSeD,
                WooCommerceName = OnlineShop.ZinSeD,
                WooCommerceSampleName = OnlineShopSamples.ZinSeD,
                ErpSampleCode = ErpSampleCodes.ZinSeD
            },
            new()
            {
                Name = GenericNames.EnzyMill,
                ErpCode = ErpCodes.EnzyMill,
                ErpPriceCode = ErpPriceCodes.EnzyMill,
                ErpPriceNoVat = ErpPriceNoVat.EnzyMill,
                ErpLot = ErpLots.EnzyMill,
                WooCommerceName = OnlineShop.EnzyMill,
                WooCommerceSampleName = OnlineShopSamples.EnzyMill,
                ErpSampleCode = ErpSampleCodes.EnzyMill
            },
            new()
            {
                Name = GenericNames.CystiRen,
                ErpCode = ErpCodes.CystiRen,
                ErpPriceCode = ErpPriceCodes.CystiRen,
                ErpPriceNoVat = ErpPriceNoVat.CystiRen,
                ErpLot = ErpLots.CystiRen,
                WooCommerceName = OnlineShop.CystiRen,
                WooCommerceSampleName = OnlineShopSamples.CystiRen,
                ErpSampleCode = ErpSampleCodes.CystiRen
            },
            new()
            {
                Name = GenericNames.LadyHarmonia,
                ErpCode = ErpCodes.LadyHarmonia,
                ErpPriceCode = ErpPriceCodes.LadyHarmonia,
                ErpPriceNoVat = ErpPriceNoVat.LadyHarmonia,
                ErpLot = ErpLots.LadyHarmonia,
                WooCommerceName = OnlineShop.LadyHarmonia,
                WooCommerceSampleName = OnlineShopSamples.LadyHarmonia,
                ErpSampleCode = ErpSampleCodes.LadyHarmonia
            },
            new()
            {
                Name = GenericNames.DetoxiFive,
                ErpCode = ErpCodes.DetoxiFive,
                ErpPriceCode = ErpPriceCodes.DetoxiFive,
                ErpPriceNoVat = ErpPriceNoVat.DetoxiFive,
                ErpLot = ErpLots.DetoxiFive,
                WooCommerceName = OnlineShop.DetoxiFive,
                WooCommerceSampleName = OnlineShopSamples.DetoxiFive,
                ErpSampleCode = ErpSampleCodes.DetoxiFive
            },
            new()
            {
                Name = GenericNames.LaxaL,
                ErpCode = ErpCodes.LaxaL,
                ErpPriceCode = ErpPriceCodes.LaxaL,
                ErpPriceNoVat = ErpPriceNoVat.LaxaL,
                ErpLot = ErpLots.LaxaL,
                WooCommerceName = OnlineShop.LaxaL,
                WooCommerceSampleName = OnlineShopSamples.LaxaL,
                ErpSampleCode = ErpSampleCodes.LaxaL
            },
            new()
            {
                Name = GenericNames.Bland,
                ErpCode = ErpCodes.Bland,
                ErpPriceCode = ErpPriceCodes.Bland,
                ErpPriceNoVat = ErpPriceNoVat.Bland,
                ErpLot = ErpLots.Bland,
                WooCommerceName = OnlineShop.Bland,
                WooCommerceSampleName = OnlineShopSamples.Bland,
                ErpSampleCode = ErpSampleCodes.Bland
            },
            new()
            {
                Name = GenericNames.DiabeForGluco,
                ErpCode = ErpCodes.DiabeForGluco,
                ErpPriceCode = ErpPriceCodes.DiabeForGluco,
                ErpPriceNoVat = ErpPriceNoVat.DiabeForGluco,
                ErpLot = ErpLots.DiabeForGluco,
                WooCommerceName = OnlineShop.DiabeForGluco,
                WooCommerceSampleName = OnlineShopSamples.DiabeForGluco,
                ErpSampleCode = ErpSampleCodes.DiabeForGluco
            },
            new()
            {
                Name = GenericNames.DiabeForProtect,
                ErpCode = ErpCodes.DiabeForProtect,
                ErpPriceCode = ErpPriceCodes.DiabeForProtect,
                ErpPriceNoVat = ErpPriceNoVat.DiabeForProtect,
                ErpLot = ErpLots.DiabeForProtect,
                WooCommerceName = OnlineShop.DiabeForProtect,
                WooCommerceSampleName = OnlineShopSamples.DiabeForProtect,
                ErpSampleCode = ErpSampleCodes.DiabeForProtect
            },
            new()
            {
                Name = GenericNames.GinkgoVin,
                ErpCode = ErpCodes.GinkgoVin,
                ErpPriceCode = ErpPriceCodes.GinkgoVin,
                ErpPriceNoVat = ErpPriceNoVat.GinkgoVin,
                ErpLot = ErpLots.GinkgoVin,
                WooCommerceName = OnlineShop.GinkgoVin,
                WooCommerceSampleName = OnlineShopSamples.GinkgoVin,
                ErpSampleCode = ErpSampleCodes.GinkgoVin
            },
            new()
            {
                Name = GenericNames.GinkgoVinCentella,
                ErpCode = ErpCodes.GinkgoVinCentella,
                ErpPriceCode = ErpPriceCodes.GinkgoVinCentella,
                ErpPriceNoVat = ErpPriceNoVat.GinkgoVinCentella,
                ErpLot = ErpLots.GinkgoVinCentella,
                WooCommerceName = OnlineShop.GinkgoVinCentella,
                WooCommerceSampleName = OnlineShopSamples.GinkgoVinCentella,
                ErpSampleCode = ErpSampleCodes.GinkgoVinCentella
            },
            new()
            {
                Name = GenericNames.Venaxin,
                ErpCode = ErpCodes.Venaxin,
                ErpPriceCode = ErpPriceCodes.Venaxin,
                ErpPriceNoVat = ErpPriceNoVat.Venaxin,
                ErpLot = ErpLots.Venaxin,
                WooCommerceName = OnlineShop.Venaxin,
                WooCommerceSampleName = OnlineShopSamples.Venaxin,
                ErpSampleCode = ErpSampleCodes.Venaxin
            },
            new()
            {
                Name = GenericNames.ForFlex,
                ErpCode = ErpCodes.ForFlex,
                ErpPriceCode = ErpPriceCodes.ForFlex,
                ErpPriceNoVat = ErpPriceNoVat.ForFlex,
                ErpLot = ErpLots.ForFlex,
                WooCommerceName = OnlineShop.ForFlex,
                WooCommerceSampleName = OnlineShopSamples.ForFlex,
                ErpSampleCode = ErpSampleCodes.ForFlex
            },
            new()
            {
                Name = GenericNames.Flexen,
                ErpCode = ErpCodes.Flexen,
                ErpPriceCode = ErpPriceCodes.Flexen,
                ErpPriceNoVat = ErpPriceNoVat.Flexen,
                ErpLot = ErpLots.Flexen,
                WooCommerceName = OnlineShop.Flexen,
                WooCommerceSampleName = OnlineShopSamples.Flexen,
                ErpSampleCode = ErpSampleCodes.Flexen
            },
            new()
            {
                Name = GenericNames.ProstaRen,
                ErpCode = ErpCodes.ProstaRen,
                ErpPriceCode = ErpPriceCodes.ProstaRen,
                ErpPriceNoVat = ErpPriceNoVat.ProstaRen,
                ErpLot = ErpLots.ProstaRen,
                WooCommerceName = OnlineShop.ProstaRen,
                WooCommerceSampleName = OnlineShopSamples.ProstaRen,
                ErpSampleCode = ErpSampleCodes.ProstaRen
            },
            new()
            {
                Name = GenericNames.Sleep,
                ErpCode = ErpCodes.Sleep,
                ErpPriceCode = ErpPriceCodes.Sleep,
                ErpPriceNoVat = ErpPriceNoVat.Sleep,
                ErpLot = ErpLots.Sleep,
                WooCommerceName = OnlineShop.Sleep,
                WooCommerceSampleName = OnlineShopSamples.Sleep,
                ErpSampleCode = ErpSampleCodes.Sleep
            },
            new()
            {
                Name = GenericNames.Ceget,
                ErpCode = ErpCodes.Ceget,
                ErpPriceCode = ErpPriceCodes.Ceget,
                ErpPriceNoVat = ErpPriceNoVat.Ceget,
                ErpLot = ErpLots.Ceget,
                WooCommerceName = OnlineShop.Ceget,
                WooCommerceSampleName = OnlineShopSamples.Ceget,
                ErpSampleCode = ErpSampleCodes.Ceget
            }
        };
}
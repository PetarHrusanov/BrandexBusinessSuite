namespace BrandexBusinessSuite.MarketingAnalysis.Data.Seeding;

using BrandexBusinessSuite.Services.Data;
using Models;

using static BrandexBusinessSuite.Common.ProductConstants;

public class ApplicationDbContextSeeder : ISeeder
{
    private readonly MarketingAnalysisDbContext db;

    public ApplicationDbContextSeeder(MarketingAnalysisDbContext db)
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
        
        if (db.MediaTypes.Any()) return;
        foreach (var mediaType in GetMediaTypes())
        {
            db.MediaTypes.Add(mediaType);
        }
        db.SaveChanges();
    }

    private static IEnumerable<Product> GetProducts() =>
        new List<Product>
        {
            new()
            {
                Name = GenericNames.ZinSeD,
                ShortName = ShortName.ZinSeD
            },
            new()
            {
                Name = GenericNames.EnzyMill,
                ShortName = ShortName.EnzyMill
            },
            new()
            {
                Name = GenericNames.CystiRen,
                ShortName = ShortName.CystiRen
            },
            new()
            {
                Name = GenericNames.LadyHarmonia,
                ShortName = ShortName.LadyHarmonia
            },
            new()
            {
                Name = GenericNames.DetoxiFive,
                ShortName = ShortName.DetoxiFive
            },
            new()
            {
                Name = GenericNames.LaxaL,
                ShortName = ShortName.LaxaL
            },
            new()
            {
                Name = GenericNames.Bland,
                ShortName = ShortName.Bland
            },
            new()
            {
                Name = GenericNames.DiabeForGluco,
                ShortName = ShortName.DiabeForGluco
            },
            new()
            {
                Name = GenericNames.DiabeForProtect,
                ShortName = ShortName.DiabeForProtect
            },
            new()
            {
                Name = GenericNames.GinkgoVin,
                ShortName = ShortName.GinkgoVin
            },
            new()
            {
                Name = GenericNames.GinkgoVinCentella,
                ShortName = ShortName.GinkgoVinCentella
            },
            new()
            {
                Name = GenericNames.Venaxin,
                ShortName = ShortName.Venaxin
            },
            new()
            {
                Name = GenericNames.ForFlex,
                ShortName = ShortName.ForFlex
            },
            new()
            {
                Name = GenericNames.Flexen,
                ShortName = ShortName.Flexen
            },
            new()
            {
                Name = GenericNames.ProstaRen,
                ShortName = ShortName.ProstaRen
            },
            new()
            {
                Name = GenericNames.Sleep,
                ShortName = ShortName.Sleep
            },
            new()
            {
                Name = GenericNames.Ceget,
                ShortName = ShortName.Ceget
            }
        };
    
    private static IEnumerable<MediaType> GetMediaTypes() =>
        new List<MediaType>
        {
            new() { Name = "Radio" },
            new() { Name = "Tv" },
            new() { Name = "Print" },
            new() { Name = "Facebook" },
            new() { Name = "Google" },
            new() { Name = "Digital" },
            new() { Name = "Pharmacies" },
        };
}


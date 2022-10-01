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
                Name = MarketingNames.ZinSeD,
                ShortName = ShortName.ZinSeD
            },
            new()
            {
                Name = MarketingNames.EnzyMill,
                ShortName = ShortName.EnzyMill
            },
            new()
            {
                Name = MarketingNames.CystiRen,
                ShortName = ShortName.CystiRen
            },
            new()
            {
                Name = MarketingNames.LadyHarmonia,
                ShortName = ShortName.LadyHarmonia
            },
            new()
            {
                Name = MarketingNames.DetoxiFive,
                ShortName = ShortName.DetoxiFive
            },
            new()
            {
                Name = MarketingNames.LaxaL,
                ShortName = ShortName.LaxaL
            },
            new()
            {
                Name = MarketingNames.Bland,
                ShortName = ShortName.Bland
            },
            new()
            {
                Name = MarketingNames.DiabeForGluco,
                ShortName = ShortName.DiabeForGluco
            },
            new()
            {
                Name = MarketingNames.DiabeForProtect,
                ShortName = ShortName.DiabeForProtect
            },
            new()
            {
                Name = MarketingNames.GinkgoVin,
                ShortName = ShortName.GinkgoVin
            },
            new()
            {
                Name = MarketingNames.GinkgoVinCentella,
                ShortName = ShortName.GinkgoVinCentella
            },
            new()
            {
                Name = MarketingNames.Venaxin,
                ShortName = ShortName.Venaxin
            },
            new()
            {
                Name = MarketingNames.ForFlex,
                ShortName = ShortName.ForFlex
            },
            new()
            {
                Name = MarketingNames.Flexen,
                ShortName = ShortName.Flexen
            },
            new()
            {
                Name = MarketingNames.ProstaRen,
                ShortName = ShortName.ProstaRen
            },
            new()
            {
                Name = MarketingNames.Sleep,
                ShortName = ShortName.Sleep
            },
            new()
            {
                Name = MarketingNames.Ceget,
                ShortName = ShortName.Ceget
            },
            new()
            {
                Name = MarketingNames.ViruFor,
                ShortName = ShortName.ViruFor
            },
            new()
            {
                Name = MarketingNames.Botanic,
                ShortName = ShortName.Botanic
            }
        };
    
    private static IEnumerable<MediaType> GetMediaTypes() =>
        new List<MediaType>
        {
            new() { Name = "Radio", NameBg = "радио"},
            new() { Name = "Tv", NameBg = "TV"},
            new() { Name = "Print", NameBg = "принт"},
            new() { Name = "Facebook", NameBg = "Facebook"},
            new() { Name = "Google", NameBg = "Google"},
            new() { Name = "Digital", NameBg = "интернет"},
            new() { Name = "Pharmacies", NameBg = "аптеки"},
        };
}


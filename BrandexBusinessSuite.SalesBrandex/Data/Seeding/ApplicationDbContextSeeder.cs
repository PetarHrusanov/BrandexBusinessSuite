namespace BrandexBusinessSuite.SalesBrandex.Data.Seeding;

using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Options;

using Models;

using BrandexBusinessSuite;
using BrandexBusinessSuite.Services.Data;

using static Common.ExcelDataConstants.Regions;
using static Common.ProductConstants;

public class ApplicationDbContextSeeder : ISeeder
{
    
    private readonly BrandexSalesAnalysisDbContext _db;

    public ApplicationDbContextSeeder(BrandexSalesAnalysisDbContext db) 
        => _db = db;

    public void SeedAsync()
    {
        if (!_db.Regions.Any())
        {
            foreach (var region in GetRegions())
            {
                _db.Regions.Add(region);
            }
            _db.SaveChanges();
        }
        
        if (!_db.Products.Any())
        {
            foreach (var product in GetProducts())
            {
                _db.Products.Add(product);
            }
            _db.SaveChanges();
        }

    }
    
    private static IEnumerable<Region> GetRegions()
            => new List<Region>
            {
                new() { Name = CentralOfficeSofia, ErpId = "kir"},
                new() { Name = Ruse , ErpId = "kir"},
                new() { Name = Varna , ErpId = "kir"},
                new() { Name = Burgas , ErpId = "kir"},
                new() { Name = StaraZagora , ErpId = "kir"},
                new() { Name = SofiaLiulin , ErpId = "kir"},
                new() { Name = SofiaMladost, ErpId = "kir" },
                new() { Name = SofiaKrasnoSelo , ErpId = "kir"},
                new() { Name = SofiaDrujba , ErpId = "kir"},
                new() { Name = Plovdiv , ErpId = "kir"},
                new() { Name = Pleven , ErpId = "kir"},
                new() { Name = Vidin , ErpId = "kir"},
                new() { Name = Unserviced , ErpId = "kir"},
                new() { Name = Common.ExcelDataConstants.Regions.OnlineShop , ErpId = "kir"},
            };
    
    private static IEnumerable<Product> GetProducts() =>
        new List<Product>
        {
            new()
            {
                Name = ERP_Accounting.ZinSeD,
                ErpId = ErpCodes.ZinSeD,
                ShortName = ShortName.ZinSeD,
                BrandexId = BrandexId.ZinSeD,
                Price = PriceNoVat.ZinSeD,
            },
            new()
            {
                Name = ERP_Accounting.EnzyMill,
                ErpId = ErpCodes.EnzyMill,
                ShortName = ShortName.EnzyMill,
                BrandexId = BrandexId.EnzyMill,
                Price = PriceNoVat.EnzyMill,
            },
            new()
            {
                Name = ERP_Accounting.EnzyMillCompact,
                ErpId = ErpCodes.EnzyMillCompact,
                ShortName = ShortName.EnzyMillCompact,
                BrandexId = BrandexId.EnzyMillCompact,
                Price = PriceNoVat.EnzyMillCompact,
            },
            new()
            {
                Name = ERP_Accounting.CystiRen,
                ErpId = ErpCodes.CystiRen,
                ShortName = ShortName.CystiRen,
                BrandexId = BrandexId.CystiRen,
                Price = PriceNoVat.CystiRen,
            },
            new()
            {
                Name = ERP_Accounting.LadyHarmonia,
                ErpId = ErpCodes.LadyHarmonia,
                ShortName = ShortName.LadyHarmonia,
                BrandexId = BrandexId.LadyHarmonia,
                Price = PriceNoVat.LadyHarmonia,
            },
            new()
            {
                Name = ERP_Accounting.DetoxiFive,
                ErpId = ErpCodes.DetoxiFive,
                ShortName = ShortName.DetoxiFive,
                BrandexId = BrandexId.DetoxiFive,
                Price = PriceNoVat.DetoxiFive,
            },
            new()
            {
                Name = ERP_Accounting.LaxaL,
                ErpId = ErpCodes.LaxaL,
                ShortName = ShortName.LaxaL,
                BrandexId = BrandexId.LaxaL,
                Price = PriceNoVat.LaxaL,
            },
            new()
            {
                Name = ERP_Accounting.Bland,
                ErpId = ErpCodes.Bland,
                ShortName = ShortName.Bland,
                BrandexId = BrandexId.Bland,
                Price = PriceNoVat.Bland,
            },
            new()
            {
                Name = ERP_Accounting.DiabeForGluco,
                ErpId = ErpCodes.DiabeForGluco,
                ShortName = ShortName.DiabeForGluco,
                BrandexId = BrandexId.DiabeForGluco,
                Price = PriceNoVat.DiabeForGluco,
            },
            new()
            {
                Name = ERP_Accounting.DiabeForProtect,
                ErpId = ErpCodes.DiabeForProtect,
                ShortName = ShortName.DiabeForProtect,
                BrandexId = BrandexId.DiabeForProtect,
                Price = PriceNoVat.DiabeForProtect,
            },
            new()
            {
                Name = ERP_Accounting.GinkgoVin,
                ErpId = ErpCodes.GinkgoVin,
                ShortName = ShortName.GinkgoVin,
                BrandexId = BrandexId.GinkgoVin,
                Price = PriceNoVat.GinkgoVin,
            },
            new()
            {
                Name = ERP_Accounting.GinkgoVinCentella,
                ErpId = ErpCodes.GinkgoVinCentella,
                ShortName = ShortName.GinkgoVinCentella,
                BrandexId = BrandexId.GinkgoVinCentella,
                Price = PriceNoVat.GinkgoVinCentella,
            },
            new()
            {
                Name = ERP_Accounting.Venaxin,
                ErpId = ErpCodes.Venaxin,
                ShortName = ShortName.Venaxin,
                BrandexId = BrandexId.Venaxin,
                Price = PriceNoVat.Venaxin,
            },
            new()
            {
                Name = ERP_Accounting.ForFlex,
                ErpId = ErpCodes.ForFlex,
                ShortName = ShortName.ForFlex,
                BrandexId = BrandexId.ForFlex,
                Price = PriceNoVat.ForFlex,
            },
            new()
            {
                Name = ERP_Accounting.Flexen,
                ErpId = ErpCodes.Flexen,
                ShortName = ShortName.Flexen,
                BrandexId = BrandexId.Flexen,
                Price = PriceNoVat.Flexen,
            },
            new()
            {
                Name = ERP_Accounting.ProstaRen,
                ErpId = ErpCodes.ProstaRen,
                ShortName = ShortName.ProstaRen,
                BrandexId = BrandexId.ProstaRen,
                Price = PriceNoVat.ProstaRen,
            },
            new()
            {
                Name = ERP_Accounting.Sleep,
                ErpId = ErpCodes.Sleep,
                ShortName = ShortName.Sleep,
                BrandexId = BrandexId.Sleep,
                Price = PriceNoVat.Sleep,
            },
            new()
            {
                Name = ERP_Accounting.Ceget,
                ErpId = ErpCodes.Ceget,
                ShortName = ShortName.Ceget,
                BrandexId = BrandexId.Ceget,
                Price = PriceNoVat.Ceget,
            },
            new()
            {
                Name = ERP_Accounting.ViruFor,
                ErpId = ErpCodes.ViruFor,
                ShortName = ShortName.ViruFor,
                BrandexId = BrandexId.ViruFor,
                Price = PriceNoVat.ViruFor,
            },
        };
}
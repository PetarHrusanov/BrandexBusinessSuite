namespace BrandexBusinessSuite.SalesAnalysis.Services.Products;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
    
using Data;
using SalesAnalysis.Data.Models;
using SalesAnalysis.Models.Products;
    
using static Common.ExcelDataConstants.Ditributors;

public class ProductsService : IProductsService
{
    SpravkiDbContext db;

    public ProductsService(SpravkiDbContext db)
    {
        this.db = db;
    }

    public async Task<string> CreateProduct(ProductInputModel productInputModel)
    {
        if (productInputModel.BrandexId == 0 || productInputModel.Name == null || productInputModel.ShortName == null ||
            productInputModel.Price == 0) return "";
        var productDBModel = new Product
        {
            Name = productInputModel.Name,
            Price = productInputModel.Price,
            ShortName = productInputModel.ShortName,
            BrandexId = productInputModel.BrandexId,
            PharmnetId = productInputModel.PharmnetId,
            PhoenixId = productInputModel.PhoenixId,
            SopharmaId = productInputModel.SopharmaId,
            StingId = productInputModel.StingId
        };

        await db.Products.AddAsync(productDBModel);
        await db.SaveChangesAsync();
        return productDBModel.Name;

    }

    public async Task<List<ProductCheckModel>> GetProductsCheck()
    {
        return await db.Products.Select(p => new ProductCheckModel
        {
            Id = p.Id,
            BrandexId = p.BrandexId,
            PhoenixId = p.PhoenixId,
            PharmnetId = p.PharmnetId,
            StingId = p.StingId,
            SopharmaId = p.SopharmaId
        }).ToListAsync();
    }
    

    public async Task<string> NameById(string input, string distributor)
    {
        var unused = int.TryParse(input, out var convertedNumber);

        return distributor switch
        {
            Brandex => await db.Products.Where(c => c.BrandexId == convertedNumber)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(),
            Sting => await db.Products.Where(c => c.StingId == convertedNumber)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(),
            Phoenix => await db.Products.Where(c => c.PhoenixId == convertedNumber)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(),
            Pharmnet => await db.Products.Where(c => c.PharmnetId == convertedNumber)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(),
            Sopharma => await db.Products.Where(c => c.SopharmaId == input)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(),
            _ => ""
        };
    }

    public async Task<IEnumerable<string>> GetProductsNames()
    {
        return await db.Products.Select(p => p.Name).ToListAsync();
    }

    public async Task<IEnumerable<int>> GetProductsId()
    {
        return await db.Products.Select(p => p.Id).ToListAsync();
    }

    public async Task<IEnumerable<ProductShortOutputModel>> GetProductsIdPrices()
    {
        return await db.Products.Select(p => new ProductShortOutputModel
        {
            Name = p.Name,
            Id = p.Id,
            Price = p.Price
        }).ToListAsync();
    }
}
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using EFCore.BulkExtensions;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Products;

using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using MarketingAnalysis.Models.Products;
using Data;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class ProductsService :IProductsService
{
    
    private readonly MarketingAnalysisDbContext _db;
    public ProductsService(MarketingAnalysisDbContext db) => _db = db;

    public async Task UploadBulk(List<ProductInputModel> products)
    {
        var entities = products.Select(activity => new Product()
        {
            Name = activity.Name,
            ShortName = activity.ShortName,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();

        await _db.BulkInsertAsync(entities);
    }

    public async Task Upload(ProductInputModel inputModel)
    { 
        await _db.Products.AddAsync(new Product 
        { 
            Name = inputModel.Name.ToUpper().TrimEnd(),
            ShortName = inputModel.ShortName
        });
        await _db.SaveChangesAsync();
    }

    public async Task<List<ProductCheckModel>> GetCheckModels() 
        => await _db.Products.Select(p => new ProductCheckModel() 
        { 
            Id = p.Id, 
            Name = p.Name, 
            ShortName = p.ShortName 
        }).ToListAsync();

}
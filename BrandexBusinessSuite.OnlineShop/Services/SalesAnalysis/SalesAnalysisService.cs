namespace BrandexBusinessSuite.OnlineShop.Services.SalesAnalysis;

using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using BrandexBusinessSuite.OnlineShop.Data.Models;
using Models;
using Data;

public class SalesAnalysisService :ISalesAnalysisService
{
    private readonly OnlineShopDbContext _db;
    public SalesAnalysisService(OnlineShopDbContext db) 
        =>  _db = db;
    
    public async Task UploadBulk(List<SalesOnlineAnalysisInput> salesAnalysis)
    {
        
        var orders = salesAnalysis.Select(o => new SaleOnlineAnalysis
        {
            OrderNumber = o.OrderNumber,
            Date = o.Date,
            ProductId = o.ProductId,
            Quantity = o.Quantity,
            Total = o.Total,
            City = o.City,
            Sample = o.Sample ?? string.Empty,
            AdSource = o.AdSource ?? string.Empty

        }).ToList();
        
        await _db.BulkInsertAsync(orders);
    }

    public async Task<List<SaleOnlineAnalysis>> GetCheckModelsByDate(DateTime date)
        => await _db.SaleOnline.Where(d=>d.Date<=date).ToListAsync();
}
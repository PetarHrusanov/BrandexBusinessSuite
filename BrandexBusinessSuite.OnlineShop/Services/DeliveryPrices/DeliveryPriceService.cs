using BrandexBusinessSuite.OnlineShop.Data;
using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.OnlineShop.Services.DeliveryPrices;

public class DeliveryPriceService :IDeliveryPriceService
{
    private OnlineShopDbContext _db;
    
    public DeliveryPriceService(OnlineShopDbContext db)
    {
        _db = db;
    }
    
    public async Task<DeliveryPrice> GetDeliveryPrice()
    {
        return (await _db.DeliveryPrices.FirstOrDefaultAsync())!;
    }
}
namespace BrandexBusinessSuite.OnlineShop.Services.DeliveryPrices;

using BrandexBusinessSuite.OnlineShop.Data;
using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore;

public class DeliveryPriceService :IDeliveryPriceService
{
    private readonly OnlineShopDbContext _db;
    
    public DeliveryPriceService(OnlineShopDbContext db) 
        => _db = db;

    public async Task<DeliveryPrice> GetDeliveryPrice()
        => (await _db.DeliveryPrices.FirstOrDefaultAsync())!;

    public async Task EditDeliveryPrice(DeliveryPrice deliveryPriceEdit)
    {
        _db.DeliveryPrices.Update(deliveryPriceEdit);
        await _db.SaveChangesAsync();
    }
}
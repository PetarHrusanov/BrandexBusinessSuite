namespace BrandexBusinessSuite.OnlineShop.Services.DeliveryPrices;

using BrandexBusinessSuite.OnlineShop.Data;
using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task EditDeliveryPrice(DeliveryPrice deliveryPriceEdit)
    {
        var deliveryPrice = await _db.DeliveryPrices.Where(d => d.Id == deliveryPriceEdit.Id).FirstOrDefaultAsync();
        deliveryPrice.ErpId = deliveryPriceEdit.ErpId;
        deliveryPrice.ErpPriceId = deliveryPriceEdit.ErpPriceId;
        deliveryPrice.Price = deliveryPriceEdit.Price;

        await _db.SaveChangesAsync();
    }
}
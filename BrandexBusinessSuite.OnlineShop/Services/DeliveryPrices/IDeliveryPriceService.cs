using BrandexBusinessSuite.OnlineShop.Data.Models;

namespace BrandexBusinessSuite.OnlineShop.Services.DeliveryPrices;

public interface IDeliveryPriceService
{
    Task<DeliveryPrice> GetDeliveryPrice();
    
    Task EditDeliveryPrice(DeliveryPrice deliveryPrice);
}
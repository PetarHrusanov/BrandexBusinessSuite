using BrandexBusinessSuite.Inventory.Data;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Inventory.Models.Orders;

namespace BrandexBusinessSuite.Inventory.Services.Orders;

public class OrdersService :IOrdersService
{
    private readonly InventoryDbContext _db;

    public OrdersService(InventoryDbContext db)
    {
        _db = db;
    }

    
    public async Task Upload(OrderInputModel inputModel)
    {
        var order = new Order()
        {
            MaterialId = inputModel.MaterialId,
            SupplierId = inputModel.SupplierId,
            Quantity = inputModel.Quantity,
            Price = inputModel.Price,
            OrderDate = inputModel.OrderDate,
            Delivered = inputModel.Delivered
        };
        
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }
}
using BrandexBusinessSuite.Inventory.Data;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Inventory.Models.Orders;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<MaterialsQuantitiesOutputModel>> GetLatest()
    {

        var materials = await _db.Materials.Select(m => m.Id).ToListAsync();

        var materialsList = new List<MaterialsQuantitiesOutputModel>();

        foreach (var material in materials)
        {
            var order = await _db.Orders.OrderBy(o => o.OrderDate).Where(o=>o.MaterialId==material).Select(o =>
                new MaterialsQuantitiesOutputModel()
                {
                    MaterialName = o.Material.Name,
                    MaterialErpId = o.Material.ErpId,
                    SupplierName = o.Supplier.Name,
                    Price = o.Price,
                    QuantityOrdered = o.Quantity,
                    PriceQuantity = o.Price/o.Quantity,
                    DateOrdered = o.OrderDate.ToString("yyyy-MM-dd"),
                    Delivered = o.Delivered
                }).FirstOrDefaultAsync();
            
            if (order!=null) materialsList.Add(order!);
        }

        return materialsList;
    }
}
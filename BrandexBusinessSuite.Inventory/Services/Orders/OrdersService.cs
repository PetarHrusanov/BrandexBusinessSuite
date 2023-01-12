namespace BrandexBusinessSuite.Inventory.Services.Orders;

using Microsoft.EntityFrameworkCore;

using AutoMapper;

using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Inventory.Models.Orders;
using Data;

public class OrdersService :IOrdersService
{
    private readonly InventoryDbContext _db;
    private readonly IMapper _mapper;

    public OrdersService(InventoryDbContext db, IMapper mapper)
    {
        _mapper = mapper;
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
            Notes = inputModel.Notes ?? string.Empty,
            OrderDate = inputModel.OrderDate,
            DeliveryDate = inputModel.DeliveryDate,
        };
        
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id==id);
        _db.Orders.Remove(order!);
        await _db.SaveChangesAsync();
    }

    public async Task<OrderEditModel> GetOrder(int id)
        => (await _mapper.ProjectTo<OrderEditModel>(_db.Orders.Where(m => m.Id == id))
            .FirstOrDefaultAsync())!;
    

    public async Task<OrderEditModel> Edit(OrderEditModel inputModel)
    {
        var order = await _db.Orders.Where(m => m.Id == inputModel.Id).FirstOrDefaultAsync();
        order.MaterialId = inputModel.MaterialId;
        order.SupplierId = inputModel.SupplierId;
        order.Quantity = inputModel.Quantity;
        order.Price = inputModel.Price;
        order.OrderDate = inputModel.OrderDate;
        order.Notes = inputModel.Notes;
        order.DeliveryDate = inputModel.DeliveryDate;
        await _db.SaveChangesAsync();
        return inputModel;
    }

    public async Task DeliverOrder(int id)
    {
        var order = await _db.Orders.Where(m => m.Id == id).FirstOrDefaultAsync();
        order!.DeliveryDate = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task<List<MaterialsQuantitiesOutputModel>> GetLatest()
    {

        var materials = await _db.Materials.Select(m => m.Id).ToListAsync();

        var materialsList = new List<MaterialsQuantitiesOutputModel>();

        foreach (var material in materials)
        {
            var order = await _db.Orders.OrderByDescending(o => o.OrderDate).Where(o=>o.MaterialId==material).Select(o =>
                new MaterialsQuantitiesOutputModel
                {
                    MaterialName = o.Material.Name,
                    MaterialErpId = o.Material.ErpId,
                    SupplierName = o.Supplier.Name,
                    Price = o.Price,
                    Notes = o.Notes,
                    QuantityOrdered = o.Quantity,
                    PriceQuantity = o.Price/o.Quantity,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    DeliveryDate = o.DeliveryDate != null ? o.DeliveryDate.Value.ToString("yyyy-MM-dd") : null,
                    Delivered = o.DeliveryDate != null ? true:false
                }).FirstOrDefaultAsync();
            
            if (order!=null) materialsList.Add(order!);
        }

        return materialsList;
    }

    public async Task<List<OrderOutputModel>> GetSpecificOrders(int ordersNumber, int? materialId)
    {

        if (materialId!=null)
        {
            return await _mapper.ProjectTo<OrderOutputModel>(_db.Orders.OrderByDescending(o=>o.OrderDate).Where(m => m.MaterialId == materialId)).Take(ordersNumber)
                .ToListAsync();
        }
        
        return await _mapper.ProjectTo<OrderOutputModel>(_db.Orders.OrderByDescending(o=>o.OrderDate).Take(ordersNumber))
            .ToListAsync();
    }

    public async Task<List<OrderOutputModel>> GetUndelivered() 
        => await _mapper.ProjectTo<OrderOutputModel>(_db.Orders.Where(m => m.DeliveryDate == null))
            .ToListAsync();
}
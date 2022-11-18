using BrandexBusinessSuite.Inventory.Data;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Inventory.Models.Suppliers;
using BrandexBusinessSuite.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.Inventory.Services.Suppliers;

public class SuppliersService :ISuppliersService
{
    private readonly InventoryDbContext _db;

    public SuppliersService(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task<List<BasicCheckModel>> GetAll()
        => await _db.Suppliers.Select(s => new BasicCheckModel()
        {
            Name = s.Name,
            Id = s.Id
        }).ToListAsync();

    public async Task Upload(SupplierInputModel supplier)
    {
        var dbModel = new Supplier()
        {
            Name = supplier.Name,
            Contact = supplier.Contact,
            VAT = supplier.VAT,
        };

        await _db.Suppliers.AddAsync(dbModel);
        await _db.SaveChangesAsync();
    }
}
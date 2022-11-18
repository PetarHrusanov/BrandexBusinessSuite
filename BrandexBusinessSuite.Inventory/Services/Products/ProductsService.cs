namespace BrandexBusinessSuite.Inventory.Services.Products;

using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

using Data;

using static  Common.Constants;
using static  Common.ExcelDataConstants.Generic;


public class ProductsService : IProductsService
{
    private readonly InventoryDbContext _db;
    private readonly IConfiguration _configuration;

    public ProductsService(InventoryDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
    public async Task<List<BasicCheckErpModel>> GetProductsCheck() 
        => await _db.Products.Select(p => new BasicCheckErpModel
        { 
            Id = p.Id, 
            Name = p.Name, 
            ErpId = p.ErpId 
        }).ToListAsync();

    public async Task UploadBulk(List<ErpProduct> products)
    {
        var table = new DataTable();
        table.TableName = "Products";
            
        table.Columns.Add(Name);
        table.Columns.Add(ErpId);
        table.Columns.Add(PartNumber);
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var product in products)
        {
            var row = table.NewRow();
            row[Name] = product.Name.BG;
            row[ErpId] = product.Id;
            row[PartNumber] = product.PartNumber;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
        
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = "Products";
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        objbulk.ColumnMappings.Add(PartNumber, PartNumber);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
    }
    
}
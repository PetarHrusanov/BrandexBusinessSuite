using NPOI.HPSF;
using Array = System.Array;

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

    private const string Pills = "Pills";

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

    public async Task UploadBulk(IEnumerable<ErpProduct> products, int pills)
    {
        var table = new DataTable();
        table.TableName = "Products";

        var dataColumns = new DataColumn[]
        {
            new (Name),
            new (ErpId),
            new (PartNumber),
            new (Pills),
            new (CreatedOn),
            new (IsDeleted, typeof(bool)),
        };
        
        table.Columns.AddRange(dataColumns);

        foreach (var values in products.Select(product => new object[] { product.Name.BG.TrimEnd(), product.Id, product.PartNumber, pills, DateTime.Now, false }))
        {
            table.LoadDataRow(values, true);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = "Products";

        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        objbulk.ColumnMappings.Add(PartNumber, PartNumber);
        objbulk.ColumnMappings.Add(Pills, Pills);

        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
    }
    
}
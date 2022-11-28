using BrandexBusinessSuite.MarketingAnalysis.Data.Models;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Products;

using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using MarketingAnalysis.Models.Products;
using Data;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class ProductsService :IProductsService
{
    
    private MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    
    public ProductsService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
    public async Task UploadBulk(List<ProductInputModel> products)
    {
        var table = new DataTable();
        table.TableName = Products;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(ShortName, typeof(string));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var product in products)
        {
            var row = table.NewRow();
            row[Name] = product.Name;
            row[ShortName] = product.ShortName;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        string connection = _configuration.GetConnectionString(DefaultConnection);
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = Products;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ShortName, ShortName);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();
    }

    public async Task Upload(ProductInputModel inputModel)
    {
        var product = new Product()
        {
            Name = inputModel.Name!.ToUpper().TrimEnd(),
            ShortName = inputModel.ShortName!
        };

        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ProductCheckModel>> GetCheckModels()
    {
        return await _db.Products.Select(p => new ProductCheckModel()
        {
            Id = p.Id,
            Name = p.Name,
            ShortName = p.ShortName
        }).ToListAsync();
    }
    
}
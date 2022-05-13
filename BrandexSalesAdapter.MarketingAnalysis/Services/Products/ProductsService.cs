namespace BrandexSalesAdapter.MarketingAnalysis.Services.Products;

using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using BrandexSalesAdapter.MarketingAnalysis.Models.Products;
using BrandexSalesAdapter.MarketingAnalysis.Data;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class ProductsService :IProductsService
{
    
    private MarketingAnalysisDbContext db;
    private readonly IConfiguration _configuration;
    
    public ProductsService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        this.db = db;
        _configuration = configuration;
    }
    
    public async Task UploadBulk(List<ProductInputModel> products)
    {
        var table = new DataTable();
        table.TableName = Products;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(ShortName, typeof(string));
        
        table.Columns.Add(MediaType, typeof(int));
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

    public Task<string> CreateProduct(ProductInputModel productInputModel)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ProductCheckModel>> GetCheckModels()
    {
        return await db.Products.Select(p => new ProductCheckModel()
        {
            Id = p.Id,
            Name = p.Name,
            ShortName = p.ShortName
        }).ToListAsync();
    }

    public Task<string> NameById(string input, string distributor)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetProductsNames()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<int>> GetProductsId()
    {
        throw new NotImplementedException();
    }
}
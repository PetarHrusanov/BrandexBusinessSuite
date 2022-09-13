namespace BrandexBusinessSuite.OnlineShop.Services.SalesAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using BrandexBusinessSuite.OnlineShop.Data.Models;
using Models;
using System.Data;
using Data;

public class SalesAnalysisService :ISalesAnalysisService
{

    OnlineShopDbContext _db;
    private readonly IConfiguration _configuration;

    private const string SaleOnline = "SaleOnline";
    
    private const string OrderNumber = "OrderNumber";
    private const string Date = "Date";
    private const string ProductId = "ProductId";
    private const string Quantity = "Quantity";
    private const string Total = "Total";
    private const string City = "City";
    private const string Sample = "Sample";
    private const string AdSource = "AdSource";
    

    public SalesAnalysisService(OnlineShopDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task UploadBulk(List<SalesOnlineAnalysisInput> salesAnalysis)
    {
        var table = new DataTable();
        table.TableName = SaleOnline;
        
        var columnNames = new[]{OrderNumber, Date, ProductId, Quantity, Total, City, Sample, AdSource};

        foreach (var column in columnNames)
        {
            table.Columns.Add(column);
        }

        foreach (var sale in salesAnalysis)
        {
            var row = table.NewRow();
            row[OrderNumber] = sale.OrderNumber;
            row[Date] = sale.Date;
            row[ProductId] = sale.ProductId;
            row[Quantity] = sale.Quantity;
            row[Total] = sale.Total;
            row[City] = sale.City;
            row[Sample] = string.Empty;
            row[AdSource] = string.Empty;
            
            if (sale.AdSource!=null)
            {
                row[AdSource] = sale.AdSource;
            }
            
            if (sale.Sample!=null)
            {
                row[Sample] = sale.Sample;
            }

            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = SaleOnline;
        
        foreach (var column in columnNames)
        {
            objbulk.ColumnMappings.Add(column, column);
        }

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close(); 
    }

    public async Task<List<SaleOnlineAnalysis>> GetCheckModelsByDate(DateTime date)
        => await _db.SaleOnline.Where(d=>d.Date<=date).ToListAsync();
    
    
}
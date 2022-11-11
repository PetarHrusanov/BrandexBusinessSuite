namespace BrandexBusinessSuite.SalesAnalysis.Services.Sales;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using Data;
using Models.Sales;
    
using Microsoft.Extensions.Configuration;

using static Common.ExcelDataConstants.SalesColumns;
using static Common.ExcelDataConstants.Generic;
using static Common.Constants;

public class SalesService :ISalesService
{
    private readonly SalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public SalesService(SalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task UploadBulk(List<SaleInputModel> sales)
    {
        var table = new DataTable();
        table.TableName = Sales;

        table.Columns.Add(PharmacyId, typeof(string));
        table.Columns.Add(ProductId, typeof(int));
        table.Columns.Add(DistributorId, typeof(int));
        table.Columns.Add(Date, typeof(DateTime));
        table.Columns.Add(Count, typeof(int));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var sale in sales)
        {
            var row = table.NewRow();
            row[PharmacyId] = sale.PharmacyId;
            row[ProductId] = sale.ProductId;
            row[DistributorId] = sale.DistributorId;
            row[Date] = sale.Date;
            row[Count] = sale.Count;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;

            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = "Sales";
            
        objbulk.ColumnMappings.Add(PharmacyId, PharmacyId);   
        objbulk.ColumnMappings.Add(ProductId, ProductId);  
        objbulk.ColumnMappings.Add(DistributorId, DistributorId);  
        objbulk.ColumnMappings.Add(Date, Date);
        objbulk.ColumnMappings.Add(Count, Count);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);
    
        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
            
    }


    public async Task<int> ProductCountSumByIdDate(int productId, DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    {

        dateBegin ??= DateTime.MinValue;
        dateEnd ??= DateTime.MaxValue;
            
        if (regionId != null)
        {
            return await _db.Sales
                .Where(p => p.Pharmacy.RegionId == regionId)
                .Where(d => d.Date >= dateBegin && d.Date <= dateEnd)
                .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
        }
            
        return await _db.Sales
            .Where(d => d.Date >= dateBegin && d.Date <= dateEnd)
            .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
            
    }

    public async Task<List<DateTime>> GetDistinctDatesByMonths()
    {
        var datesRough = await this._db.Sales.Select(s => s.Date).Distinct().ToListAsync();
        var dates = datesRough.Select(t => new DateTime(t.Year, t.Month, 1)).Distinct().ToList();
        return dates;
    }
}
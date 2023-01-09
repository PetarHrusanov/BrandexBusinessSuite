using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.Models.Dates;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.SalesBrandex.Services.Sales;

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Data;
using Models.Sales;

using static Common.ExcelDataConstants.SalesColumns;
using static Common.ExcelDataConstants.Generic;
using static Common.Constants;

public class SalesService :ISalesService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public SalesService(BrandexSalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task UploadBulk(List<SaleDbInputModel> sales)
    {
        var table = new DataTable();
        table.TableName = Sales;
        
        table.Columns.Add(ErpId);
        table.Columns.Add(PharmacyId, typeof(string));
        table.Columns.Add(ProductId, typeof(int));
        table.Columns.Add(Date, typeof(DateTime));
        table.Columns.Add(Count, typeof(int));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var sale in sales)
        {
            var row = table.NewRow();
            
            row[ErpId] = sale.ErpId;
            row[PharmacyId] = sale.PharmacyId;
            row[ProductId] = sale.ProductId;
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
        
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        objbulk.ColumnMappings.Add(PharmacyId, PharmacyId);
        objbulk.ColumnMappings.Add(ProductId, ProductId);
        objbulk.ColumnMappings.Add(Date, Date);
        objbulk.ColumnMappings.Add(Count, Count);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);
    
        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
            
    }

    public async Task<List<string>> QuickCheckListErpIdByDates(DateStartEndInputModel dateStartEndInputModel)
        => await _db.Sales
            .Where(d => d.Date >= dateStartEndInputModel.DateStart && d.Date <= dateStartEndInputModel.DateEnd)
            .Select(d => d.ErpId).ToListAsync();

    public async Task<List<ProductQuantitiesOutputModel>> AverageSales()
    {
        var curDate = DateTime.Now;
        var startDate = curDate.AddMonths(-3).AddDays(1 - curDate.Day);
        var endDate = startDate.AddMonths(3).AddDays(-1);

        var sales = await _db.Sales
            .Where(s=>s.Count>0)
            .Where(s => s.Date.Date >= startDate.Date && s.Date.Date <= endDate.Date)
            .Select(s=> new { 
                ProductName = s.Product.Name, 
                ProductErp = s.Product.ErpId, 
                Count = s.Count 
            }).ToListAsync();

        var salesGrouped = new List<ProductQuantitiesOutputModel>();
        
        foreach (var sale in sales)
        {
            if (salesGrouped.All(s => s.ErpId != sale.ProductErp))
            {
                var saleGroup = new ProductQuantitiesOutputModel
                {
                    Name = sale.ProductName,
                    ErpId = sale.ProductErp,
                    Quantity = 0
                };
                salesGrouped.Add(saleGroup);
            }
            var productQuantity = salesGrouped.FirstOrDefault(p => p.ErpId == sale.ProductErp);
            productQuantity!.Quantity += sale.Count;
        }
        
        salesGrouped = salesGrouped.Select(item => new ProductQuantitiesOutputModel()
        {
            Name = item.Name,
            ErpId = item.ErpId,
            Quantity = item.Quantity / 3
        }).ToList();
        

        return salesGrouped;
    }
}
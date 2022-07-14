namespace BrandexBusinessSuite.SalesAnalysis.Services.Sales;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using Data;
using Data.Models;
using Models.Sales;
    
using Microsoft.Extensions.Configuration;
    
    
using static BrandexBusinessSuite.Common.ExcelDataConstants.Ditributors;
using static BrandexBusinessSuite.Common.ExcelDataConstants.SalesColumns;
using static BrandexBusinessSuite.Common.Constants;


public class SalesService :ISalesService
{
    SpravkiDbContext db;
    private readonly IConfiguration _configuration;

    public SalesService(SpravkiDbContext db, IConfiguration configuration)
    {
        this.db = db;
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

        string connection = _configuration.GetConnectionString("DefaultConnection");
            
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

    public async Task CreateSale(SaleInputModel sale, string distributor)
    {
        var distributorId = await db.Distributors
            .Where(n => n.Name == distributor)
            .Select(i => i.Id)
            .FirstOrDefaultAsync();

        if (sale.PharmacyId != 0
            && sale.ProductId != 0
            && sale.Date != null
            && sale.Count != 0
            && distributorId != 0)
        { 

            var saleDbModel = new Sale
            {
                PharmacyId = sale.PharmacyId,
                ProductId = sale.ProductId,
                Date = sale.Date,
                Count = sale.Count,
                DistributorId = distributorId
            };

            await db.Sales.AddAsync(saleDbModel);
            await db.SaveChangesAsync();
        }
    }

    public async Task<bool> UploadIndividualSale(string pharmacyId, string productId, string date, int count, string distributor)
    {
        DateTime dateForDb = DateTime.ParseExact(date, "dd-MM-yyyy", null);

        bool successProduct = int.TryParse(productId, out var convertedProductId);

        bool successPharmacy = int.TryParse(pharmacyId, out var convertedPharmacyId);

        var newSale = new Sale();

            
        switch (distributor)
        {
            case Brandex:
                if (await db.Products.Where(c => c.BrandexId == convertedProductId).AnyAsync()
                    && await db.Pharmacies.Where(c => c.BrandexId == convertedPharmacyId).AnyAsync())
                {
                    newSale.ProductId = await db.Products
                        .Where(c => c.BrandexId == convertedProductId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    newSale.PharmacyId = await db.Pharmacies
                        .Where(c => c.BrandexId == convertedPharmacyId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    newSale.Date = dateForDb;
                    newSale.Count = count;
                    newSale.DistributorId = await db.Distributors
                        .Where(d => d.Name == distributor)
                        .Select(d => d.Id)
                        .FirstOrDefaultAsync();

                    await db.Sales.AddAsync(newSale);
                    await db.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }

            case Sting:
                if (await db.Products.Where(c => c.StingId == convertedProductId).AnyAsync()
                    && await db.Pharmacies.Where(c => c.StingId == convertedPharmacyId).AnyAsync())
                {
                    newSale.ProductId = await db.Products
                        .Where(c => c.StingId == convertedProductId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    newSale.PharmacyId = await db.Pharmacies
                        .Where(c => c.StingId == convertedPharmacyId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    newSale.Date = dateForDb;
                    newSale.Count = count;
                    newSale.DistributorId = await db.Distributors
                        .Where(d => d.Name == distributor)
                        .Select(d => d.Id)
                        .FirstOrDefaultAsync();

                    await db.Sales.AddAsync(newSale);
                    await db.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }

            case Phoenix:
                if (await db.Products.Where(c => c.PhoenixId == convertedProductId).AnyAsync()
                    && await db.Pharmacies.Where(c => c.PhoenixId == convertedPharmacyId).AnyAsync())
                {
                    newSale.ProductId = await db.Products
                        .Where(c => c.PhoenixId == convertedProductId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    newSale.PharmacyId = await db.Pharmacies
                        .Where(c => c.PhoenixId == convertedPharmacyId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    newSale.Date = dateForDb;

                    newSale.Count = count;

                    newSale.DistributorId = await db.Distributors
                        .Where(d => d.Name == distributor)
                        .Select(d => d.Id)
                        .FirstOrDefaultAsync();

                    await db.Sales.AddAsync(newSale);
                    await db.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }

            case Pharmnet:
                if (await db.Products.Where(c => c.PharmnetId == convertedProductId).AnyAsync()
                    && await db.Pharmacies.Where(c => c.PharmnetId == convertedPharmacyId).AnyAsync())
                {
                    newSale.ProductId = await db.Products
                        .Where(c => c.PharmnetId == convertedProductId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    newSale.PharmacyId = await db.Pharmacies
                        .Where(c => c.PharmnetId == convertedPharmacyId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    newSale.Date = dateForDb;

                    newSale.Count = count;

                    newSale.DistributorId = await db.Distributors
                        .Where(d => d.Name == distributor)
                        .Select(d => d.Id)
                        .FirstOrDefaultAsync();

                    await db.Sales.AddAsync(newSale);
                    await db.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }

            case Sopharma:
                if (await db.Products.Where(c => c.SopharmaId == productId).AnyAsync()
                    && await db.Pharmacies.Where(c => c.SopharmaId == convertedPharmacyId).AnyAsync())
                {
                    newSale.ProductId = await db.Products
                        .Where(c => c.SopharmaId == productId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    newSale.PharmacyId = await db.Pharmacies
                        .Where(c => c.SopharmaId == convertedPharmacyId)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    newSale.Date = dateForDb;

                    newSale.Count = count;

                    newSale.DistributorId = await db.Distributors
                        .Where(d => d.Name == distributor)
                        .Select(d => d.Id)
                        .FirstOrDefaultAsync();

                    await db.Sales.AddAsync(newSale);
                    await db.SaveChangesAsync();

                    return true;
                }
                else return false;

            default:
                return false;
        };

    }

    public async Task<int> ProductCountSumById(int productId, int? regionId=null)
    {
        
        if (regionId != null)
        {
            return await this.db.Sales
                .Where(p => p.Pharmacy.RegionId == regionId)
                .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
        }
        return await this.db.Sales.Where(p => p.ProductId == productId).SumAsync(c => c.Count);

    }

    public async Task<int> ProductCountSumByIdDate(int productId, DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    {

        dateBegin ??= DateTime.MinValue;
        dateEnd ??= DateTime.MaxValue;
            
        if (regionId != null)
        {
            return await db.Sales
                .Where(p => p.Pharmacy.RegionId == regionId)
                .Where(d => d.Date >= dateBegin && d.Date <= dateEnd)
                .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
        }
            
        return await db.Sales
            .Where(d => d.Date >= dateBegin && d.Date <= dateEnd)
            .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
            
    }

    public async Task<List<DateTime>> GetDistinctDatesByMonths()
    {
        var datesRough = await this.db.Sales.Select(s => s.Date).Distinct().ToListAsync();
        var dates = datesRough.Select(t => new DateTime(t.Year, t.Month, 1)).Distinct().ToList();
        return dates;
    }
}
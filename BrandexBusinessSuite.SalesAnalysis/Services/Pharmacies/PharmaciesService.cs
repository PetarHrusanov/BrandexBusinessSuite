namespace BrandexBusinessSuite.SalesAnalysis.Services.Pharmacies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
    
using System.Data;
using Data;
using BrandexBusinessSuite.Models.DataModels;

using Models.Pharmacies;
using Models.Sales;

using static Common.ExcelDataConstants.PharmacyColumns;
using static Common.ExcelDataConstants.Generic;
using static  Common.Constants;

using static Methods.DataMethods;

public class PharmaciesService : IPharmaciesService
{
    private readonly SalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public PharmaciesService(SalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task UploadBulk(List<PharmacyDbInputModel> pharmacies)
    {
        var table = new DataTable();
        table.TableName = Pharmacies;
            
        table.Columns.Add(BrandexId);
        table.Columns.Add(Name);
        table.Columns.Add(PharmacyClass, typeof(int));
        table.Columns.Add(Active, typeof(bool));
        table.Columns.Add(CompanyId);
        table.Columns.Add(PharmacyChainId);
        table.Columns.Add(Address);
        table.Columns.Add(CityId);
        table.Columns.Add(PharmnetId);
        table.Columns.Add(PhoenixId);
        table.Columns.Add(SopharmaId);
        table.Columns.Add(StingId);
        table.Columns.Add(RegionId);
        
        table.Columns.Add(ErpId);
            
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var pharmacy in pharmacies)
        {
            var row = table.NewRow();
            row[BrandexId] = pharmacy.BrandexId;
            row[Name] = pharmacy.Name;
            row[PharmacyClass] = pharmacy.PharmacyClass;
            row[Active] = pharmacy.Active;
            row[CompanyId] = pharmacy.CompanyId;
            row[PharmacyChainId] = pharmacy.PharmacyChainId;
            row[Address] = pharmacy.Address;
            row[CityId] = pharmacy.CityId;

            row[PharmnetId] = pharmacy.PharmnetId;
            row[PhoenixId] = pharmacy.PhoenixId;
            row[SopharmaId] = pharmacy.SopharmaId;
            row[StingId] = pharmacy.StingId;
                
            row[RegionId] = pharmacy.RegionId;
            
            row[ErpId] = pharmacy.ErpId;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = Pharmacies;
            
        objbulk.ColumnMappings.Add(BrandexId, BrandexId);
        objbulk.ColumnMappings.Add(Name, Name); 
        objbulk.ColumnMappings.Add(PharmacyClass, PharmacyClass); 
        objbulk.ColumnMappings.Add(Active, Active); 
        objbulk.ColumnMappings.Add(CompanyId, CompanyId); 
        objbulk.ColumnMappings.Add(PharmacyChainId, PharmacyChainId); 
        objbulk.ColumnMappings.Add(Address, Address); 
        objbulk.ColumnMappings.Add(CityId, CityId); 
        objbulk.ColumnMappings.Add(PharmnetId, PharmnetId);
        objbulk.ColumnMappings.Add(PhoenixId, PhoenixId); 
        objbulk.ColumnMappings.Add(SopharmaId, SopharmaId); 
        objbulk.ColumnMappings.Add(StingId, StingId); 
        objbulk.ColumnMappings.Add(RegionId, RegionId); 
        
        objbulk.ColumnMappings.Add(ErpId, ErpId); 
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);
            
        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close(); 
            
    }

    public async Task<List<PharmacyCheckErpModel>> GetAllCheckErp()
    {
        return await _db.Pharmacies.Select(p => new PharmacyCheckErpModel
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId,
            Address = p.Address,
            
            RegionErp = p.Region.ErpId,
            PharmacyChainErp = p.PharmacyChain.ErpId,
            
            IsActive = p.Active,
            CompanyIdErp = p.Company.ErpId,
            
            PhoenixId = p.PhoenixId,
            PharmnetId = p.PharmnetId,
            StingId = p.StingId,
            SopharmaId = p.SopharmaId
        }).ToListAsync();
        
    }

    public async Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    {
        if (regionId!=null)
        {
            return await _db.Pharmacies
                .Where(p => p.RegionId == regionId)
                .Select(p => new PharmacyExcelModel
                {
                    Name = p.Name,
                    Address = p.Address,
                    PharmacyClass = p.PharmacyClass,
                    Region = p.Region.Name,
                    Sales = p.Sales
                        .Where(d => d.Date >= dateBegin &&  d.Date<=dateEnd)
                        .Select(s => new SaleExcelOutputModel
                        {
                            Name = s.Product.Name,
                            ProductId = s.ProductId,
                            Count = s.Count,
                            ProductPrice = s.Product.Price
                            // Date = date
                        }).ToList()
                }).ToListAsync();
        }

        return await _db.Pharmacies.Select(p => new PharmacyExcelModel
        {
            Name = p.Name,
            Address = p.Address,
            PharmacyClass = p.PharmacyClass,
            Region = p.Region.Name,
            Sales = p.Sales
                .Where(d => d.Date >= dateBegin &&  d.Date<=dateEnd)
                .Select(s => new SaleExcelOutputModel
                {
                    Name = s.Product.Name,
                    ProductId = s.ProductId,
                    Count = s.Count,
                    ProductPrice = s.Product.Price
                }).ToList()
        }).ToListAsync();

    }

    public async Task Update(List<PharmacyDbInputModel> pharmacies)
    {
        foreach (var pharmacy in pharmacies)
        {
            var pharmacyDb = await _db.Pharmacies.Where(p => p.BrandexId == pharmacy.BrandexId)
                .FirstOrDefaultAsync();

            pharmacyDb.Name = pharmacy.Name;
            pharmacyDb.PharmacyClass = pharmacy.PharmacyClass;
            pharmacyDb.Active = pharmacy.Active;
            pharmacyDb.CompanyId  = pharmacy.CompanyId;
            pharmacyDb.PharmacyChainId  = pharmacy.PharmacyChainId;
            pharmacyDb.Address  = pharmacy.Address;
            pharmacyDb.CityId  = pharmacy.CityId;
            pharmacyDb.PharmnetId  = pharmacy.PharmnetId;
            pharmacyDb.PhoenixId  = pharmacy.PhoenixId;
            pharmacyDb.SopharmaId  = pharmacy.SopharmaId;
            pharmacyDb.StingId  = pharmacy.StingId;
            pharmacyDb.RegionId  = pharmacy.RegionId;
                
            await _db.SaveChangesAsync();
        }
    }

    public async Task BulkUpdateData(List<PharmacyDbUpdateModel> list)
    {
        var dt = ConvertToDataTable(list);
        
        var connection = _configuration.GetConnectionString("DefaultConnection");

        await using var conn = new SqlConnection(connection);
        await using var command = new SqlCommand("CREATE TABLE #TmpTable(Id int NOT NULL,ErpId nvarchar(50) NOT NULL, Name nvarchar(400) NOT NULL)", conn);
        try
        {
            conn.Open();
            command.ExecuteNonQuery();

            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.BulkCopyTimeout = 6600;
                bulkCopy.DestinationTableName = "#TmpTable";
                await bulkCopy.WriteToServerAsync(dt);
                bulkCopy.Close();
            }

            command.CommandTimeout = 3000;
            command.CommandText = $"UPDATE P SET P.[ErpId]= T.[ErpId] FROM [{Pharmacies}] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id] ;DROP TABLE #TmpTable;";
            command.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Handle exception properly
        }
        finally
        {
            conn.Close();
        }
    }

    public async Task<List<PharmacyCheckModel>> GetAllCheck()
    {
        return await _db.Pharmacies.Select(p => new PharmacyCheckModel
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId,
            BrandexId = p.BrandexId,
            PhoenixId = p.PhoenixId,
            PharmnetId = p.PharmnetId,
            StingId = p.StingId,
            SopharmaId = p.SopharmaId
        }).ToListAsync();
    }
    
}
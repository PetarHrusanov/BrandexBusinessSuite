using BrandexBusinessSuite.SalesAnalysis.Data.Models;
using EFCore.BulkExtensions;

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
        
        var entities = pharmacies.Select(o => new Pharmacy
        {
            BrandexId = o.BrandexId,
            Name = o.Name,
            PharmacyClass = o.PharmacyClass,
            Active = o.Active,
            CompanyId = o.CompanyId,
            PharmacyChainId = o.PharmacyChainId,
            Address = o.Address,
            CityId = o.CityId,
            PharmnetId = o.PharmnetId,
            PhoenixId = o.PhoenixId,
            SopharmaId = o.SopharmaId,
            StingId = o.StingId,
            RegionId = o.RegionId,
            ErpId = o.ErpId,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();
        
        await _db.BulkInsertAsync(entities);
    }

    public async Task<List<PharmacyCheckErpModel>> GetAllCheckErp()
    {
        return await _db.Pharmacies.Select(p => new PharmacyCheckErpModel
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId,
            Address = p.Address,
            
            RegionId = p.RegionId,
            RegionErp = p.Region.ErpId,
            
            PharmacyChainId = p.PharmacyChainId,
            PharmacyChainErp = p.PharmacyChain.ErpId,
            
            IsActive = p.Active,
            
            CompanyId = p.CompanyId,
            CompanyIdErp = p.Company.ErpId,
            
            PhoenixId = p.PhoenixId,
            PharmnetId = p.PharmnetId,
            StingId = p.StingId,
            SopharmaId = p.SopharmaId
        }).ToListAsync();
        
    }

    public async Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    {
        var pharmacies = _db.Pharmacies.AsQueryable();
        if (regionId != null) pharmacies = pharmacies.Where(p => p.RegionId == regionId);
        
        return await pharmacies.Select(p => new PharmacyExcelModel 
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

    public async Task BulkUpdateData(List<PharmacyDbUpdateModel> list)
    {
        var dt = ConvertToDataTable(list);
        
        var connection = _configuration.GetConnectionString("DefaultConnection");

        await using var conn = new SqlConnection(connection);
        await using var command = new SqlCommand("CREATE TABLE #TmpTable(Id int NOT NULL, Name nvarchar(400) NOT NULL, CompanyId int NOT NULL, PharmacyChainId int NOT NULL, Address nvarchar(max) NOT NULL, PharmnetId int, PhoenixId int, SopharmaId int, StingId int, RegionId int NOT NULL, ModifiedOn datetime2)", conn);
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
            command.CommandText = $"UPDATE P SET P.[Name]= T.[Name], P.[CompanyId]= T.[CompanyId], P.[PharmacyChainId]= T.[PharmacyChainId], P.[Address]= T.[Address], P.[PharmnetId]= T.[PharmnetId], P.[PhoenixId]= T.[PhoenixId], P.[SopharmaId]= T.[SopharmaId], P.[StingId]= T.[StingId], P.[RegionId]= T.[RegionId], P.[ModifiedOn]= T.[ModifiedOn] FROM [{Pharmacies}] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id] ;DROP TABLE #TmpTable;";
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
namespace BrandexBusinessSuite.SalesBrandex.Services.Pharmacies;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
    
using System.Data;
using Data;

using Models.Pharmacies;

using static Common.ExcelDataConstants.PharmacyColumns;
using static  Common.Constants;
using static Common.ExcelDataConstants.Generic;

public class PharmaciesService : IPharmaciesService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PharmaciesService(BrandexSalesAnalysisDbContext db, IConfiguration configuration, IMapper mapper)
    {
        _db = db;
        _configuration = configuration;
        _mapper = mapper;

    }

    public async Task UploadBulk(List<PharmacyDbInputModel> pharmacies)
    {
        var table = new DataTable();
        table.TableName = Pharmacies;
            
        table.Columns.Add(BrandexId);
        table.Columns.Add(ErpId);
        table.Columns.Add(Name);
        table.Columns.Add(PartyCode);
        table.Columns.Add(PharmacyClass, typeof(int));
        table.Columns.Add(CompanyId);
        table.Columns.Add(PharmacyChainId);
        table.Columns.Add(Address);
        table.Columns.Add(CityId);
        table.Columns.Add(RegionId);
            
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var pharmacy in pharmacies)
        {
            var row = table.NewRow();
            row[BrandexId] = pharmacy.BrandexId;
            row[ErpId] = pharmacy.ErpId;
            row[Name] = pharmacy.Name;
            row[PartyCode] = pharmacy.PartyCode;
            row[PharmacyClass] = pharmacy.PharmacyClass;
            row[CompanyId] = pharmacy.CompanyId;
            row[PharmacyChainId] = pharmacy.PharmacyChainId;
            row[Address] = pharmacy.Address;
            row[CityId] = pharmacy.CityId;
            row[RegionId] = pharmacy.RegionId;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }
            
            
            
        var connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = Pharmacies;
            
        objbulk.ColumnMappings.Add(BrandexId, BrandexId);
        objbulk.ColumnMappings.Add(ErpId, ErpId); 
        objbulk.ColumnMappings.Add(Name, Name); 
        objbulk.ColumnMappings.Add(PartyCode, PartyCode); 
        objbulk.ColumnMappings.Add(PharmacyClass, PharmacyClass);
        objbulk.ColumnMappings.Add(CompanyId, CompanyId); 
        objbulk.ColumnMappings.Add(PharmacyChainId, PharmacyChainId); 
        objbulk.ColumnMappings.Add(Address, Address); 
        objbulk.ColumnMappings.Add(CityId, CityId);
        objbulk.ColumnMappings.Add(RegionId, RegionId); 
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);
            
        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close(); 
            
    }
    
    public async Task<List<PharmacyCheckModel>> GetPharmaciesCheck() 
        => await _mapper.ProjectTo<PharmacyCheckModel>(_db.Pharmacies).ToListAsync();
    

    // public async Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    // {
    //     if (regionId!=null)
    //     {
    //         return await db.Pharmacies
    //             .Where(p => p.RegionId == regionId)
    //             .Select(p => new PharmacyExcelModel
    //             {
    //                 Name = p.Name,
    //                 Address = p.Address,
    //                 PharmacyClass = p.PharmacyClass,
    //                 Region = p.Region.Name,
    //                 Sales = p.Sales
    //                     .Where(d => d.Date >= dateBegin &&  d.Date<=dateEnd)
    //                     .Select(s => new SaleExcelOutputModel
    //                     {
    //                         Name = s.Product.Name,
    //                         ProductId = s.ProductId,
    //                         Count = s.Count,
    //                         ProductPrice = s.Product.Price
    //                         // Date = date
    //                     }).ToList()
    //             }).ToListAsync();
    //     }
    //
    //     return await db.Pharmacies.Select(p => new PharmacyExcelModel
    //     {
    //         Name = p.Name,
    //         Address = p.Address,
    //         PharmacyClass = p.PharmacyClass,
    //         Region = p.Region.Name,
    //         Sales = p.Sales
    //             .Where(d => d.Date >= dateBegin &&  d.Date<=dateEnd)
    //             .Select(s => new SaleExcelOutputModel
    //             {
    //                 Name = s.Product.Name,
    //                 ProductId = s.ProductId,
    //                 Count = s.Count,
    //                 ProductPrice = s.Product.Price
    //             }).ToList()
    //     }).ToListAsync();
    //
    // }
    //
    // public async Task Update(List<PharmacyDbInputModel> pharmacies)
    // {
    //     foreach (var pharmacy in pharmacies)
    //     {
    //         var pharmacyDb = await db.Pharmacies.Where(p => p.BrandexId == pharmacy.BrandexId)
    //             .FirstOrDefaultAsync();
    //
    //         pharmacyDb.Name = pharmacy.Name;
    //         pharmacyDb.PharmacyClass = pharmacy.PharmacyClass;
    //         pharmacyDb.Active = pharmacy.Active;
    //         pharmacyDb.CompanyId  = pharmacy.CompanyId;
    //         pharmacyDb.PharmacyChainId  = pharmacy.PharmacyChainId;
    //         pharmacyDb.Address  = pharmacy.Address;
    //         pharmacyDb.CityId  = pharmacy.CityId;
    //         pharmacyDb.PharmnetId  = pharmacy.PharmnetId;
    //         pharmacyDb.PhoenixId  = pharmacy.PhoenixId;
    //         pharmacyDb.SopharmaId  = pharmacy.SopharmaId;
    //         pharmacyDb.StingId  = pharmacy.StingId;
    //         pharmacyDb.RegionId  = pharmacy.RegionId;
    //             
    //         await db.SaveChangesAsync();
    //     }
    // }

}
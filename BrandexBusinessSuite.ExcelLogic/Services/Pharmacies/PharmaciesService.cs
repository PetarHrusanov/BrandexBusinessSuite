namespace BrandexBusinessSuite.ExcelLogic.Services.Pharmacies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
    
using System.Data;
using Data;
using Data.Models;

using Models.Pharmacies;
using Models.Sales;

using static BrandexBusinessSuite.Common.ExcelDataConstants.Ditributors;
using static BrandexBusinessSuite.Common.ExcelDataConstants.PharmacyColumns;
using static  BrandexBusinessSuite.Common.Constants;

public class PharmaciesService : IPharmaciesService
{
    SpravkiDbContext db;
    private readonly IConfiguration _configuration;

    public PharmaciesService(SpravkiDbContext db, IConfiguration configuration)
    {
        this.db = db;
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
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }
            
            
            
        string connection = _configuration.GetConnectionString("DefaultConnection");
            
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
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);
            
        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close(); 
            
    }

    public async Task<string> CreatePharmacy(PharmacyDbInputModel pharmacyDbInputModel)
    {
        if(pharmacyDbInputModel.BrandexId!=0 &&
           pharmacyDbInputModel.Name!=null &&
           pharmacyDbInputModel.PharmacyChainId!= 0 &&
           pharmacyDbInputModel.RegionId!= 0 &&
           pharmacyDbInputModel.CityId!= 0 &&
           pharmacyDbInputModel.CompanyId !=0)
        {
            var pharmacyModel = new Pharmacy
            {
                BrandexId = pharmacyDbInputModel.BrandexId,
                Name = pharmacyDbInputModel.Name,
                Address = pharmacyDbInputModel.Address,
                PharmacyChainId = pharmacyDbInputModel.PharmacyChainId,
                Active = pharmacyDbInputModel.Active,
                CityId = pharmacyDbInputModel.CityId,
                SopharmaId = pharmacyDbInputModel.SopharmaId,
                StingId = pharmacyDbInputModel.StingId,
                PhoenixId = pharmacyDbInputModel.PhoenixId,
                PharmnetId = pharmacyDbInputModel.PharmnetId,
                CompanyId = pharmacyDbInputModel.CompanyId,
                PharmacyClass = pharmacyDbInputModel.PharmacyClass,
                RegionId = pharmacyDbInputModel.RegionId

            };

            await this.db.Pharmacies.AddAsync(pharmacyModel);
            await this.db.SaveChangesAsync();
            return pharmacyModel.Name;
        }
        else
        {
            return "";
        }
    }


    public async Task<string> NameById(string input, string distributor)
    {
        var success = int.TryParse(input, out var convertedNumber);

        switch (distributor)
        {
            case Brandex:
                return await db.Pharmacies.Where(c => c.BrandexId == convertedNumber).Select(p => p.Name)
                    .FirstOrDefaultAsync();
            case Sting:
                return await db.Pharmacies.Where(c => c.StingId == convertedNumber).Select(p => p.Name)
                    .FirstOrDefaultAsync();
            case Phoenix:
                return await db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Name)
                    .FirstOrDefaultAsync();
            case Pharmnet:
                return await db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Name)
                    .FirstOrDefaultAsync();
            case Sopharma:
                return await db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).Select(p => p.Name)
                    .FirstOrDefaultAsync();
            default:
                return "";
        }

        ;
    }

    public async Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    {
        if (regionId!=null)
        {
            return await db.Pharmacies
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

        return await db.Pharmacies.Select(p => new PharmacyExcelModel
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

    public async Task Update(List<PharmacyDbInputModel> pharmacies)
    {
        foreach (var pharmacy in pharmacies)
        {
            var pharmacyDb = await db.Pharmacies.Where(p => p.BrandexId == pharmacy.BrandexId)
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
                
            await db.SaveChangesAsync();
        }
    }
        

    public async Task<List<PharmacyCheckModel>> GetPharmaciesCheck()
    {
        return await this.db.Pharmacies.Select(p => new PharmacyCheckModel
        {
            Id = p.Id,
            BrandexId = p.BrandexId,
            PhoenixId = p.PhoenixId,
            PharmnetId = p.PharmnetId,
            StingId = p.StingId,
            SopharmaId = p.SopharmaId
        }).ToListAsync();

    }
        
    // public async Task<bool> CheckPharmacyByDistributor(string input, string distributor)
    // {
    //     int convertedNumber = int.Parse(input);
    //     switch (distributor)
    //     {
    //         case Brandex:
    //             return await db.Pharmacies.Where(c => c.BrandexId == convertedNumber).AnyAsync();
    //         case Sting:
    //             return await db.Pharmacies.Where(c => c.StingId == convertedNumber).AnyAsync();
    //         case Phoenix:
    //             return await db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).AnyAsync();
    //         case Pharmnet:
    //             return await db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).AnyAsync();
    //         case Sopharma:
    //             return await db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).AnyAsync();
    //         default:
    //             return false;
    //     }
    // }
        
    // public async Task<int> PharmacyIdByDistributor(string input, string distributor)
    // {
    //     int convertedNumber = int.Parse(input);
    //     switch (distributor)
    //     {
    //         case Brandex:
    //             return await db.Pharmacies.Where(c => c.BrandexId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
    //         case Sting:
    //             return await db.Pharmacies.Where(c => c.StingId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
    //         case Phoenix:
    //             return await db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
    //         case Pharmnet:
    //             return await db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
    //         case Sopharma:
    //             return await db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
    //         default:
    //             return 0;
    //     };
    // }
        
    // public async Task<ICollection<PharmacyDistributorCheck>> PharmacyIdsByDistributorForCheck(string distributor)
    // {
    //     switch (distributor)
    //     {
    //         case Brandex:
    //             return await this.db.Pharmacies.Where(c => c.BrandexId != null).Select(p => new PharmacyDistributorCheck
    //             {
    //                 PharmacyId = p.Id,
    //                 DistributorId = (int)p.BrandexId
    //             }).ToListAsync();
    //         case Sting:
    //             return await this.db.Pharmacies.Where(c => c.StingId != null).Select(p => new PharmacyDistributorCheck
    //             {
    //                 PharmacyId = p.Id,
    //                 DistributorId = (int)p.StingId
    //             }).ToListAsync();
    //         case Phoenix:
    //             return await this.db.Pharmacies.Where(c => c.PhoenixId != null).Select(p => new PharmacyDistributorCheck
    //             {
    //                 PharmacyId = p.Id,
    //                 DistributorId = (int)p.PhoenixId
    //             }).ToListAsync();
    //         case Pharmnet:
    //             return await this.db.Pharmacies.Where(c => c.PharmnetId != null).Select(p => new PharmacyDistributorCheck
    //             {
    //                 PharmacyId = p.Id,
    //                 DistributorId = (int)p.PharmnetId
    //             }).ToListAsync();
    //         case Sopharma:
    //             return await this.db.Pharmacies.Where(c => c.SopharmaId != null).Select(p => new PharmacyDistributorCheck
    //             {
    //                 PharmacyId = p.Id,
    //                 DistributorId = (int)p.SopharmaId
    //             }).ToListAsync();
    //         default:
    //             return new List<PharmacyDistributorCheck>();
    //     };
    // }
}
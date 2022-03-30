namespace BrandexSalesAdapter.ExcelLogic.Services.Pharmacies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using static Common.DataConstants.Ditributors;

    public class PharmaciesService :IPharmaciesService
    {
        SpravkiDbContext db;

        public PharmaciesService(SpravkiDbContext db)
        {
            this.db = db;
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

        public async Task<bool> CheckPharmacyByDistributor(string input, string distributor)
        {
            int convertedNumber = int.Parse(input);
            switch (distributor)
            {
                case Brandex:
                    return await db.Pharmacies.Where(c => c.BrandexId == convertedNumber).AnyAsync();
                case Sting:
                    return await db.Pharmacies.Where(c => c.StingId == convertedNumber).AnyAsync();
                case Phoenix:
                    return await db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).AnyAsync();
                case Pharmnet:
                    return await db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).AnyAsync();
                case Sopharma:
                    return await db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).AnyAsync();
                default:
                    return false;
            }
        }

        public async Task<int> PharmacyIdByDistributor(string input, string distributor)
        {
                int convertedNumber = int.Parse(input);
                switch (distributor)
                {
                    case Brandex:
                        return await db.Pharmacies.Where(c => c.BrandexId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Sting:
                        return await db.Pharmacies.Where(c => c.StingId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Phoenix:
                        return await db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Pharmnet:
                        return await db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Sopharma:
                        return await db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    default:
                        return 0;
                };
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

        public async Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime date, int? regionId)
        {
            if (date != null && regionId!=null)
            {
                //int filterRegionId = (int)regionId;
                //var currRowDate = DateTime.ParseExact(date, "MM/yyyy", null);

                return await db.Pharmacies
                    .Where(p => p.RegionId == regionId)
                    .Select(p => new PharmacyExcelModel
                {
                    Name = p.Name,
                    Address = p.Address,
                    PharmacyClass = p.PharmacyClass,
                    Region = p.Region.Name,
                    Sales = p.Sales
                            .Where(d => d.Date.Month == date.Month && d.Date.Year == date.Year)
                            .Select(s => new SaleExcelOutputModel
                            {
                                Name = s.Product.Name,
                                ProductId = s.ProductId,
                                Count = s.Count,
                                Date = date
                            }).ToList()
                }).ToListAsync();
            }

            else
            {
                //var currRowDate = DateTime.ParseExact(date, "MM/yyyy", null);

                return await db.Pharmacies.Select(p => new PharmacyExcelModel
                {
                    Name = p.Name,
                    Address = p.Address,
                    PharmacyClass = p.PharmacyClass,
                    Region = p.Region.Name,
                    Sales = p.Sales
                            .Where(d => d.Date.Month == date.Month && d.Date.Year == date.Year)
                            .Select(s => new SaleExcelOutputModel
                            {
                                Name = s.Product.Name,
                                ProductId = s.ProductId,
                                Count = s.Count,
                                Date = date
                            }).ToList()
                }).ToListAsync();
            }
            
        }

        public async Task<ICollection<PharmacyDistributorCheck>> PharmacyIdsByDistributorForCheck(string distributor)
        {
            switch (distributor)
            {
                case Brandex:
                    return await this.db.Pharmacies.Where(c => c.BrandexId != null).Select(p => new PharmacyDistributorCheck
                    {
                        PharmacyId = p.Id,
                        DistributorId = (int)p.BrandexId
                    }).ToListAsync();
                case Sting:
                    return await this.db.Pharmacies.Where(c => c.StingId != null).Select(p => new PharmacyDistributorCheck
                    {
                        PharmacyId = p.Id,
                        DistributorId = (int)p.StingId
                    }).ToListAsync();
                case Phoenix:
                    return await this.db.Pharmacies.Where(c => c.PhoenixId != null).Select(p => new PharmacyDistributorCheck
                    {
                        PharmacyId = p.Id,
                        DistributorId = (int)p.PhoenixId
                    }).ToListAsync();
                case Pharmnet:
                    return await this.db.Pharmacies.Where(c => c.PharmnetId != null).Select(p => new PharmacyDistributorCheck
                    {
                        PharmacyId = p.Id,
                        DistributorId = (int)p.PharmnetId
                    }).ToListAsync();
                case Sopharma:
                    return await this.db.Pharmacies.Where(c => c.SopharmaId != null).Select(p => new PharmacyDistributorCheck
                    {
                        PharmacyId = p.Id,
                        DistributorId = (int)p.SopharmaId
                    }).ToListAsync();
                default:
                    return new List<PharmacyDistributorCheck>();
            };
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
    }
}

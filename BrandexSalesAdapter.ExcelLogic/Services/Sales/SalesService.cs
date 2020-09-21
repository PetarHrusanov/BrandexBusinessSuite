namespace BrandexSalesAdapter.ExcelLogic.Services.Sales
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Sales;
    using static Common.DataConstants.Ditributors;

    public class SalesService :ISalesService
    {
        SpravkiDbContext db;

        public SalesService(SpravkiDbContext db)
        {
            this.db = db;
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

            int convertedProductId;
            bool successProduct = int.TryParse(productId, out convertedProductId);

            int convertedPharmacyId;
            bool successPharmacy = int.TryParse(pharmacyId, out convertedPharmacyId);

            var newSale = new Sale();

            
            switch (distributor)
            {
                case Brandex:
                    if (await this.db.Products.Where(c => c.BrandexId == convertedProductId).AnyAsync()
                        && await this.db.Pharmacies.Where(c=> c.BrandexId == convertedPharmacyId).AnyAsync())
                    {
                        newSale.ProductId = await this.db.Products
                            .Where(c => c.BrandexId == convertedProductId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();
                        newSale.PharmacyId = await this.db.Pharmacies
                            .Where(c => c.BrandexId == convertedPharmacyId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();
                        newSale.Date = dateForDb;
                        newSale.Count = count;
                        newSale.DistributorId = await this.db.Distributors
                            .Where(d => d.Name == distributor)
                            .Select(d => d.Id)
                            .FirstOrDefaultAsync();

                        await this.db.Sales.AddAsync(newSale);
                        await this.db.SaveChangesAsync();

                        return true;
                    }
                    else return false;

                case Sting:
                    if (await this.db.Products.Where(c => c.StingId == convertedProductId).AnyAsync()
                        && await this.db.Pharmacies.Where(c => c.StingId == convertedPharmacyId).AnyAsync())
                    {
                        newSale.ProductId = await this.db.Products
                            .Where(c => c.StingId == convertedProductId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();
                        newSale.PharmacyId = await this.db.Pharmacies
                            .Where(c => c.StingId == convertedPharmacyId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();
                        newSale.Date = dateForDb;
                        newSale.Count = count;
                        newSale.DistributorId = await this.db.Distributors
                            .Where(d => d.Name == distributor)
                            .Select(d => d.Id)
                            .FirstOrDefaultAsync();

                        await this.db.Sales.AddAsync(newSale);
                        await this.db.SaveChangesAsync();

                        return true;
                    }
                    else return false;

                case Phoenix:
                    if (await this.db.Products.Where(c => c.PhoenixId == convertedProductId).AnyAsync()
                        && await this.db.Pharmacies.Where(c => c.PhoenixId == convertedPharmacyId).AnyAsync())
                    {
                        newSale.ProductId = await this.db.Products
                            .Where(c => c.PhoenixId == convertedProductId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        newSale.PharmacyId = await this.db.Pharmacies
                            .Where(c => c.PhoenixId == convertedPharmacyId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        newSale.Date = dateForDb;

                        newSale.Count = count;

                        newSale.DistributorId = await this.db.Distributors
                            .Where(d => d.Name == distributor)
                            .Select(d => d.Id)
                            .FirstOrDefaultAsync();

                        await this.db.Sales.AddAsync(newSale);
                        await this.db.SaveChangesAsync();

                        return true;
                    }
                    else return false;

                case Pharmnet:
                    if (await this.db.Products.Where(c => c.PharmnetId == convertedProductId).AnyAsync()
                        && await this.db.Pharmacies.Where(c => c.PharmnetId == convertedPharmacyId).AnyAsync())
                    {
                        newSale.ProductId = await this.db.Products
                            .Where(c => c.PharmnetId == convertedProductId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        newSale.PharmacyId = await this.db.Pharmacies
                            .Where(c => c.PharmnetId == convertedPharmacyId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        newSale.Date = dateForDb;

                        newSale.Count = count;

                        newSale.DistributorId = await this.db.Distributors
                            .Where(d => d.Name == distributor)
                            .Select(d => d.Id)
                            .FirstOrDefaultAsync();

                        await this.db.Sales.AddAsync(newSale);
                        await this.db.SaveChangesAsync();

                        return true;
                    }
                    else return false;

                case Sopharma:
                    if (await this.db.Products.Where(c => c.SopharmaId == productId).AnyAsync()
                        && await this.db.Pharmacies.Where(c => c.SopharmaId == convertedPharmacyId).AnyAsync())
                    {
                        newSale.ProductId = await this.db.Products
                            .Where(c => c.SopharmaId == productId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        newSale.PharmacyId = await this.db.Pharmacies
                            .Where(c => c.SopharmaId == convertedPharmacyId)
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        newSale.Date = dateForDb;

                        newSale.Count = count;

                        newSale.DistributorId = await this.db.Distributors
                            .Where(d => d.Name == distributor)
                            .Select(d => d.Id)
                            .FirstOrDefaultAsync();

                        await this.db.Sales.AddAsync(newSale);
                        await this.db.SaveChangesAsync();

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
            else
            {
                return await this.db.Sales.Where(p => p.ProductId == productId).SumAsync(c => c.Count);
            }
            

        }

        public async Task<int> ProductCountSumByIdDate(int productId, DateTime dateTime, int? regionId)
        {
            if (regionId != null)
            {
                return await this.db.Sales
                    .Where(p => p.Pharmacy.RegionId == regionId)
                    .Where(d => d.Date.Month == dateTime.Month && d.Date.Year == dateTime.Year)
                    .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
            }
            else
            {
                return await this.db.Sales
                    .Where(d => d.Date.Month == dateTime.Month && d.Date.Year == dateTime.Year)
                    .Where(p => p.ProductId == productId).SumAsync(c => c.Count);
            }
        }

        public async Task<List<DateTime>> GetDistinctDatesByMonths()
        {
            var datesRough = await this.db.Sales.Select(s => s.Date).Distinct().ToListAsync();
            var dates = datesRough.Select(t => new DateTime(t.Year, t.Month, 1)).Distinct().ToList();
            return dates;
        }
    }
}

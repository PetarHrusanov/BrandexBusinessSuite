namespace BrandexSalesAdapter.ExcelLogic.Services.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.EntityFrameworkCore;
    
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Products;
    
    using static Common.ExcelDataConstants.Ditributors;

    public class ProductsService : IProductsService
    {
        SpravkiDbContext db;

        public ProductsService(SpravkiDbContext db)
        {
            this.db = db;
        }

        public async Task<string> CreateProduct(ProductInputModel productInputModel)
        {
            if(productInputModel.BrandexId!= 0 &&
               productInputModel.Name != null &&
               productInputModel.ShortName != null &&
               productInputModel.Price != 0)
            {
                var productDBModel = new Product
                {
                    Name = productInputModel.Name,
                    Price = productInputModel.Price,
                    ShortName = productInputModel.ShortName,
                    BrandexId = productInputModel.BrandexId,
                    PharmnetId = productInputModel.PharmnetId,
                    PhoenixId = productInputModel.PhoenixId,
                    SopharmaId = productInputModel.SopharmaId,
                    StingId = productInputModel.StingId
                };

                await db.Products.AddAsync(productDBModel);
                await db.SaveChangesAsync();
                return productDBModel.Name;
            }

            return "";
        }

        public async Task<List<ProductCheckModel>> GetProductsCheck()
        {
            return await this.db.Products.Select(p => new ProductCheckModel
            {
                Id = p.Id,
                BrandexId = p.BrandexId,
                PhoenixId = p.PhoenixId,
                PharmnetId = p.PharmnetId,
                StingId = p.StingId,
                SopharmaId = p.SopharmaId
            }).ToListAsync();
        }

        public async Task<bool> CheckProductByDistributor(string input, string Distributor)
        {
            int convertedNumber;

            bool success = int.TryParse(input, out convertedNumber);

            switch (Distributor)
            {
                case Brandex:
                    return await this.db.Products.Where(c => c.BrandexId == convertedNumber).AnyAsync();
                case Sting:
                    return await this.db.Products.Where(c => c.StingId == convertedNumber).AnyAsync();
                case Phoenix:
                    return await this.db.Products.Where(c => c.PhoenixId == convertedNumber).AnyAsync();
                case Pharmnet:
                    return await this.db.Products.Where(c => c.PharmnetId == convertedNumber).AnyAsync();
                case Sopharma:
                    var value = await this.db.Products.Where(c => c.SopharmaId == input).AnyAsync();
                    return value;
                default:
                    return false;
            };

        }

        public async Task<int> ProductIdByDistributor(string input, string Distributor)
        {
            int convertedNumber;
            bool success = int.TryParse(input, out convertedNumber);

            switch (Distributor)
            {
                case Brandex:
                    return await this.db.Products.Where(c => c.BrandexId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                case Sting:
                    return await this.db.Products.Where(c => c.StingId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                case Phoenix:
                    return await this.db.Products.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                case Pharmnet:
                    return await this.db.Products.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                case Sopharma:
                    return await this.db.Products.Where(c => c.SopharmaId == input).Select(p => p.Id).FirstOrDefaultAsync();
                default:
                    return 0;
            };
        }

        public async Task<ICollection<ProductDistributorCheck>> ProductsIdByDistributorForCheck(string distributor)
        {
            return distributor switch
            {
                Brandex => await db.Products.Where(c => c.BrandexId != null)
                    .Select(p =>
                        new ProductDistributorCheck { ProductId = p.Id, DistributorId = p.BrandexId.ToString() })
                    .ToListAsync(),
                Sting => await db.Products.Where(c => c.StingId != null)
                    .Select(p => new ProductDistributorCheck { ProductId = p.Id, DistributorId = p.StingId.ToString() })
                    .ToListAsync(),
                Phoenix => await db.Products.Where(c => c.PhoenixId != null)
                    .Select(p =>
                        new ProductDistributorCheck { ProductId = p.Id, DistributorId = p.PhoenixId.ToString() })
                    .ToListAsync(),
                Pharmnet => await db.Products.Where(c => c.PharmnetId != null)
                    .Select(p =>
                        new ProductDistributorCheck { ProductId = p.Id, DistributorId = p.PharmnetId.ToString() })
                    .ToListAsync(),
                Sopharma => await db.Products.Where(c => c.SopharmaId != null)
                    .Select(p =>
                        new ProductDistributorCheck { ProductId = p.Id, DistributorId = p.SopharmaId.ToString() })
                    .ToListAsync(),
                _ => new List<ProductDistributorCheck>()
            };
            ;
        }

        public async Task<string> NameById(string input, string distributor)
        {
            bool unused = int.TryParse(input, out var convertedNumber);

            return distributor switch
            {
                Brandex => await db.Products.Where(c => c.BrandexId == convertedNumber)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(),
                Sting => await db.Products.Where(c => c.StingId == convertedNumber)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(),
                Phoenix => await db.Products.Where(c => c.PhoenixId == convertedNumber)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(),
                Pharmnet => await db.Products.Where(c => c.PharmnetId == convertedNumber)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(),
                Sopharma => await db.Products.Where(c => c.SopharmaId == input)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(),
                _ => ""
            };
            ;
        }

        public async Task<IEnumerable<string>> GetProductsNames()
        {
            return await this.db.Products.Select(p => p.Name).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetProductsId()
        {
            return await this.db.Products.Select(p => p.Id).ToListAsync();
        }

        public async Task<IEnumerable<ProductShortOutputModel>> GetProductsIdPrices()
        {
            return await this.db.Products.Select(p => new ProductShortOutputModel
            {
                Name = p.Name,
                Id = p.Id,
                Price = p.Price
            }).ToListAsync();
        }
    }
}

namespace BrandexSalesAdapter.ExcelLogic.Services.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    using BrandexSalesAdapter.ExcelLogic.Models.Products;
    using static Common.DataConstants.Ditributors;

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

                await this.db.Products.AddAsync(productDBModel);
                await this.db.SaveChangesAsync();
                return productDBModel.Name;
            }
            else
            {
                return "";
            }
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

        public async Task<string> NameById(string input, string distributor)
        {
            int convertedNumber;
            bool success = int.TryParse(input, out convertedNumber);

            switch (distributor)
            {
                case Brandex:
                    return await this.db.Products.Where(c => c.BrandexId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Sting:
                    return await this.db.Products.Where(c => c.StingId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Phoenix:
                    return await this.db.Products.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Pharmnet:
                    return await this.db.Products.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Sopharma:
                    return await this.db.Products.Where(c => c.SopharmaId == input).Select(p => p.Name).FirstOrDefaultAsync();
                default:
                    return "";
            };
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

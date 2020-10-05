namespace BrandexSalesAdapter.ExcelLogic.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public interface ISeeder
    {
        Task SeedAsync(SpravkiDbContext dbContext, IServiceProvider serviceProvider);
    }
}

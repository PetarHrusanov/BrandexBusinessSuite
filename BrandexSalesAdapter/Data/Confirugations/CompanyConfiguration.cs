namespace BrandexSalesAdapter.ExcelLogic.Data.Confirugations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;

    internal class CompanyConfiguration :IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder
                .HasKey(c => c.Id);
        }
    }
}

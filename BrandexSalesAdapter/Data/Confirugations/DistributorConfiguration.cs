namespace BrandexSalesAdapter.ExcelLogic.Data.Confirugations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;

    internal class DistributorConfiguration : IEntityTypeConfiguration<Distributor>
    {
        public void Configure(EntityTypeBuilder<Distributor> builder)
        {
            builder
                .HasKey(c => c.Id);

        }
    }
}

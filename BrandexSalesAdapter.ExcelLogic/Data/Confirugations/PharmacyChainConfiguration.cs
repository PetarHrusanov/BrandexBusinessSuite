namespace BrandexSalesAdapter.ExcelLogic.Data.Confirugations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;

    internal class PharmacyChainConfiguration : IEntityTypeConfiguration<PharmacyChain>
    {
        public void Configure(EntityTypeBuilder<PharmacyChain> builder)
        {
            builder
                .HasKey(c => c.Id);
        }
    }
}

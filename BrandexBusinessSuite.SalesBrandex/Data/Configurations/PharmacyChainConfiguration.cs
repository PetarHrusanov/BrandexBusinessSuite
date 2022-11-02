namespace BrandexBusinessSuite.SalesBrandex.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

internal class PharmacyChainConfiguration : IEntityTypeConfiguration<PharmacyChain>
{
    public void Configure(EntityTypeBuilder<PharmacyChain> builder)
    {
        builder
            .HasKey(c => c.Id);
    }
}
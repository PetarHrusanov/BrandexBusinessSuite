namespace BrandexBusinessSuite.SalesAnalysis.Data.Confirugations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.Data.Models;

internal class PharmacyChainConfiguration : IEntityTypeConfiguration<PharmacyChain>
{
    public void Configure(EntityTypeBuilder<PharmacyChain> builder)
    {
        builder
            .HasKey(c => c.Id);
        
        builder
            .Property(c => c.ErpId);
    }
}
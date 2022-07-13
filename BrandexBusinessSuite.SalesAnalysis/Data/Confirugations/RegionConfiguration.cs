using BrandexBusinessSuite.SalesAnalysis.Data.Models;

namespace BrandexBusinessSuite.SalesAnalysis.Data.Confirugations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.Data.Models;

internal class RegionConfiguration
    : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();

    }
}
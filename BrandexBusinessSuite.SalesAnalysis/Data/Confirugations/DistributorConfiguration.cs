using BrandexBusinessSuite.SalesAnalysis.Data.Models;

namespace BrandexBusinessSuite.SalesAnalysis.Data.Confirugations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.Data.Models;

internal class DistributorConfiguration : IEntityTypeConfiguration<Distributor>
{
    public void Configure(EntityTypeBuilder<Distributor> builder)
    {
        builder
            .HasKey(c => c.Id);
    }
}
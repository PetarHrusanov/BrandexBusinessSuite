namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RegionConfiguration: IEntityTypeConfiguration<Region>
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
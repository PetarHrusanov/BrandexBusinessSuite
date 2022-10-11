using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

public class DriverRegionConfiguration: IEntityTypeConfiguration<DriverRegion>
{
    public void Configure(EntityTypeBuilder<DriverRegion> builder)
    {

        builder
            .Property(c => c.Active)
            .IsRequired();

    }
}
namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CarBrandConfiguration: IEntityTypeConfiguration<CarBrand>
{
    public void Configure(EntityTypeBuilder<CarBrand> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
    }
}
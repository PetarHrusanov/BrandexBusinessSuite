using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

public class DriverCarConfiguration: IEntityTypeConfiguration<DriverCar>
{
    public void Configure(EntityTypeBuilder<DriverCar> builder)
    {
        // builder
        //     .HasKey(c => new {c.DriverId, c.CarId});
        
        builder
            .Property(c => c.Active)
            .IsRequired();
        
        // builder
        //     .HasNoKey();
    }
}
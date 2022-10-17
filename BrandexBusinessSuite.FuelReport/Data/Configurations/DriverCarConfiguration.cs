namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class DriverCarConfiguration: IEntityTypeConfiguration<DriverCar>
{
    public void Configure(EntityTypeBuilder<DriverCar> builder)
    {

        builder
            .Property(c => c.Active)
            .IsRequired();
        
        builder
            .HasOne(c => c.Car)
            .WithMany(c => c.DriverCars)
            .HasForeignKey(c => c.CarId);
        
        builder
            .HasOne(c => c.Driver)
            .WithMany(c => c.DriverCars)
            .HasForeignKey(c => c.DriverId);
        
    }
}
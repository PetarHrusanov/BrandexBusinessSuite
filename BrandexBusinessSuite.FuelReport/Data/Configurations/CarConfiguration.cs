using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

public class CarConfiguration: IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Registration)
            .IsRequired();
        
        builder
            .Property(c => c.Mileage)
            .IsRequired();
        
        builder
            .Property(c => c.Active)
            .IsRequired();
        
        builder
            .HasOne(c => c.CarModel)
            .WithMany(c => c.Cars)
            .HasForeignKey(c => c.CarModelId);


    }
}
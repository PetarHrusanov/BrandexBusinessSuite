using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

public class DriverConfiguration: IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.LastName)
            .IsRequired();
        
        builder
            .Property(c => c.UserId)
            .IsRequired();
        
        builder
            .Property(c => c.Active)
            .IsRequired();
        
        // builder
        //     .HasOne(c => c.CarBrand)
        //     .WithMany(c => c.CarModels)
        //     .HasForeignKey(c => c.CarBrandId);
    }
}
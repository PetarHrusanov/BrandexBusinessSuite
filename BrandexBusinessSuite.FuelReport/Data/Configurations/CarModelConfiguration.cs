namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CarModelConfiguration: IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .HasOne(c => c.CarBrand)
            .WithMany(c => c.CarModels)
            .HasForeignKey(c => c.CarBrandId);
    }
}
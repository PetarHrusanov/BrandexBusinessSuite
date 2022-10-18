namespace BrandexBusinessSuite.FuelReport.Data.Configurations;

using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RouteLogConfiguration: IEntityTypeConfiguration<RouteLog>
{
    public void Configure(EntityTypeBuilder<RouteLog> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Km)
            .IsRequired();
        
        builder
            .Property(c => c.MileageStart)
            .IsRequired();
        
        builder
            .Property(c => c.MileageEnd)
            .IsRequired();
        
        builder
            .Property(c => c.Date)
            .IsRequired();
        
        builder
            .HasOne(c => c.DriverCar)
            .WithMany(c => c.RouteLogs)
            .HasForeignKey(c => new {c.DriverCarDriverId, c.DriverCarCarId});

    }
}
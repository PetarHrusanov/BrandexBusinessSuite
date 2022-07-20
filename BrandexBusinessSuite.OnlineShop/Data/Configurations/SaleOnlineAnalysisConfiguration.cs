using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.OnlineShop.Data.Configurations;

public class SaleOnlineAnalysisConfiguration
{
    public void Configure(EntityTypeBuilder<SaleOnlineAnalysis> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.ProductId)
            .IsRequired();
        
        builder
            .HasOne(c => c.Product)
            .WithMany(s => s.SaleOnlineAnalysis)
            .HasForeignKey(s => s.ProductId);
        
        builder
            .Property(c => c.OrderNumber)
            .IsRequired();
        
        builder
            .Property(c => c.Date)
            .IsRequired();
        
        builder
            .Property(c => c.Quantity)
            .IsRequired();
        
        builder
            .Property(c => c.Total)
            .IsRequired();
        
        builder
            .Property(c => c.City)
            .IsRequired();
        
        builder
            .Property(c => c.Sample)
            .IsRequired();
        
        builder
            .Property(c => c.AdSource)
            .IsRequired();
    }
}

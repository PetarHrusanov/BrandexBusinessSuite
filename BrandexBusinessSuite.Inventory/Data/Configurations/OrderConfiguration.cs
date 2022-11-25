using BrandexBusinessSuite.Inventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.Inventory.Data.Configurations;

public class OrderConfiguration: IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Quantity)
            .IsRequired();
        
        builder
            .Property(c => c.Price)
            .IsRequired();
        
        builder
            .Property(c => c.OrderDate)
            .IsRequired();
        builder
            .HasOne(c => c.Material)
            .WithMany(c => c.Orders)
            .HasForeignKey(c => c.MaterialId);
        
        builder
            .HasOne(c => c.Supplier)
            .WithMany(c => c.Orders)
            .HasForeignKey(c => c.SupplierId);
    }
}



using BrandexBusinessSuite.Inventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.Inventory.Data.Configurations;

public class SupplierConfiguration: IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.Contact)
            .IsRequired();
    }
}
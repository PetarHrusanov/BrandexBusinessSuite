namespace BrandexBusinessSuite.OnlineShop.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.ErpCode)
            .IsRequired();
        
        builder
            .Property(c => c.ErpPriceCode)
            .IsRequired();
        builder
            .Property(c => c.ErpPriceNoVat)
            .IsRequired();
    }
}
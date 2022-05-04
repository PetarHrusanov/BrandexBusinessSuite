using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexSalesAdapter.MarketingAnalysis.Data.Configurations;

using BrandexSalesAdapter.MarketingAnalysis.Data.Models;
using Microsoft.EntityFrameworkCore;


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
            .Property(c => c.ShortName)
            .IsRequired();
    }
}
using BrandexBusinessSuite.Inventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.Inventory.Data.Configurations;

public class StockConfiguration: IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Quantity)
            .IsRequired();
    }
}
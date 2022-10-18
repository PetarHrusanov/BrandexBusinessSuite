using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.OnlineShop.Data.Configurations;

public class DeliveryPriceConfiguration : IEntityTypeConfiguration<DeliveryPrice>
{
    public void Configure(EntityTypeBuilder<DeliveryPrice> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Price)
            .IsRequired();

        builder
            .Property(c => c.ErpId)
            .IsRequired();

        builder
            .Property(c => c.ErpPriceId)
            .IsRequired();
    }
}
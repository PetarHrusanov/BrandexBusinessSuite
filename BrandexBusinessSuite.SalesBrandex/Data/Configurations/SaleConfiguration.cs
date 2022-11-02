namespace BrandexBusinessSuite.SalesBrandex.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;


internal class SaleConfiguration
    : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .HasOne(c => c.Product)
            .WithMany(s => s.Sales)
            .HasForeignKey(s => s.ProductId);

        builder
            .HasOne(p => p.Pharmacy)
            .WithMany(s => s.Sales)
            .HasForeignKey(s => s.PharmacyId);
        
        
    }
}
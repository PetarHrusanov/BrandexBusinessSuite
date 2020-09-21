namespace BrandexSalesAdapter.ExcelLogic.Data.Confirugations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;


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

            builder
                .HasOne(p => p.Distributor)
                .WithMany(s => s.Sales)
                .HasForeignKey(s => s.DistributorId);
        }
    }
}

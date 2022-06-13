namespace BrandexSalesAdapter.MarketingAnalysis.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class MarketingActivityConfiguration : IEntityTypeConfiguration<MarketingActivity>
{
    public void Configure(EntityTypeBuilder<MarketingActivity> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(p => p.Price)
            .HasColumnType("decimal(18,4)");

        builder
            .HasOne(c => c.Product)
            .WithMany(s => s.MarketingActivities)
            .HasForeignKey(s => s.ProductId);

        builder
            .HasOne(p => p.AdMedia)
            .WithMany(s => s.MarketingActivities)
            .HasForeignKey(s => s.AdMediaId);
        
    }
}
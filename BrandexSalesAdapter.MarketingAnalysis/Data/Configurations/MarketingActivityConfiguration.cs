using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexSalesAdapter.MarketingAnalysis.Data.Configurations;

using BrandexSalesAdapter.MarketingAnalysis.Data.Models;
using Microsoft.EntityFrameworkCore;

public class MarketingActivityConfiguration : IEntityTypeConfiguration<MarketingActivity>
{
    public void Configure(EntityTypeBuilder<MarketingActivity> builder)
    {
        builder
            .HasKey(c => c.Id);

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
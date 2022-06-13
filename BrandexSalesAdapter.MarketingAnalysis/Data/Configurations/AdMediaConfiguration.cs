namespace BrandexSalesAdapter.MarketingAnalysis.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class AdMediaConfiguration : IEntityTypeConfiguration<AdMedia>
{
    public void Configure(EntityTypeBuilder<AdMedia> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
    }
}
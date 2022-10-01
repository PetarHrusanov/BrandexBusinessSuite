using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.MarketingAnalysis.Data.Configurations;

public class MediaTypeConfiguration : IEntityTypeConfiguration<MediaType>
{
    public void Configure(EntityTypeBuilder<MediaType> builder)
    {
        builder
            .HasKey(c => c.Id);
        
        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.NameBg)
            .IsRequired();
    }
}
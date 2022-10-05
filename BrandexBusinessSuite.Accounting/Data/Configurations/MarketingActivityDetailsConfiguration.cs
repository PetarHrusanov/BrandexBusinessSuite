namespace BrandexBusinessSuite.Accounting.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class MarketingActivityDetailsConfiguration: IEntityTypeConfiguration<MarketingActivityDetails>
{
    public void Configure(EntityTypeBuilder<MarketingActivityDetails> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.Subject)
            .IsRequired();
    
        builder
            .Property(c => c.PartyId)
            .IsRequired();
        builder
            .Property(c => c.Measure)
            .IsRequired();
        builder
            .Property(c => c.Type)
            .IsRequired();
        builder
            .Property(c => c.Media)
            .IsRequired();
        builder
            .Property(c => c.PublishType)
            .IsRequired();
        
    }
}
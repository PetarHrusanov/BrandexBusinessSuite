using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.MarketingAnalysis.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .HasKey(c => c.Id);
        
        builder
            .Property(c => c.Name)
            .IsRequired();
    }
}
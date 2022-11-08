using BrandexBusinessSuite.SalesAnalysis.Data.Models;

namespace BrandexBusinessSuite.SalesAnalysis.Data.Confirugations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.Data.Models;

internal class CompanyConfiguration :IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .HasKey(c => c.Id);
        
        builder
            .Property(c => c.ErpId);
    }
}
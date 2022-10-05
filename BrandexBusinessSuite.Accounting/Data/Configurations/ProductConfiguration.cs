namespace BrandexBusinessSuite.Accounting.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.FacebookName)
            .IsRequired();
        
        builder
            .Property(c => c.GoogleName)
            .IsRequired();
        
        builder
            .Property(c => c.GoogleNameErp)
            .IsRequired();
        
        builder
            .Property(c => c.AccountingName)
            .IsRequired();
        
        builder
            .Property(c => c.AccountingErpNumber)
            .IsRequired();
        
    }
}
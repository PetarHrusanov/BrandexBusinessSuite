namespace BrandexBusinessSuite.Inventory.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class MaterialConfiguration: IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .Property(c => c.ErpId)
            .IsRequired();
        
        builder
            .Property(c => c.PartNumber)
            .IsRequired();
        builder
            .Property(c => c.Type)
            .IsRequired();
        builder
            .Property(c => c.Measurement)
            .IsRequired();
    }
}
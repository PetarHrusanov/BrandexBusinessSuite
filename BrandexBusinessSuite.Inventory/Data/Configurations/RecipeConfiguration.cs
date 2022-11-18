using BrandexBusinessSuite.Inventory.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrandexBusinessSuite.Inventory.Data.Configurations;

public class RecipeConfiguration: IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder
            .HasOne(c => c.Product)
            .WithMany(c => c.Recipes)
            .HasForeignKey(c => c.ProductId);
        
        builder
            .HasOne(c => c.Material)
            .WithMany(c => c.Recipes)
            .HasForeignKey(c => c.MaterialId);
        
        builder
            .Property(c => c.QuantityRequired)
            .IsRequired();
    }
}
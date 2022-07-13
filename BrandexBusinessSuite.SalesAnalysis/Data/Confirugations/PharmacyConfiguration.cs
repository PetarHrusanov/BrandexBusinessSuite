using BrandexBusinessSuite.SalesAnalysis.Data.Models;

namespace BrandexBusinessSuite.SalesAnalysis.Data.Confirugations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.Data.Models;

internal class PharmacyConfiguration
    : IEntityTypeConfiguration<Pharmacy>
{
    public void Configure(EntityTypeBuilder<Pharmacy> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();

        builder
            .HasOne(c => c.PharmacyChain)
            .WithMany(c => c.Pharmacies)
            .HasForeignKey(c => c.PharmacyChainId);

        builder
            .HasOne(c => c.Company)
            .WithMany(c => c.Pharmacies)
            .HasForeignKey(c => c.CompanyId);

        builder
            .HasOne(c => c.City)
            .WithMany(c => c.Pharmacies)
            .HasForeignKey(c => c.CityId);

        builder
            .HasOne(c => c.Region)
            .WithMany(c => c.Pharmacies)
            .HasForeignKey(c => c.RegionId);

    }
}
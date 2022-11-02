﻿namespace BrandexBusinessSuite.SalesBrandex.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

internal class CompanyConfiguration :IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .HasKey(c => c.Id);
    }
}
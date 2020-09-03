namespace SpravkiFirstDraft.Data.Confirugations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using SpravkiFirstDraft.Data.Models;

    internal class RegionConfiguration
        : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder
                .HasKey(c => c.Id);

            builder
                .Property(c => c.Name)
                .IsRequired();

        }
    }
}

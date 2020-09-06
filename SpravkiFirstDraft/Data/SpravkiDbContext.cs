using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SpravkiFirstDraft.Data.Models;

namespace SpravkiFirstDraft.Data
{
    public class SpravkiDbContext : DbContext
    {
        public SpravkiDbContext(DbContextOptions<SpravkiDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Pharmacy> Pharmacies { get; set; }

        public DbSet<PharmacyChain> PharmacyChains { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Distributor> Distributors { get; set; }

    }
}

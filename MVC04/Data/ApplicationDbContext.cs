using Microsoft.EntityFrameworkCore;
using MVC04.Models;

namespace MVC04.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("tblProducts");
                entity.HasKey(e => e.ProductID);
                entity.HasIndex(e => e.ProductName).IsUnique();
                entity.Property(e => e.ProductPrice).HasColumnType("decimal(18,2)");
            });
        }
    }
}

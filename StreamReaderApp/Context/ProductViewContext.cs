using Microsoft.EntityFrameworkCore;
using StreamReaderApp.Models;

namespace StreamReaderApp.Context
{
    public class ProductViewContext : DbContext
    {
        public DbSet<ProductView> ProductViews { get; set; }
        public DbSet<Product> ?Products { get; set; }
        public DbSet<User> ?Users { get; set; }
        public DbSet<Models.Context>? Contexts { get; set; }
        //DBSet ler eklendi.
            
        public ProductViewContext(DbContextOptions<ProductViewContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=123456;Database=data-db");
                //ConnectionString buradan da alınabilir. Lakin HostBuilder tarafında verdigim icin burada kullanmıyorum.
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}

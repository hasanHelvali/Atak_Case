using Atak_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Atak_API.Context
{
    public class APIDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public APIDbContext(DbContextOptions<APIDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Models.ProductView> ProductViews { get; set; }
        //public DbSet<Models.Order> Orders { get; set; }
        //public DbSet<Models.OrderItem> OrderItems { get; set; }
        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Context> Contexts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Order>()
            //    .HasMany(o => o.OrderItems)
            //    .WithOne(oi => oi.Order)
            //    .HasForeignKey(oi => oi.OrderId);
            ////Order ve OrderItem lar arasında 1-n iliski tercih edildi.
            //base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("ConnectionString"));
        }
    }
}

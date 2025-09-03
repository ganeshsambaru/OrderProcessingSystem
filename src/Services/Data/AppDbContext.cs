using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Models;

namespace OrderProcessingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }

        public DbSet<Order> Orders { get; set; }
    }
    
}

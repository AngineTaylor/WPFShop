// Shop/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(ci => ci.Order)
                .HasForeignKey(ci => ci.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Уникальный email для пользователей
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
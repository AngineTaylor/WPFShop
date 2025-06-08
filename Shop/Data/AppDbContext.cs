using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор для использования через DI
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        // DbSet для моделей
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Здесь можно добавить seed данные или дополнительные настройки
        }
    }
}
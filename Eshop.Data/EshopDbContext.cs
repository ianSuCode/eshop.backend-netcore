using Eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Data
{
    public class EshopDbContext : DbContext
    {
        public EshopDbContext(DbContextOptions<EshopDbContext> options) : base(options)
        { }

        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Category> Category { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<Product> Product { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<UserRole> UserRole { get; set; }
    }
}
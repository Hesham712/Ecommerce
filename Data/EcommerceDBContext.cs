using Ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
    public class EcommerceDBContext : IdentityDbContext<ApplicationUser>
    {
        public EcommerceDBContext(DbContextOptions<EcommerceDBContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<CartProduct> CartProduct { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }
        public DbSet<WishListProduct> WishListProduct { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.WishList)
                .WithOne(w => w.User)
                .HasForeignKey<WishList>(w => w.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(u => u.Refund)
                .WithOne(c => c.Order)
                .HasForeignKey<Refund>(c => c.OrderId);

            // Composite Key Configuration
            modelBuilder.Entity<WishListProduct>()
                .HasKey(wp => new { wp.WishListId, wp.ProductId });

            // Relationships
            modelBuilder.Entity<Product>()
                .HasMany(wp => wp.WishLists)
                .WithMany(w => w.Products)
                .UsingEntity<WishListProduct>();

            // Composite Key Configuration
            modelBuilder.Entity<OrderProduct>()
                .HasKey(wp => new { wp.OrderId, wp.ProductId });

            // Relationships
            modelBuilder.Entity<Product>()
                .HasMany(wp => wp.Orders)
                .WithMany(w => w.Products)
                .UsingEntity<OrderProduct>();

            // Composite Key Configuration
            modelBuilder.Entity<CartProduct>()
                .HasKey(wp => new { wp.CartId, wp.ProductId });

            // Relationships
            modelBuilder.Entity<Product>()
                .HasMany(wp => wp.Carts)
                .WithMany(w => w.Products)
                .UsingEntity<CartProduct>();

        }
    }
}

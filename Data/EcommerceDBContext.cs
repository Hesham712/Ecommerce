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
        public DbSet<Rate> Rates { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Order> Orders { get; set; }
        //public DbSet<SubOrder> SubOrder { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<CartProduct> CartProduct { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }
        public DbSet<WishListProduct> WishListProduct { get; set; }
        public DbSet<RefundItem> RefundItem { get; set; }
        public DbSet<InteractionType> InteractionType { get; set; }
        public DbSet<UserInteraction> UserInteraction { get; set; }
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

            // Composite Key Configuration
            modelBuilder.Entity<OrderProduct>()
                .HasAlternateKey(wp => new { wp.OrderId, wp.ProductId });

            modelBuilder.Entity<Product>()
                .Property(e => e.IsVisible)
                .HasDefaultValue(true);

            // Composite Key Configuration
            modelBuilder.Entity<CartProduct>()
                .HasKey(wp => new { wp.CartId, wp.ProductId });

            modelBuilder.Entity<Rate>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rates)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // لا يمكن حذف المستخدم إذا كان لديه تقييمات

            modelBuilder.Entity<Rate>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Rates)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // عند حذف المنتج، يتم حذف التقييمات

            modelBuilder.Entity<Rate>()
                .HasAlternateKey(r => new { r.UserId, r.ProductId });

            modelBuilder.Entity<Refund>()
                .HasMany(r => r.RefundItems)
                .WithOne(ri => ri.Refund)
                .HasForeignKey(ri => ri.RefundId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}

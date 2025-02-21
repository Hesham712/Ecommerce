namespace Ecommerce.Models
{
    public class Product : AbstractModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int Stock { get; set; } = 0;
        public string? ImagePath { get; set; }
        public double? Rate { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public List<Order> Orders { get; set; }
        public List<Cart> Carts { get; set; }
        public List<WishList> WishLists { get; set; }
    }
}

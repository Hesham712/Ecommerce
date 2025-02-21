using Ecommerce.Models;

namespace Ecommerce.DTO_s.Product
{
    public class ProductGetDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int Stock { get; set; } = 0;
        public string? ImagePath { get; set; }
        public double? Rate { get; set; }
        public string UserId { get; set; }
        public int ProductCategoryId { get; set; }
        public List<Order> Orders { get; set; }
        public List<Cart> Carts { get; set; }
        public List<WishList> WishLists { get; set; }
    }
}

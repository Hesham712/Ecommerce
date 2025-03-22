using Ecommerce.DTO_s.ApplicationUser;
using Ecommerce.DTO_s.ProductCategory;
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
        public bool IsVisible { get; set; }
        public ProductCategoryGetDTO ProductCategory { get; set; }
        public UserGetDTO User { get; set; }
        //public List<Order>? Orders { get; set; }
        //public List<Models.Cart>? Carts { get; set; }
        //public List<WishList>? WishLists { get; set; }
    }
}

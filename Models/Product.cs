using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Product : AbstractModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; } = 0;
        public string? ImagePath { get; set; }
        public bool IsVisible { get; set; } = true;
        [ForeignKey("User")]
        public string SellerId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public List<Rate>? Rates { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        public List<CartProduct> CartProducts { get; set; }
        public List<WishListProduct> WishListProduct { get; set; }
    }
}

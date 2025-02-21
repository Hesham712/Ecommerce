using Ecommerce.DTO_s.Product;

namespace Ecommerce.DTO_s.Cart
{
    public class CartProductGetDTO
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductGetDTO Product { get; set; }
    }
}

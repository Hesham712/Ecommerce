using Ecommerce.DTO_s.Product;

namespace Ecommerce.DTO_s.WishList
{
    public class WishListProductGetDTO
    {
        public int WishListId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductGetDTO Product { get; set; }
    }
}

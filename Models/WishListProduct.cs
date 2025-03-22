namespace Ecommerce.Models
{
    public class WishListProduct 
    {
        public int WishListId { get; set; }
        public WishList WishList { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

    }
}

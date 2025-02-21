namespace Ecommerce.Models
{
    public class WishList : AbstractModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<Product> Products { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO_s.WishList
{
    public class WishListPostDTO
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}

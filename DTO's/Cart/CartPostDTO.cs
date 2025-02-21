using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO_s.Cart
{
    public class CartPostDTO
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}

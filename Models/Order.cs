using Ecommerce.Helper;

namespace Ecommerce.Models
{
    public class Order : AbstractModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal? TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int? RefundId { get; set; }
        public Refund Refund { get; set; }
        public List<Product> Products { get; set; }

    }
}

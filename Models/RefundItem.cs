using Ecommerce.Helper;

namespace Ecommerce.Models
{
    public class RefundItem : AbstractModel
    {
        public int RefundId { get; set; }
        public Refund? Refund { get; set; }
        public int OrderProductId { get; set; }
        public OrderProduct OrderProduct { get; set; }
        public int Quantity { get; set; }
        public string? Reason { get; set; }
    }
}

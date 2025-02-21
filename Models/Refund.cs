using Ecommerce.Helper;

namespace Ecommerce.Models
{
    public class Refund : AbstractModel
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
        public RefundStatus Status { get; set; } = RefundStatus.Pending;
    }
}

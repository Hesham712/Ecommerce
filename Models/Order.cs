using Ecommerce.Helper;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce.Models
{
    public class Order : AbstractModel
    {
        [ForeignKey("User")]
        public string CustomerId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal? TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? ShippingAddress { get; set; }
        public int? RefundId { get; set; }
        public Refund? Refund { get; set; }
        public List<OrderProduct> OrderProduct { get; set; }

    }
}
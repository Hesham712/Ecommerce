using Ecommerce.DTO_s.OrderProduct;
using Ecommerce.Helper;

namespace Ecommerce.DTO_s.Order
{
    public class OrderGetDTO
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public decimal? TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public int? RefundId { get; set; }
        public List<OrderProductGetDTO> OrderProduct { get; set; }
    }
}

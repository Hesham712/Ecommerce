namespace Ecommerce.DTO_s.Refund
{
    public class RefundPostDTO
    {
        public int OrderId { get; set; }
        public RefundItemPostDTO RefundItems { get; set; }
    }
    public class RefundItemPostDTO
    {
        public int OrderProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }
}

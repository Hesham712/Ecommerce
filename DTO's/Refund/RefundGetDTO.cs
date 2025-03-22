namespace Ecommerce.DTO_s.Refund
{
    public class RefundGetDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<RefundItemGetDTO> RefundItems { get; set; }
    }
    public class RefundItemGetDTO
    {
        public int Id { get; set; }
        public int OrderProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }
}

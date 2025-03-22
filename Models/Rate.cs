namespace Ecommerce.Models
{
    public class Rate : AbstractModel
    {
        public int RateValue { get; set; }
        public string? Comment { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}

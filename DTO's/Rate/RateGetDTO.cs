using Ecommerce.Models;

namespace Ecommerce.DTO_s.Rate
{
    public class RateGetDTO : AbstractModel
    {
        public int RateValue { get; set; }
        public string UserId { get; set; }
        public string? Comment { get; set; }
        public int ProductId { get; set; }

    }
}

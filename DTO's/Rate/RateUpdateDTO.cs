using Ecommerce.Models;

namespace Ecommerce.DTO_s.Rate
{
    public class RateUpdateDTO : IBaseModel
    {
        public int Id { get; set; }
        public int RateValue { get; set; }
        public string? Comment { get; set; }

    }
}

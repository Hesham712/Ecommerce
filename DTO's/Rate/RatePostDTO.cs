using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO_s.Rate
{
    public class RatePostDTO
    {
        [Required]
        [Range(1, 5)]
        public int RateValue { get; set; }
        [JsonIgnore]
        public string? UserId { get; set; }
        public string? Comment { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}

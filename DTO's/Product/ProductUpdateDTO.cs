using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO_s.Product
{
    public class ProductUpdateDTO : IBaseModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Stock { get; set; }
        public IFormFile? ImagePath { get; set; }
        public int? ProductCategoryId { get; set; }
        [Required]
        public bool IsVisible { get; set; }
        [JsonIgnore]
        internal string userName { get; set; }
    }
}

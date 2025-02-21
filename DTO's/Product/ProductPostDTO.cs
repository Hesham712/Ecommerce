using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO_s.Product
{
    public class ProductPostDTO
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public double? Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public IFormFile? ImagePath { get; set; }
        public double? Rate { get; set; }
        [Required]
        public int ProductCategoryId { get; set; }
    }
}

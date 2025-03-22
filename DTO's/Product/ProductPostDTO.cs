using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [Required]
        public int ProductCategoryId { get; set; }
        [JsonIgnore]
        internal string? userName { get; set; }

    }
}

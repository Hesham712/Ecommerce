using Ecommerce.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Ecommerce.DTO_s.Order
{
    public class OrderPostDTO
    {
        [JsonIgnore]
        public string? UserName { get; set; }
        public string ShippingAddress { get; set; }
    }
}

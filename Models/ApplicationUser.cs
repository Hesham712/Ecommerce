using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Ecommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public int? CartId { get; set; }
        public Cart Cart { get; set; }
        public int? WishListId { get; set; }
        public WishList WishList { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public List<Product>? Products { get; set; }
        public List<Order>? Orders { get; set; }
    }
}

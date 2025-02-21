
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO_S.ApplicationUser
{
    public class UserUpdateDTO
    {
        [MaxLength(100)]
        public string? FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
    }
}

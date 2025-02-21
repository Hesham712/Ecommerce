using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO_S.ApplicationUser
{
    public class UserRegisterDTO
    {
        [MaxLength(100)]
        [Required]
        public string FullName { get; set; }
        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password mismatch")]
        [Required]
        public string ConfirmPassword { get; set; }
        [Phone]
        [Required]
        public string PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public bool IsSeller { get; set; } = false;
    }
}

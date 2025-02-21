using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO_S.ApplicationUser
{
    public class UserChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword),ErrorMessage ="Password mismatch")]
        public string ConfirmNewPassword { get; set; }
    }
}

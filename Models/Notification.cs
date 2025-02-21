namespace Ecommerce.Models
{
    public class Notification : AbstractModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}

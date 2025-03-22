namespace Ecommerce.Models
{
    public class UserInteraction : AbstractModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int InteractionTypeId { get; set; }
        public InteractionType InteractionType { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

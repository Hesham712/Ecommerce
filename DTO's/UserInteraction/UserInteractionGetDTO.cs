using Ecommerce.DTO_s.ApplicationUser;
using Ecommerce.DTO_s.InteractionType;
using Ecommerce.DTO_s.Product;
using Ecommerce.Models;

namespace Ecommerce.DTO_s.UserInteraction
{
    public class UserInteractionGetDTO : AbstractModel
    {
        public string UserId { get; set; }
        public UserGetDTO User { get; set; }
        public int InteractionTypeId { get; set; }
        public InteractionTypeGetDTO InteractionType { get; set; }
        public int ProductId { get; set; }
        public ProductGetDTO Product { get; set; }

    }
}

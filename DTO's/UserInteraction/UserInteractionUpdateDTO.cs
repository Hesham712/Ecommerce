using Ecommerce.Models;

namespace Ecommerce.DTO_s.UserInteraction
{
    public class UserInteractionUpdateDTO :IBaseModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int InteractionTypeId { get; set; }
        public int ProductId { get; set; }

    }
}

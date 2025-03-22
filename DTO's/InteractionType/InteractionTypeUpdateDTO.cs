using Ecommerce.Models;

namespace Ecommerce.DTO_s.InteractionType
{
    public class InteractionTypeUpdateDTO :IBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

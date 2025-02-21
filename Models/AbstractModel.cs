using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class AbstractModel : IBaseModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; }
    }
}

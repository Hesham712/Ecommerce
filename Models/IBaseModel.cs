using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public interface IBaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}

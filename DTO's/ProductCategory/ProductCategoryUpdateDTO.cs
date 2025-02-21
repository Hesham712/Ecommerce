using Ecommerce.Models;

namespace Ecommerce.DTO_s.ProductCategory
{
    public class ProductCategoryUpdateDTO : IBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

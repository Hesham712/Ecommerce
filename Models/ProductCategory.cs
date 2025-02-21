namespace Ecommerce.Models
{
    public class ProductCategory : AbstractModel
    {
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}

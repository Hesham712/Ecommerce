using Ecommerce.Models;

namespace Ecommerce.DTO_s.OrderProduct
{
    public class OrderProductGetDTO : AbstractModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }  // Product ID
        public string ProductName { get; set; }  // Product name
        public decimal? Price { get; set; }  // Product price
        public int Quantity { get; set; }  // Quantity ordered
    }
}

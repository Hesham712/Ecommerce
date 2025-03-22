using Ecommerce.DTO_s.Cart;
using Ecommerce.DTO_s.Order;
using Ecommerce.Helper;

namespace Ecommerce.Repository.OrderService
{
    public interface IOrderService
    {
        public Task<ResponseResult> Checkout(OrderPostDTO dto);
        public Task<ResponseResult> GetAll();
        public Task<ResponseResult> GetById(int orderId);
        public Task<ResponseResult> GetByCustomer(string UserName);
        public Task<ResponseResult> GetBySeller(string UserName);
        public Task<ResponseResult> UpdateOrderStatus(int OrderId, OrderStatus status);
        public Task<ResponseResult> Delete(int OrderId,string userName);
    }
}

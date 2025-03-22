using Ecommerce.DTO_s.Cart;
using Ecommerce.Helper;
using Ecommerce.Models;

namespace Ecommerce.Repository.CartService
{
    public interface ICartService
    {
        public Task<ResponseResult> GetCartAsync(string userName);
        public Task<ResponseResult> DeleteProductFromCartAsync(int productId,string userName);
        public Task<ResponseResult> AddCartAsync(CartPostDTO dto,string userName);
    }
}

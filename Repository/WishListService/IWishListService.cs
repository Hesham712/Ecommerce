using Ecommerce.DTO_s.Cart;
using Ecommerce.DTO_s.WishList;
using Ecommerce.Helper;

namespace Ecommerce.Repository.WishListService
{
    public interface IWishListService
    {
        public Task<ResponseResult> GetWishListAsync(string userName);
        public Task<ResponseResult> DeleteProductFromWishListAsync(int productId, string userName);
        public Task<ResponseResult> AddWishListAsync(WishListPostDTO dto, string userName);
    }
}

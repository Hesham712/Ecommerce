using Ecommerce.DTO_s.Product;
using Ecommerce.Helper;

namespace Ecommerce.Repository.ProductSerivce
{
    public interface IProductSerivce
    {
        Task<ResponseResult> Post(ProductPostDTO productPostDTO);
        Task<ResponseResult> Put(ProductUpdateDTO productUpdateDTO);
        Task<ResponseResult> Delete(int id, string userName);
        Task<ResponseResult> Get(int id,string? userId);
        Task<ResponseResult> GetAll();
        Task<ResponseResult> GetAllBySeller(string userName);
        Task<ResponseResult> SearchByName(string productName);
        //Task<ResponseResult> VisibaltyProduct(bool status, int productId,string userName);

    }
}

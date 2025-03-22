using Ecommerce.DTO_s.Refund;
using Ecommerce.Helper;

namespace Ecommerce.Repository.RefundService
{
    public interface IRefendService
    {
        Task<ResponseResult> Post(RefundPostDTO dto);
        Task<ResponseResult> Get();
        Task<ResponseResult> GetById(int refundId);
        Task<ResponseResult> Delete(int refundId);
        Task<ResponseResult> ChangeRefundStatus(RefundStatus status,int refundId);
        Task<ResponseResult> GetByUser(string userName);
    }
}

using Ecommerce.DTO_S.ApplicationUser;
using Ecommerce.Helper;

namespace Ecommerce.Repository.UserService
{
    public interface IUserService
    {

        public Task<ResponseResult> RegisterAsync(UserRegisterDTO dto);
        public Task<ResponseResult> LoginAsync(UserLoginDTO dto);
        public Task<ResponseResult> UpdateUserAsync(UserUpdateDTO dto,string userName);
        public Task<ResponseResult> ChangePasswordAsync(UserChangePasswordDTO dto,string userName);
        public Task<ResponseResult> GetAllUsersAsync();
        public Task<ResponseResult> GetUserAsync(string userId);
        //Task<object> GetUserAsync(Guid userId);
    }
}

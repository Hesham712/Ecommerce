using Ecommerce.DTO_S.ApplicationUser;
using Ecommerce.Repository.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var result = await _userService.LoginAsync(dto);
            return result.Status ? Ok(result) : Unauthorized(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
        {
            var userName = User.Identity.Name;
            var result = await _userService.UpdateUserAsync(dto,userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDTO dto)
        {
            var userName = User.Identity.Name;
            var result = await _userService.ChangePasswordAsync(dto, userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [Authorize]
        [HttpGet("UserLogin")]
        public async Task<IActionResult> GetUser()
        {
            var UserName = User.Identity.Name;
            var result = await _userService.GetUserAsync(UserName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}

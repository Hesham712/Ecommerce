using AutoMapper;
using Ecommerce.DTO_S.ApplicationUser;
using Ecommerce.Helper;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Repository.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _mapper = mapper;
        }

        public async Task<ResponseResult> ChangePasswordAsync(UserChangePasswordDTO dto, string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user is null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User not found"
                    };
                var changePassword = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

                if (!changePassword.Succeeded)
                {
                    var errors = changePassword.Errors.Select(e => e.Description).ToList();

                    return new ResponseResult
                    {
                        Status = false,
                        Object = errors
                    };
                }
                return new ResponseResult
                {
                    Object = "Password changed successfully",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Object = $"Error : {ex.InnerException?.Message ?? ex.Message}",
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetAllUsersAsync()
        {

            return new ResponseResult
            {
                Object = await _userManager.Users.Select(x => new { x.FullName, x.UserName, x.Email, x.PhoneNumber }).ToListAsync(),
                Status = true
            };
        }

        public async Task<ResponseResult> LoginAsync(UserLoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(m => m.UserName.ToLower() == loginDTO.UserName.ToLower());
                var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

                if (user is null || !checkPassword.Succeeded)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "UserName / Password not correct.!",
                    };
                return new ResponseResult
                {
                    Object = new
                    {
                        FullName = user.FullName,
                        token = await CreateToken(user),
                        Role = await _userManager.GetRolesAsync(user)
                    },
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Object = $"Error : {ex.InnerException?.Message ?? ex.Message}",
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> RegisterAsync(UserRegisterDTO userDTO)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(userDTO.Email);

                if (userExists is not null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Email is taken"
                    };

                var user = _mapper.Map<ApplicationUser>(userDTO);

                var result = await _userManager.CreateAsync(user, userDTO.Password);
                if (result.Succeeded)
                {
                    var roleName = userDTO.IsSeller ? "Seller" : "Customer";
                    await _userManager.AddToRoleAsync(user, roleName);
                    return new ResponseResult
                    {
                        Status = true,
                        Object = "Register Successfully"
                    };
                }

                //list of error when create user failed
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                return new ResponseResult
                {
                    Status = false,
                    Object = new { Errors = errorMessages }
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Object = $"Error : {ex.InnerException?.Message ?? ex.Message}",
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> UpdateUserAsync(UserUpdateDTO dto, string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);

                if (user is null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User Id not found.!",
                    };

                var userUpdated = await _userManager.UpdateAsync(_mapper.Map(dto, user));
                if (!userUpdated.Succeeded)
                {
                    var errors = userUpdated.Errors.Select(e => e.Description).ToList();
                    return new ResponseResult
                    {
                        Object = errors,
                        Status = false
                    };
                }
                return new ResponseResult
                {
                    Object = userUpdated,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Object = $"Error : {ex.InnerException?.Message ?? ex.Message}",
                    Status = false
                };
            }
        }

        private Task<string> CreateToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id)
            };
            // Add user roles to the claims
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));

            var token = new JwtSecurityToken
            (
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}

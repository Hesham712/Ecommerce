using AutoMapper;
using Ecommerce.DTO_s.Cart;
using Ecommerce.Repository.CartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        public CartsController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userName = User?.Identity?.Name;
            var result = await _cartService.GetCartAsync(userName);
            if (result.Status)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartPostDTO dto)
        {
            var userName = User?.Identity?.Name;
            var result = await _cartService.AddCartAsync(dto, userName);
            if (result.Status)
                return Ok(result);
            return BadRequest(result);
        }
    }
}

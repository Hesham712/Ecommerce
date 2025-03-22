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
        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userName = User?.Identity?.Name;
            var result = await _cartService.GetCartAsync(userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartPostDTO dto)
        {
            var userName = User?.Identity?.Name;
            var result = await _cartService.AddCartAsync(dto, userName);
            return result.Status ? Ok(result) : BadRequest(result);

        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductFromCart(int productId)
        {
            var userName = User?.Identity?.Name;
            var result = await _cartService.DeleteProductFromCartAsync(productId, userName);
            return result.Status ? Ok(result) : BadRequest(result);

        }
    }
}

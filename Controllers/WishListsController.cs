using AutoMapper;
using Ecommerce.DTO_s.WishList;
using Ecommerce.Repository.WishListService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class WishListsController : ControllerBase
    {
        private readonly IWishListService _wishListService;
        public WishListsController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }
        [HttpGet]
        public async Task<IActionResult> GetWishList()
        {
            var userName = User?.Identity?.Name;
            var result = await _wishListService.GetWishListAsync(userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddWishList([FromBody] WishListPostDTO dto)
        {
            var userName = User?.Identity?.Name;
            var result = await _wishListService.AddWishListAsync(dto, userName);
            return result.Status ? Ok(result) : BadRequest(result);

        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductFromWishList(int productId)
        {
            var userName = User?.Identity?.Name;
            var result = await _wishListService.DeleteProductFromWishListAsync(productId, userName);
            return result.Status ? Ok(result) : BadRequest(result);

        }
    }

}

using AutoMapper;
using Ecommerce.DTO_s.Order;
using Ecommerce.Helper;
using Ecommerce.Repository.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(OrderPostDTO dto)
        {
            dto.UserName = User.Identity.Name;
            var result = await _orderService.Checkout(dto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _orderService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{OrderId}")]
        public async Task<IActionResult> GetById(int OrderId)
        {
            var result = await _orderService.GetById(OrderId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("GetOrderByCustomer")]
        public async Task<IActionResult> GetOrderByCustomer()
        {
            var userName = User.Identity.Name;
            var result = await _orderService.GetByCustomer(userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("GetOrdersBySeller")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetOrdersBySeller()
        {
            var userName = User.Identity.Name;
            var result = await _orderService.GetBySeller(userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("OrderStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptOrder([Required] int OrderId, [Required] OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatus(OrderId, status);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("{OrderId}")]
        public async Task<IActionResult> Delete([Required] int OrderId)
        {
            var userName = User.Identity.Name;
            var result = await _orderService.Delete(OrderId, userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}

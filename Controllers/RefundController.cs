using Ecommerce.DTO_s.Refund;
using Ecommerce.Helper;
using Ecommerce.Repository.RefundService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RefundController : ControllerBase
    {
        private readonly IRefendService _service;

        public RefundController(IRefendService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Post(RefundPostDTO dto)
        {
            var result = await _service.Post(dto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.Get();
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{refundId}")]
        public async Task<IActionResult> GetById(int refundId)
        {
            var result = await _service.GetById(refundId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("{refundId}")]
        public async Task<IActionResult> Delete(int refundId)
        {
            var result = await _service.Delete(refundId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpPut("RefundStatus")]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> ChangeRefundStatus([Required]RefundStatus status, [Required] int refundId)
        {
            var result = await _service.ChangeRefundStatus(status, refundId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("RefundsByUser")]
        public async Task<IActionResult> GetByUser()
        {
            var userName = User.Identity.Name;
            var result = await _service.GetByUser(userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}

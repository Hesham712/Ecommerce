using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.ApplicationUser;
using Ecommerce.DTO_s.Product;
using Ecommerce.DTO_s.ProductCategory;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Ecommerce.Repository.ProductSerivce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller")]
    public class ProductController : ControllerBase
    {
        private readonly IProductSerivce _productSerivce;
        public ProductController(IProductSerivce productSerivce)
        {
            _productSerivce = productSerivce;
        }
        //[HttpPost("ProductVisibility")]
        //public async Task<IActionResult> VisibaltyProduct([FromQuery] bool IsTrue, int productId)
        //{
        //    var userName = User?.Identity?.Name;
        //    var result = await _productSerivce.VisibaltyProduct(IsTrue, productId, userName);
        //    return result.Status ? Ok(result) : BadRequest(result);
        //}
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductPostDTO dto)
        {
            dto.userName = User?.Identity?.Name;
            var result = await _productSerivce.Post(dto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] ProductUpdateDTO dto)
        {
            dto.userName = User?.Identity?.Name;
            var result = await _productSerivce.Put(dto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userName = User?.Identity?.Name;
            var result = await _productSerivce.GetAllBySeller(userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id,string? userId)
        {
            var result = await _productSerivce.Get(id,userId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userName = User?.Identity?.Name;
            var result = await _productSerivce.Delete(id, userName);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productSerivce.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpGet("Filter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductName([FromQuery][Required] string name)
        {
            var result = await _productSerivce.SearchByName(name);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}

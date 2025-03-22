using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.Rate;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatesController : GenericController<EcommerceDBContext, Rate, RateGetDTO, RateGetDTO, RatePostDTO, RateUpdateDTO>
    {
        private readonly EcommerceDBContext _context;
        public RatesController(IGenericBasicDataRepo<Rate, EcommerceDBContext> repo, IMapper mapper, EcommerceDBContext context) : base(repo, mapper)
        {
            _context = context;
        }
        public override async Task<IActionResult> Post([FromBody] RatePostDTO dto)
        {
            try
            {
                var UserName = User.Identity.Name;
                var user = await _context.ApplicationUsers
                    .Include(x => x.Orders)
                    .ThenInclude(x => x.OrderProduct)
                    .FirstOrDefaultAsync(u => u.UserName == UserName);

                if (user == null)
                    return BadRequest("User not found");

                // Check if the user has ordered the product they're trying to rate
                bool hasOrderedProduct = user.Orders.Any(order =>
                    order.OrderProduct.Any(op => op.ProductId == dto.ProductId));

                if (!hasOrderedProduct)
                    return BadRequest("You can only rate products you have ordered");
                dto.UserId = user.Id;
                return await base.Post(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.UserInteraction;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInteractionsController : GenericController<EcommerceDBContext, UserInteraction, UserInteractionGetDTO, UserInteractionGetDTO, UserInteractionPostDTO, UserInteractionUpdateDTO>
    {
        private readonly IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> _repo;
        public UserInteractionsController(IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> repo, IMapper mapper) : base(repo, mapper)
        {
            _getIncludes = ["User", "Product", "InteractionType"];
            _getAllIncludes = ["User", "Product", "InteractionType"];
            _repo = repo;
        }
        [HttpGet("UserId")]
        public async Task<IActionResult> GetInteractionsByUserId([Required] string userId)
        {
            var result = await _repo.FindAllAsync(x => x.UserId == userId);
            return Ok(result);
        }
    }
}

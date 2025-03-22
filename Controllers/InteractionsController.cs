using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.InteractionType;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class InteractionTypeController : GenericController<EcommerceDBContext, InteractionType, InteractionTypeGetDTO, InteractionTypeGetDTO, InteractionTypePostDTO, InteractionTypeUpdateDTO>
    {
        public InteractionTypeController(IGenericBasicDataRepo<InteractionType, EcommerceDBContext> repo, IMapper mapper) : base(repo, mapper)
        {
        }        
    }
}

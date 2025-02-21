using AutoMapper;
using Ecommerce.DTO_s.ErrorsDTOs;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GenericController<TDbContext, T, GetAllDTO, GetDTO, PostDTO, PutDTO> :ControllerBase where TDbContext : DbContext where T : AbstractModel where PutDTO : IBaseModel
    {
        private readonly IGenericBasicDataRepo<T, TDbContext> _repo;

        private readonly IMapper _mapper;

        public GenericController(IGenericBasicDataRepo<T, TDbContext> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        protected string[] _getAllIncludes = Array.Empty<string>();

        protected string[] _getIncludes = Array.Empty<string>();

        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            IMapper mapper = _mapper;
            return Ok(mapper.Map<IEnumerable<GetAllDTO>>(await _repo.GetAllAsync(_getAllIncludes)));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id)
        {
            ErrorsDTO errorsDTO = await _repo.IsTypeExists(id);
            if (!errorsDTO.IsValid)
            {
                return NotFound(errorsDTO);
            }

            IMapper mapper = _mapper;
            return Ok(mapper.Map<GetDTO>(await _repo.GetByIdAsync(id, _getIncludes)));
        }

        

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            ErrorsDTO errorsDTO = await _repo.IsTypeExists(id);
            if (!errorsDTO.IsValid)
            {
                return NotFound(errorsDTO);
            }

            await _repo.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] PostDTO dto)
        {
            string name = typeof(T).Name;
            IMapper mapper = _mapper;
            return Created(name, mapper.Map<GetDTO>(await _repo.AddAsync(_mapper.Map<T>(dto))));
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromBody] PutDTO dto)
        {
            T val = await _repo.GetByIdAsync(dto.Id);
            if (val == null)
                return NotFound($"Object with Id: {dto.Id} was not found");

            IMapper mapper = _mapper;
            return Ok(_mapper.Map<GetDTO>(await _repo.UpdateAsync(_mapper.Map(dto, val))));
        }
    }
}

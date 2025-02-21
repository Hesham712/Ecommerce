using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.Product;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller")]
    public class ProductController : GenericController<EcommerceDBContext, Product, ProductGetDTO, ProductGetDTO, ProductPostDTO, ProductUpdateDTO>
    {
        private readonly EcommerceDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        IGenericBasicDataRepo<Product, EcommerceDBContext> _repo;
        public ProductController(IGenericBasicDataRepo<Product, EcommerceDBContext> repo, IMapper mapper, EcommerceDBContext context, IWebHostEnvironment env) : base(repo, mapper)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
            _env = env;
        }
        public override async Task<IActionResult> Post([FromForm] ProductPostDTO dto)
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => new { u.Id, CategoryExists = _context.ProductCategory.Any(c => c.Id == dto.ProductCategoryId) })
                .FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Login failed try again");
            if (!user.CategoryExists)
                return BadRequest("Category not found");

            var product = _mapper.Map<Product>(dto);

            product.UserId = user.Id;
            product.ImagePath = dto.ImagePath is not null ? await Upload(dto.ImagePath) : null;

            return Ok(_mapper.Map<ProductGetDTO>(await _repo.AddAsync(product)));
        }
        public override async Task<IActionResult> Put([FromForm] ProductUpdateDTO dto)
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users
                .Where(u => u.UserName == userName)
                .Include(x => x.Products)
                .Select(u => new
                {
                    u.Id,
                    CategoryExists = _context.ProductCategory.Any(c => c.Id == dto.ProductCategoryId),
                    Product = u.Products.Where(p => p.Id == dto.Id).FirstOrDefault()
                }).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Login failed try again");
            if (!user.CategoryExists && dto.ProductCategoryId is not null)
                return BadRequest("Category not found");
            if (user.Product == null)
                return BadRequest("Product not found");

            user.Product.ImagePath = dto.ImagePath is not null ? await Upload(dto.ImagePath) : null;

            _mapper.Map(dto, user.Product);

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<ProductGetDTO>(user.Product));
        }
        public override async Task<IActionResult> Get()
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users
                .Where(u => u.UserName == userName)
                .Include(x => x.Products)
                .ThenInclude(x => x.ProductCategory)
                .Select(u => new
                {
                    u.Id,
                    Products = u.Products
                }).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("Login failed try again");
            if (!user.Products.Any())
                return BadRequest("No products found");
            return Ok(_mapper.Map<List<ProductGetDTO>>(user.Products));
        }
        public override async Task<IActionResult> Get(int id)
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users
                .Where(u => u.UserName == userName)
                .Include(x => x.Products)
                .ThenInclude(x => x.ProductCategory)
                .Select(u => new
                {
                    u.Id,
                    Products = u.Products.Where(x => x.Id == id).FirstOrDefault()
                }).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("Login failed try again");
            if (user.Products is null)
                return BadRequest("No products found");
            return Ok(_mapper.Map<ProductGetDTO>(user.Products));
        }
        public override async Task<IActionResult> Delete(int id)
        {
            var userName = User?.Identity?.Name;
            var user = await _context.Users
                .Where(u => u.UserName == userName)
                .Include(x => x.Products)
                .ThenInclude(x => x.ProductCategory)
                .Select(u => new
                {
                    u.Id,
                    Products = u.Products.Where(x => x.Id == id).FirstOrDefault()
                }).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("Login failed try again");
            if (user.Products is null)
                return BadRequest("No products found");

            await _repo.DeleteAsync(user.Products.Id);
            return Ok("Deleted Successfully");
        }
        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public Task<IActionResult> GetAll()
        {
            return base.Get();
        } 

        private async Task<string> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ("File is null");

            string[] names = file.FileName.Split('.');
            if (names.Length < 2 || file.Length > 5242880)
                return ("Invalid file");

            if (!Directory.Exists(_env.WebRootPath))
                Directory.CreateDirectory(_env.WebRootPath);

            string extension = Path.GetExtension(file.FileName);

            string newRandomFileName = $"{Path.GetRandomFileName()}{DateTime.Now:dM}{extension}";
            string savedFileLocation = Path.Combine(_env.WebRootPath, "Product", newRandomFileName);
            string returnedLocation = Path.Combine("Product", newRandomFileName).Replace("\\", "/");

            using (var fileStream = new FileStream(savedFileLocation, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return returnedLocation;
        }
    }
}

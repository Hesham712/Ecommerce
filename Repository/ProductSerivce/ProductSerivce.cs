using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.ApplicationUser;
using Ecommerce.DTO_s.Product;
using Ecommerce.DTO_s.ProductCategory;
using Ecommerce.Helper;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.ProductSerivce
{
    public class ProductSerivce : IProductSerivce
    {
        private readonly EcommerceDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> _userInteractionService;
        public ProductSerivce(EcommerceDBContext context, IMapper mapper, IWebHostEnvironment env, IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> userInteractionService)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
            _userInteractionService = userInteractionService;
        }

        public async Task<ResponseResult> Delete(int id, string userName)
        {
            try
            {
                var product = await _context.Products
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id && x.User.UserName == userName);
                if (product == null)
                    return new ResponseResult
                    {
                        Object = "Product not found",
                        Status = false
                    };
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new ResponseResult
                {
                    Object = "Product deleted successfully",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> Get(int id, string? userId)
        {
            try
            {
                var result = await _context.Products
                .Include(x => x.Rates)
                .Where(u => u.Id == id)
                .Select(p => new ProductGetDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = (double?)p.Price,
                    Stock = p.Stock,
                    ImagePath = p.ImagePath,
                    IsVisible = p.IsVisible,
                    ProductCategoryId = p.ProductCategoryId,
                    UserId = p.SellerId,
                    Rate = p.Rates.Any() ? p.Rates.Average(r => r.RateValue) : (double?)null,
                    ProductCategory = new ProductCategoryGetDTO
                    {
                        Id = p.ProductCategory.Id,
                        Name = p.ProductCategory.Name
                    },
                    User = new UserGetDTO
                    {
                        Id = p.User.Id,
                        UserName = p.User.UserName,
                        Email = p.User.Email,
                        FullName = p.User.FullName,
                        PhoneNumber = p.User.PhoneNumber
                    }
                })
                .FirstOrDefaultAsync();
                if (result == null)
                    return new ResponseResult
                    {
                        Object = "No products found",
                        Status = false
                    };
                if (userId is not null)
                    if (!await _context.ApplicationUsers.AnyAsync(x => x.Id == userId))
                        return new ResponseResult
                        {
                            Object = "User not found",
                            Status = false
                        };
                    else
                        await _userInteractionService.AddAsync(new UserInteraction
                        {
                            ProductId = id,
                            InteractionTypeId = 2,
                            UserId = userId
                        });
                return new ResponseResult
                {
                    Object = result,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetAll()
        {
            try
            {
                var result = await _context.Products
                .Include(x => x.Rates)
                .Where(x => x.IsVisible == true)
                .Select(p => new ProductGetDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = (double?)p.Price,
                    Stock = p.Stock,
                    ImagePath = p.ImagePath,
                    IsVisible = p.IsVisible,
                    ProductCategoryId = p.ProductCategoryId,
                    UserId = p.SellerId,
                    Rate = p.Rates.Any() ? p.Rates.Average(r => r.RateValue) : (double?)null,
                    ProductCategory = new ProductCategoryGetDTO
                    {
                        Id = p.ProductCategory.Id,
                        Name = p.ProductCategory.Name
                    },
                    User = new UserGetDTO
                    {
                        Id = p.User.Id,
                        UserName = p.User.UserName,
                        Email = p.User.Email,
                        FullName = p.User.FullName,
                        PhoneNumber = p.User.PhoneNumber
                    }
                }).ToListAsync();

                return new ResponseResult { Object = result, Status = true };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetAllBySeller(string userName)
        {
            try
            {
                var user = await _context.Users
                .Where(u => u.UserName == userName)
                .Include(x => x.Products)
                .ThenInclude(x => x.ProductCategory)
                .Select(u => new
                {
                    u.Id,
                    Products = u.Products,
                    user = u
                }).FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Object = "Login failed try again",
                        Status = false
                    };
                if (!user.Products.Any())
                    return new ResponseResult
                    {
                        Object = "No products found",
                        Status = false
                    };
                return new ResponseResult
                {
                    Object = _mapper.Map<List<ProductGetDTO>>(user.Products),
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> Post(ProductPostDTO productPostDTO)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserName == productPostDTO.userName)
                    .FirstOrDefaultAsync();

                if (user is null)
                    return new ResponseResult { Object = "User not found", Status = false };

                var product = _mapper.Map<Product>(productPostDTO);
                product.SellerId = user.Id;
                product.ImagePath = productPostDTO.ImagePath is not null ? await Upload(productPostDTO.ImagePath) : null;

                var result = await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return new ResponseResult
                {
                    Object = _mapper.Map<ProductGetDTO>(product),
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> Put(ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                if (!await _context.ProductCategory.AnyAsync(x => x.Id == productUpdateDTO.ProductCategoryId))
                    return new ResponseResult
                    {
                        Object = "Category not found",
                        Status = false
                    };
                var product = await _context.Products
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == productUpdateDTO.Id && x.User.UserName == productUpdateDTO.userName);
                if (product is null)
                    return new ResponseResult
                    {
                        Object = "Product not found",
                        Status = false
                    };
                product.ImagePath = productUpdateDTO.ImagePath is not null ? await Upload(productUpdateDTO.ImagePath) : product.ImagePath;

                _mapper.Map(productUpdateDTO, product);
                await _context.SaveChangesAsync();
                return new ResponseResult
                {
                    Object = _mapper.Map<ProductGetDTO>(product),
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> SearchByName(string productName)
        {
            try
            {
                var result = await _context.Products
                    .Where(x => x.Name.Contains(productName) && x.IsVisible == true)
                    .ToListAsync();
                return new ResponseResult
                {
                    Object = _mapper.Map<List<ProductGetDTO>>(result),
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> VisibaltyProduct(bool status, int productId, string userName)
        {
            try
            {
                var product = await _context.Products
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == productId && x.User.UserName == userName);
                if (product == null)
                    return new ResponseResult
                    {
                        Object = "Product not found",
                        Status = false
                    };
                product.IsVisible = status;
                await _context.SaveChangesAsync();

                return new ResponseResult
                {
                    Object = "Updated Successfully",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException!.Message ?? e.Message,
                    Status = false
                };
            }
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

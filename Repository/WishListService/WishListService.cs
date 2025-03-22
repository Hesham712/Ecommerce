using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.WishList;
using Ecommerce.Helper;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.WishListService
{
    public class WishListService : IWishListService
    {
        private readonly EcommerceDBContext _context;
        private readonly IMapper _mapper;
        IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> _userInteractionService;
        public WishListService(IMapper mapper, EcommerceDBContext context, IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> userInteractionService)
        {
            _mapper = mapper;
            _context = context;
            _userInteractionService = userInteractionService;
        }

        public async Task<ResponseResult> AddWishListAsync(WishListPostDTO dto, string userName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.WishList)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User not found"
                    };
                if (user.WishList == null)
                {
                    user.WishList = new WishList() { UserId = user.Id };
                    await _context.SaveChangesAsync();
                    user.WishListId = user.WishList.Id;
                }
                if (!await _context.Products.AnyAsync(x => x.Id == dto.ProductId && x.Stock >= dto.Quantity))
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Product / quantity not correct"
                    };
                var WishListProduct = await _context.WishListProduct
                    .Include(x=>x.Product)
                    .FirstOrDefaultAsync(c => c.WishListId == user.WishListId && c.ProductId == dto.ProductId);

                if (WishListProduct != null)
                    WishListProduct.Quantity = dto.Quantity;
                else
                {
                    WishListProduct = new WishListProduct
                    {
                        WishListId = user.WishList.Id,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity
                    };
                    await _context.WishListProduct.AddAsync(WishListProduct);
                }
                await _userInteractionService.AddAsync(new UserInteraction
                {
                    ProductId = dto.ProductId,
                    UserId = user.Id,
                    InteractionTypeId = 1
                });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ResponseResult
                {
                    Status = true,
                    Object = _mapper.Map<WishListProductGetDTO>(WishListProduct)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseResult
                {
                    Status = false,
                    Object = ex.InnerException!.Message ?? ex.Message
                };
            }
        }

        public async Task<ResponseResult> DeleteProductFromWishListAsync(int productId, string userName)
        {
            try
            {
                var user = await _context.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.WishList)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User / WishList  not found"
                    };
                if (user.WishList == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "WishList not found"
                    };
                var WishListProduct = await _context.WishListProduct
                    .FirstOrDefaultAsync(x => x.WishListId == user.WishList.Id && x.ProductId == productId);
                if (WishListProduct == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Product not found in WishList"
                    };
                _context.WishListProduct.Remove(WishListProduct);
                await _context.SaveChangesAsync();
                return new ResponseResult
                {
                    Status = true,
                    Object = "Deleted Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Status = false,
                    Object = ex.InnerException!.Message ?? ex.Message
                };
            }
        }

        public async Task<ResponseResult> GetWishListAsync(string userName)
        {
            try
            {
                var user = await _context.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.WishList)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User / WishList  not found"
                    };
                if (user.WishList == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "WishList not found"
                    };
                var wishListProduct = await _context.WishListProduct
                    .Where(x => x.WishListId == user.WishList.Id)
                    .Include(x => x.Product)
                    .ToListAsync();
                return new ResponseResult
                {
                    Status = true,
                    Object = _mapper.Map<List<WishListProductGetDTO>>(wishListProduct)
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Status = false,
                    Object = ex.InnerException!.Message ?? ex.Message
                };
            }
        }
    }
}

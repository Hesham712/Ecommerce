using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.Cart;
using Ecommerce.Helper;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Ecommerce.Repository.CartService
{
    public class CartService : ICartService
    {
        private readonly EcommerceDBContext _context;
        private readonly IMapper _mapper;
        public CartService(EcommerceDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseResult> AddCartAsync(CartPostDTO dto, string userName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.Cart)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User not found"
                    };
                if (user.Cart == null)
                {
                    user.Cart = new Cart() { UserId = user.Id };
                    await _context.SaveChangesAsync();
                    user.CartId = user.Cart.Id;
                }
                if (!await _context.Products.AnyAsync(x => x.Id == dto.ProductId && x.Stock >= dto.Quantity))
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Product / quantity not correct"
                    };
                var cartProduct = await _context.CartProduct
                       .FirstOrDefaultAsync(c => c.CartId == user.CartId && c.ProductId == dto.ProductId);

                if (cartProduct != null)
                    cartProduct.Quantity = dto.Quantity;
                else
                {
                    cartProduct = new CartProduct
                    {
                        CartId = user.Cart.Id,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity
                    };
                    await _context.CartProduct.AddAsync(cartProduct);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ResponseResult
                {
                    Status = true,
                    Object = _mapper.Map<CartProductGetDTO>(cartProduct)
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

        public async Task<ResponseResult> DeleteProductFromCartAsync(int productId, string userName)
        {
            try
            {
                var user = await _context.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.Cart)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User / cart  not found"
                    };
                if (user.Cart == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Cart not found"
                    };
                var cartProduct = await _context.CartProduct
                    .FirstOrDefaultAsync(x => x.CartId == user.Cart.Id && x.ProductId == productId);
                if (cartProduct == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Product not found in cart"
                    };
                _context.CartProduct.Remove(cartProduct);
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

        public async Task<ResponseResult> GetCartAsync(string userName)
        {
            try
            {
                var user = await _context.Users
                    .Where(x => x.UserName == userName)
                    .Include(x => x.Cart)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "User / cart  not found"
                    };
                if (user.Cart == null)
                    return new ResponseResult
                    {
                        Status = false,
                        Object = "Cart not found"
                    };
                var cartProduct = await _context.CartProduct
                    .Where(x => x.CartId == user.Cart.Id)
                    .Include(x => x.Product)
                    .ToListAsync();
                return new ResponseResult
                {
                    Status = true,
                    Object = _mapper.Map<List<CartProductGetDTO>>(cartProduct)
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

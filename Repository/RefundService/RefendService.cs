using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.Refund;
using Ecommerce.Helper;
using Ecommerce.Models;
using Ecommerce.Repository.NotificationService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.RefundService
{
    public class RefendService : IRefendService
    {
        private readonly EcommerceDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        public RefendService(IMapper mapper, EcommerceDBContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<ResponseResult> ChangeRefundStatus(RefundStatus status, int refundId)
        {
            try
            {
                var refund = await _context.Refunds
                    .Include(x => x.RefundItems)
                    .FirstOrDefaultAsync(x => x.Id == refundId);
                if (refund is null)
                    return new ResponseResult { Status = false, Object = "Refund not found" };
                if (refund.Status == RefundStatus.Approved && status == RefundStatus.Approved)
                    return new ResponseResult { Status = false, Object = "Refund already approved" };
                refund.Status = status;

                // Notify Customer
                string message = status == RefundStatus.Approved
                    ? "Your refund has been accepted!"
                    : "Your order has been rejected!";


                if (status == RefundStatus.Approved)
                {
                    var order = await _context.Orders
                        .Include(x => x.OrderProduct)
                        .ThenInclude(x => x.Product)
                        .FirstOrDefaultAsync(x => x.Id == refund.OrderId);
                    foreach (var refundItem in refund.RefundItems)
                    {
                        var orderProduct = order.OrderProduct.FirstOrDefault(x => x.Id == refundItem.OrderProductId);
                        orderProduct.Quantity -= refundItem.Quantity;
                        orderProduct.Product.Stock += refundItem.Quantity;
                    }
                    order.TotalPrice = order.OrderProduct.Sum(x => x.Product.Price * x.Quantity);

                    var sellerIds = order.OrderProduct.Select(op => op.Product.SellerId).Distinct().ToList();
                    foreach (var sellerId in sellerIds)
                    {
                        await _notificationService.SendNotification("You have a new refund for one of your products.", sellerId);
                    }
                    await _notificationService.SendNotification(message, order.CustomerId);
                }
                _context.Refunds.Update(refund);
                await _context.SaveChangesAsync();
                return new ResponseResult { Status = true, Object = "Updated Successfully" };
            }
            catch (Exception e)
            {
                return new ResponseResult { Status = false, Object = e.InnerException?.Message ?? e.Message };
            }
        }

        public async Task<ResponseResult> Delete(int refundId)
        {
            try
            {
                var refund = await _context.Refunds.Include(x => x.RefundItems).FirstOrDefaultAsync(x => x.Id == refundId);
                if (refund is null)
                    return new ResponseResult { Status = false, Object = "Refund not found" };
                if (refund.Status == RefundStatus.Approved)
                    return new ResponseResult { Status = false, Object = "Refund already approved" };
                _context.Refunds.Remove(refund);
                await _context.SaveChangesAsync();
                return new ResponseResult { Status = true, Object = "Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Status = false, Object = ex.InnerException?.Message ?? ex.Message };
            }
        }

        public async Task<ResponseResult> Get()
        {
            try
            {
                var refunds = await _context.Refunds
                    .Include(x => x.RefundItems)
                    .Include(x => x.Order)
                    .ThenInclude(x => x.User)
                    .Select(x => new
                    {

                        x.Id,
                        //x.OrderId,
                        x.Status,
                        totalPrice = x.RefundItems.Sum(ri => ri.Quantity * ri.OrderProduct.Product.Price),
                        x.CreatedAt,
                        x.ModifiedAt,
                        RefundItem = x.RefundItems.Select(ri => new
                        {
                            ri.Id,
                            ri.OrderProductId,
                            product = new
                            {
                                ri.OrderProduct.Product.Id,
                                ri.OrderProduct.Product.Name,
                                ri.OrderProduct.Product.Description,
                                ri.OrderProduct.Product.ImagePath,
                                ri.OrderProduct.Product.Price,
                                ri.OrderProduct.Product.Stock
                            },
                            ri.Quantity,
                            ri.Reason
                        }).ToList(),
                        Customer = new
                        {
                            x.Order.User.Id,
                            x.Order.User.FullName,
                            x.Order.User.UserName,
                            x.Order.User.Email,
                            x.Order.User.PhoneNumber
                        },
                    })
                    .ToListAsync();
                return new ResponseResult { Status = true, Object = refunds };
            }
            catch (Exception e)
            {
                return new ResponseResult { Status = false, Object = e.InnerException?.Message ?? e.Message };
            }
        }

        public async Task<ResponseResult> GetById(int refundId)
        {
            try
            {
                var refund = await _context.Refunds
                    .Include(x => x.RefundItems)
                    .Include(x => x.Order)
                    .ThenInclude(x => x.User)
                    .Select(x => new
                    {
                        x.Id,
                        //x.OrderId,
                        x.Status,
                        totalPrice = x.RefundItems.Sum(ri => ri.Quantity * ri.OrderProduct.Product.Price),
                        x.CreatedAt,
                        x.ModifiedAt,
                        RefundItem = x.RefundItems.Select(ri => new
                        {
                            ri.Id,
                            ri.OrderProductId,
                            product = new
                            {
                                ri.OrderProduct.Product.Id,
                                ri.OrderProduct.Product.Name,
                                ri.OrderProduct.Product.Description,
                                ri.OrderProduct.Product.ImagePath,
                                ri.OrderProduct.Product.Price,
                                ri.OrderProduct.Product.Stock
                            },
                            ri.Quantity,
                            ri.Reason
                        }).ToList(),
                        Customer = new
                        {
                            x.Order.User.Id,
                            x.Order.User.FullName,
                            x.Order.User.UserName,
                            x.Order.User.Email,
                            x.Order.User.PhoneNumber
                        },
                    })
                    .FirstOrDefaultAsync(x => x.Id == refundId);
                return new ResponseResult { Status = true, Object = refund };
            }
            catch (Exception e)
            {
                return new ResponseResult { Status = false, Object = e.InnerException?.Message ?? e.Message };
            }
        }

        public async Task<ResponseResult> GetByUser(string userName)
        {
            try
            {
                var user = await _context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                    return new ResponseResult { Status = false, Object = "User not found" };

                bool isSeller = await _userManager.IsInRoleAsync(user, "Seller");

                var refund = _context.Refunds
                    .Include(x => x.RefundItems)
                        .ThenInclude(ri => ri.OrderProduct)
                            .ThenInclude(op => op.Product)
                    .Include(x => x.Order)
                        .ThenInclude(x => x.User)
                    .AsNoTracking()
                    .AsQueryable();

                if (isSeller)
                {
                    refund = refund.Where(x => x.RefundItems.Any(ri => ri.OrderProduct.Product.User.UserName == userName));
                }
                else
                {
                    refund = refund.Where(x => x.Order.User.UserName == userName);
                }
                var refunds = await refund.Select(x => new
                {
                    x.Id,
                    //x.OrderId,
                    x.Status,
                    totalPrice = x.RefundItems.Sum(ri => ri.Quantity * ri.OrderProduct.Product.Price),
                    x.CreatedAt,
                    x.ModifiedAt,
                    RefundItem = x.RefundItems.Select(ri => new
                    {
                        ri.Id,
                        ri.OrderProductId,
                        product = new
                        {
                            ri.OrderProduct.Product.Id,
                            ri.OrderProduct.Product.Name,
                            ri.OrderProduct.Product.Description,
                            ri.OrderProduct.Product.ImagePath,
                            ri.OrderProduct.Product.Price,
                            ri.OrderProduct.Product.Stock
                        },
                        ri.Quantity,
                        ri.Reason,
                        Seller = new
                        {
                            ri.OrderProduct.Product.User.Id,
                            ri.OrderProduct.Product.User.UserName,
                            ri.OrderProduct.Product.User.FullName,
                            ri.OrderProduct.Product.User.Email
                        }
                    }).ToList(),
                    customer = new
                    {
                        x.Order.User.Id,
                        x.Order.User.FullName,
                        x.Order.User.UserName,
                        x.Order.User.Email,
                        x.Order.User.PhoneNumber
                    },
                    Seller = new
                    {

                    }
                }).ToListAsync();

                return new ResponseResult { Status = true, Object = refunds };
            }
            catch (Exception e)
            {
                return new ResponseResult { Status = false, Object = e.InnerException?.Message ?? e.Message };
            }
        }

        public async Task<ResponseResult> Post(RefundPostDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(x => x.Refund)
                    .ThenInclude(x => x.RefundItems)
                    .Include(x => x.OrderProduct)
                    .FirstOrDefaultAsync(x => x.Id == dto.OrderId);

                if (order is null || !order.OrderProduct.Any(op => op.Id == dto.RefundItems.OrderProductId))
                    return new ResponseResult { Status = false, Object = "Order or OrderProduct not found" };

                var orderProduct = order.OrderProduct.FirstOrDefault(op => op.Id == dto.RefundItems.OrderProductId);
                if (orderProduct is null || dto.RefundItems.Quantity > orderProduct.Quantity)
                    return new ResponseResult { Status = false, Object = "Invalid quantity" };

                var totalRefundedQuantity = order.Refund?.RefundItems
                    .Where(x => x.OrderProductId == orderProduct.Id)
                    .Sum(x => x.Quantity) ?? 0;

                if (dto.RefundItems.Quantity + totalRefundedQuantity > orderProduct.Quantity)
                    return new ResponseResult { Status = false, Object = "Total refund quantity exceeds available quantity" };


                Refund refund;
                if (order.Refund is null)
                {
                    refund = new Refund
                    {
                        OrderId = dto.OrderId,
                        RefundItems = new List<RefundItem>()
                    };
                    await _context.Refunds.AddAsync(refund);
                    await _context.SaveChangesAsync();
                    order.RefundId = refund.Id;
                }
                else
                {
                    refund = order.Refund;
                }
                if (refund.RefundItems.Any(x => x.OrderProductId == dto.RefundItems.OrderProductId))
                {
                    refund.RefundItems.FirstOrDefault(x => x.OrderProductId == dto.RefundItems.OrderProductId).Quantity += dto.RefundItems.Quantity;
                }
                else
                {
                    var refundItem = new RefundItem
                    {
                        RefundId = refund.Id,
                        OrderProductId = dto.RefundItems.OrderProductId,
                        Quantity = dto.RefundItems.Quantity,
                        Reason = dto.RefundItems.Reason
                    };
                    refund.RefundItems.Add(refundItem);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseResult { Status = true, Object = _mapper.Map<RefundGetDTO>(refund) };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new ResponseResult { Status = false, Object = e.InnerException?.Message ?? e.Message };
            }
        }
    }
}

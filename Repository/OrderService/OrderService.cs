using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.Order;
using Ecommerce.DTO_s.OrderProduct;
using Ecommerce.Helper;
using Ecommerce.Models;
using Ecommerce.Repository.GenericService;
using Ecommerce.Repository.NotificationService;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly EcommerceDBContext _context;
        private readonly IMapper _mapper;
        IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> _userInteractionService;
        private readonly INotificationService _notificationService;
        public OrderService(IMapper mapper, EcommerceDBContext context, IGenericBasicDataRepo<UserInteraction, EcommerceDBContext> userInteractionService, INotificationService notificationService)
        {
            _mapper = mapper;
            _context = context;
            _userInteractionService = userInteractionService;
            _notificationService = notificationService;
        }

        public async Task<ResponseResult> UpdateOrderStatus(int OrderId, OrderStatus status)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var Order = await _context.Orders
                    .Include(o => o.OrderProduct)
                    .ThenInclude(op => op.Product)
                    .FirstOrDefaultAsync(o => o.Id == OrderId);
                if (Order == null)
                    return new ResponseResult { Object = "Order not found", Status = false };
                // Notify Customer
                string message = status == OrderStatus.Shipped
                    ? "Your order has been accepted!"
                    : "Your order has been rejected!";

                await _notificationService.SendNotification(message, Order.CustomerId);

                // تحديث حالة الطلب بناءً على الحالة المطلوبة
                if (status == OrderStatus.Shipped)
                {
                    Order.Status = status;
                    _context.Orders.Update(Order);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseResult { Object = $"Order {status} successfully", Status = true };
                }
                else if (status == OrderStatus.Cancelled)
                {
                    Order.Status = status;
                    _context.Orders.Update(Order);
                    foreach (var orderProduct in Order.OrderProduct)
                    {
                        var product = await _context.Products.FindAsync(orderProduct.ProductId);
                        if (product == null)
                            return new ResponseResult { Object = "Product not found", Status = false };
                        product.Stock += orderProduct.Quantity;
                        _context.Products.Update(product);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ResponseResult { Object = $"Order {status} successfully", Status = true };
                }
                

                return new ResponseResult { Object = "Invalid status", Status = false };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseResult
                {
                    Object = ex.InnerException?.Message ?? ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> Checkout(OrderPostDTO dto)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Cart)
                    .ThenInclude(c => c.CartProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

                if (user == null || user.Cart == null || !user.Cart.CartProducts.Any())
                    return new ResponseResult { Object = "Cart is empty or user not found", Status = false };

                var order = new Order
                {
                    CustomerId = user.Id,
                    Status = OrderStatus.Pending,
                    ShippingAddress = dto.ShippingAddress,
                    TotalPrice = 0,
                    OrderProduct = new List<OrderProduct>()
                };

                foreach (var cartItem in user.Cart.CartProducts)
                {
                    if (cartItem.Product == null)
                        return new ResponseResult { Object = "Product not found", Status = false };

                    if (cartItem.Product.Stock < cartItem.Quantity)
                        return new ResponseResult { Object = $"Product {cartItem.Product.Name} is out of stock", Status = false };

                    cartItem.Product.Stock -= cartItem.Quantity;
                    _context.Products.Update(cartItem.Product);

                    await _userInteractionService.AddAsync(new UserInteraction
                    {
                        ProductId = cartItem.Product.Id,
                        UserId = user.Id,
                        InteractionTypeId = 3
                    });
                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.Product.Id,
                        Quantity = cartItem.Quantity
                    };

                    order.OrderProduct.Add(orderProduct);
                    order.TotalPrice += cartItem.Product.Price * cartItem.Quantity;
                }

                _context.Orders.Add(order);

                // Clear the cart
                _context.CartProduct.RemoveRange(user.Cart.CartProducts);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var sellerIds = order.OrderProduct.Select(op => op.Product.SellerId).Distinct().ToList();
                foreach (var sellerId in sellerIds)
                {
                    await _notificationService.SendNotification("You have a new order for one of your products.", sellerId);
                }

                return new ResponseResult { Object = _mapper.Map<OrderGetDTO>(order), Status = true };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new ResponseResult
                {
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetAll()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderProduct)
                    .ThenInclude(op => op.Product)
                    .ToListAsync();
                if (!orders.Any())
                    return new ResponseResult { Object = "Order not found", Status = false };
                return new ResponseResult { Object = _mapper.Map<List<OrderGetDTO>>(orders), Status = true };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetByCustomer(string UserName)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(so => so.OrderProduct)
                    .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.User)
                    .Where(x => x.User.UserName == UserName)
                    .ToListAsync();

                if (!orders.Any())
                    return new ResponseResult { Object = "Order not found", Status = false };
                return new ResponseResult { Object = _mapper.Map<List<OrderGetDTO>>(orders), Status = true };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetById(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(so => so.OrderProduct)
                    .ThenInclude(op => op.Product)
                    .FirstOrDefaultAsync(x => x.Id == orderId);

                if (order == null)
                    return new ResponseResult { Object = "Order not found", Status = false };
                return new ResponseResult { Object = _mapper.Map<OrderGetDTO>(order), Status = true };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> GetBySeller(string userName)
        {
            try
            {
                //Seller user
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == userName);
                if (user == null)
                    return new ResponseResult { Object = "User not found", Status = false };

                var orders = await _context.Orders
                    .Where(o => o.OrderProduct.Any(op => op.Product.SellerId == user.Id))
                    .Include(x => x.User)
                    .Include(o => o.OrderProduct)
                    .ThenInclude(op => op.Product)
                    .Select(o => new
                    {
                        Order = o,
                        SellerTotalPrice = o.OrderProduct
                            .Where(op => op.Product.SellerId == user.Id)
                            .Sum(op => op.Product.Price * op.Quantity)
                    })
                    .ToListAsync();

                if (!orders.Any())
                    return new ResponseResult { Object = "No orders found for this seller.", Status = false };

                var result = orders.Select(o => new OrderGetDTO
                {
                    Id = o.Order.Id,
                    CustomerId = o.Order.User.Id,
                    Status = o.Order.Status,
                    ShippingAddress = o.Order.ShippingAddress,
                    TotalPrice = o.SellerTotalPrice,
                    OrderProduct = _mapper.Map<List<OrderProductGetDTO>>(o.Order.OrderProduct
                        .Where(op => op.Product.SellerId == user.Id)).ToList()
                }).ToList();

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
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResponseResult> Delete(int OrderId, string userName)
        {
            try
            {
                var order = await _context.Orders
                    .Include(x => x.OrderProduct)
                    .FirstOrDefaultAsync(x => x.Id == OrderId && x.User.UserName == userName);
                if (order == null)
                    return new ResponseResult { Object = "Order not found", Status = false };
                if (order.Status != OrderStatus.Pending)
                    return new ResponseResult { Object = "Order can't be deleted", Status = false };
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return new ResponseResult { Object = "Order deleted successfully", Status = true };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }
        public async Task<ResponseResult> DeleteOrderItem(int OrderId, int OrderItemId, string userName)
        {
            try
            {
                var order = await _context.Orders
                    .Include(x => x.OrderProduct)
                    .ThenInclude(x => x.Product)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == OrderId && x.User.UserName == userName);
                if (order == null)
                    return new ResponseResult { Object = "Order not found", Status = false };
                if (order.Status != OrderStatus.Pending)
                    return new ResponseResult { Object = "OrderItem can't be deleted", Status = false };

                var orderItem = order.OrderProduct.FirstOrDefault(op => op.Id == OrderItemId);
                if (orderItem == null)
                    return new ResponseResult { Object = "OrderItem not found", Status = false };
                _context.OrderProduct.Remove(orderItem);

                order.TotalPrice = order.OrderProduct
                    .Where(op => op.Id != OrderItemId)
                    .Sum(op => op.Product.Price * op.Quantity);

                await _context.SaveChangesAsync();
                return new ResponseResult { Object = "Order Item deleted successfully", Status = true };
            }
            catch (Exception e)
            {
                return new ResponseResult
                {
                    Object = e.InnerException?.Message ?? e.Message,
                    Status = false
                };
            }
        }
    }
}

//using Ecommerce.Data;
//using Ecommerce.Models;
//using Ecommerce.Repository.NotificationService;
//using Microsoft.EntityFrameworkCore;

//public class OrderNotificationService
//{
//    private readonly EcommerceDBContext _context;
//    private readonly ILogger<OrderNotificationService> _logger;
//    private readonly IPushNotificationSender _pushSender;

//    public OrderNotificationService(
//        EcommerceDBContext context,
//        ILogger<OrderNotificationService> logger,
//        IPushNotificationSender pushSender)
//    {
//        _context = context;
//        _logger = logger;
//        _pushSender = pushSender;
//    }

//    /// <summary>
//    /// Send notification to seller about new order
//    /// </summary>
//    public async Task NotifySellerAboutNewOrder(int orderId)
//    {
//        try
//        {
//            // Get order with products and user details
//            var order = await _context.Orders
//                .Include(o => o.OrderProduct)
//                .ThenInclude(op => op.Product)
//                .Include(o => o.User)
//                .FirstOrDefaultAsync(o => o.Id == orderId);

//            if (order == null)
//            {
//                _logger.LogWarning($"Order not found for notification: {orderId}");
//                return;
//            }

//            // Group order products by seller (based on product.CreatedById)
//            var productsBySeller = order.OrderProduct
//                .GroupBy(op => op.Product.CreatedById)
//                .ToList();

//            // For each seller, create and send notification
//            foreach (var sellerGroup in productsBySeller)
//            {
//                string sellerId = sellerGroup.Key;

//                // Get seller details
//                var seller = await _context.Users
//                    .FirstOrDefaultAsync(u => u.Id == sellerId);

//                if (seller == null)
//                {
//                    _logger.LogWarning($"Seller not found for notification: {sellerId}");
//                    continue;
//                }

//                // Create notification record
//                var notification = new Notification
//                {
//                    UserId = sellerId,
//                    Message = $"You have a new order #{order.Id} from {order.User.UserName}",
//                    IsRead = false,
//                    CreatedAt = DateTime.UtcNow,
//                    ModifiedAt = DateTime.UtcNow
//                };

//                _context.Notifications.Add(notification);

//                // Build email with order details
//                var sellerProducts = sellerGroup.ToList();
//                decimal totalOrderValue = sellerProducts.Sum(op => op.Product.Price * op.Quantity);
//                int totalItems = sellerProducts.Sum(op => op.Quantity);

//                string productList = string.Join("<br>", sellerProducts.Select(op =>
//                    $"{op.Product.Name} x{op.Quantity} - ${op.Product.Price * op.Quantity}"));

//                string emailBody = $@"
//                    <h2>New Order #{order.Id}</h2>
//                    <p>You have received a new order from {order.User.FullName} ({order.User.UserName}).</p>
//                    <h3>Order Details:</h3>
//                    <p>Order Date: {order.CreatedAt:yyyy-MM-dd HH:mm}</p>
//                    <p>Total Items: {totalItems}</p>
//                    <p>Total Value: ${totalOrderValue}</p>
//                    <h3>Products:</h3>
//                    <p>{productList}</p>
//                    <p>Please login to your seller dashboard to accept this order.</p>
//                ";

//                //// Send email notification
//                //await _emailSender.SendEmailAsync(
//                //    seller.Email,
//                //    $"New Order #{order.Id} Received",
//                //    emailBody);

//                // Send push notification if possible
//                if (!string.IsNullOrEmpty(seller.DeviceToken))
//                {
//                    await _pushSender.SendPushNotificationAsync(
//                        seller.DeviceToken,
//                        "New Order Received",
//                        $"You have a new order #{order.Id} worth ${totalOrderValue}. Tap to view details.");
//                }
//            }

//            await _context.SaveChangesAsync();
//            _logger.LogInformation($"Successfully sent new order notifications for order {orderId}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error sending new order notifications for order {orderId}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Send notification to customer when seller accepts order
//    /// </summary>
//    public async Task NotifyCustomerAboutOrderAcceptance(int orderId, string sellerId)
//    {
//        try
//        {
//            // Get order with customer details
//            var order = await _context.Orders
//                .Include(o => o.User)
//                .Include(o => o.OrderProduct)
//                    .ThenInclude(op => op.Product)
//                .FirstOrDefaultAsync(o => o.Id == orderId);

//            if (order == null)
//            {
//                _logger.LogWarning($"Order not found for acceptance notification: {orderId}");
//                return;
//            }

//            // Get seller details
//            var seller = await _context.Users
//                .FirstOrDefaultAsync(u => u.Id == sellerId);

//            if (seller == null)
//            {
//                _logger.LogWarning($"Seller not found for acceptance notification: {sellerId}");
//                return;
//            }

//            // Get products for this seller in this order
//            var sellerProducts = order.OrderProduct
//                .Where(op => op.Product.CreatedById == sellerId)
//                .ToList();

//            if (!sellerProducts.Any())
//            {
//                _logger.LogWarning($"No products found for seller {sellerId} in order {orderId}");
//                return;
//            }

//            // Create in-app notification
//            var notification = new Notification
//            {
//                UserId = order.User.Id,
//                Message = $"Your order #{order.Id} has been accepted by {seller.FullName}",
//                IsRead = false,
//                CreatedAt = DateTime.UtcNow,
//                ModifiedAt = DateTime.UtcNow
//            };

//            _context.Notifications.Add(notification);

//            // Build email with accepted products details
//            string productList = string.Join("<br>", sellerProducts.Select(op =>
//                $"{op.Product.Name} x{op.Quantity} - ${op.Product.Price * op.Quantity}"));

//            decimal totalValue = sellerProducts.Sum(op => op.Product.Price * op.Quantity);

//            string emailBody = $@"
//                <h2>Order #{order.Id} Accepted</h2>
//                <p>Good news! Seller {seller.FullName} has accepted your order.</p>
//                <h3>Accepted Products:</h3>
//                <p>{productList}</p>
//                <p>Total: ${totalValue}</p>
//                <p>The seller will now prepare your items for shipping. You will receive another notification when your order ships.</p>
//                <p>Thank you for your purchase!</p>
//            ";

//            //// Send email notification
//            //await _emailSender.SendEmailAsync(
//            //    order.User.Email,
//            //    $"Order #{order.Id} Has Been Accepted",
//            //    emailBody);

//            // Send push notification if possible
//            if (!string.IsNullOrEmpty(order.User.DeviceToken))
//            {
//                await _pushSender.SendPushNotificationAsync(
//                    order.User.DeviceToken,
//                    "Order Accepted",
//                    $"Your order #{order.Id} has been accepted by {seller.FullName}. Tap to view details.");
//            }

//            await _context.SaveChangesAsync();
//            _logger.LogInformation($"Successfully sent order acceptance notification for order {orderId}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error sending order acceptance notification for order {orderId}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Mark notification as read
//    /// </summary>
//    public async Task MarkNotificationAsRead(int notificationId)
//    {
//        try
//        {
//            var notification = await _context.Notifications
//                .FirstOrDefaultAsync(n => n.Id == notificationId);

//            if (notification == null)
//            {
//                _logger.LogWarning($"Notification not found: {notificationId}");
//                return;
//            }

//            notification.IsRead = true;
//            notification.ModifiedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            _logger.LogInformation($"Marked notification {notificationId} as read");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error marking notification {notificationId} as read");
//            throw;
//        }
//    }
//}

using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTO_s.Notification;
using Ecommerce.Helper;
using Ecommerce.Models;
using Ecommerce.Repository.NotificationService;
using Microsoft.EntityFrameworkCore;

public class NotificationService : INotificationService
{
    private readonly EcommerceDBContext _context;
    private readonly IMapper _mapper;
    public NotificationService(EcommerceDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResponseResult> GetUserNotifications(string userId)
    {
        var reuslt = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return new ResponseResult
        {
            Object = _mapper.Map<List<NotificationGetDTO>>(reuslt),
            Status = true
        };
    }

    public async Task<ResponseResult> MarkNotificationAsRead(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return new ResponseResult
            {
                Status = true,
                Object = "Notification marked as read"
            };
        }
        return new ResponseResult
        {
            Status = false,
            Object = "Notification not found"
        };
    }

    public async Task SendNotification(string message, string userId)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
using GoWhere_Api.Hubs;
using GoWhere_Api.Service.IService;
using Microsoft.AspNetCore.SignalR;

namespace GoWhere_Api.Service.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAdminNewOrderAsync(int orderId, int userId, double amount, string userName)
        {
            var newOrder = new
            {
                Type = "New_Order",
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                UserName = userName,
                Time = DateTime.UtcNow,
                Message = $"New order placed by {userName} (User ID: {userId}) for amount ${amount}."

            };
            try
            {
                await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", newOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying admins of new order");
            }
        }

        public Task NotifyAllAdminsAsync(string message, object? data = null)
        {
            throw new NotImplementedException();
        }

        public Task NotifyUserAsync(int userId, string message, object? data = null)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyUserPaymentSuccessAsync(int userId, int orderId, double amount)
        {
            var paymentSuccess = new
            {
                Type = "Payment_Success",
                OrderId = orderId,
                Amount = amount,
                Time = DateTime.UtcNow,
                Message = $"Your payment for order {orderId} of amount ${amount} was successful."
            };
            try
            {
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", paymentSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error notifying user {userId} of payment success");
            }
        }
    }
}

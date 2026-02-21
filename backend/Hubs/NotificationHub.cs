using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GoWhere_Api.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            var userid = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = Context.User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation($"User connected: {userid}, Role: {role}");
            if(role == "Admin")
            {
               await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");


            }
            else if(!string.IsNullOrEmpty(userid))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User-{userid}");
            }
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userid = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"User disconnected: {userid}");
            await base.OnDisconnectedAsync(exception);

        }
        // Test 
        public async Task SendNotification(string message)
        {
            var userid = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"Received notification from user {userid}: {message}");
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}

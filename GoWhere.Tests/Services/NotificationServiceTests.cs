using GoWhere_Api.Hubs;
using GoWhere_Api.Service.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GoWhere.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IHubContext<NotificationHub>> _hubContextMock;
        private readonly Mock<ILogger<NotificationService>> _loggerMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        private readonly Mock<IHubClients> _hubClientsMock;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _loggerMock = new Mock<ILogger<NotificationService>>();
            _clientProxyMock = new Mock<IClientProxy>();
            _hubClientsMock = new Mock<IHubClients>();

            _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
            _hubClientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubClientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_clientProxyMock.Object);

            _notificationService = new NotificationService(
                _hubContextMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task NotifyAdminNewOrderAsync_ShouldSendNotificationToAdminsGroup()
        {
            // Arrange
            var orderId = 100;
            var userId = 1;
            var amount = 50000.0;
            var userName = "Test User";

            // Act
            await _notificationService.NotifyAdminNewOrderAsync(orderId, userId, amount, userName);

            // Assert
            _hubClientsMock.Verify(c => c.Group("Admins"), Times.Once);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync(
                    "ReceiveNotification",
                    It.Is<object[]>(o => o.Length == 1),
                    default
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task NotifyUserPaymentSuccessAsync_ShouldSendNotificationToUserGroup()
        {
            // Arrange
            var userId = 1;
            var orderId = 100;
            var amount = 50000.0;

            // Act
            await _notificationService.NotifyUserPaymentSuccessAsync(userId, orderId, amount);

            // Assert
            _hubClientsMock.Verify(c => c.User(userId.ToString()), Times.Once);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync(
                    "ReceiveNotification",
                    It.Is<object[]>(o => o.Length == 1),
                    default
                ),
                Times.Once
            );
        }
    }
}

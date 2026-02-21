using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Repo.IRepo;
using GoWhere_Api.Service.IService;
using GoWhere_Api.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;

namespace GoWhere.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IOrderRepo> _orderRepoMock;
        private readonly Mock<IAISuggestionRequestRepo> _aiSuggestionRepoMock;
        private readonly Mock<IUserRepo> _userRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<PaymentService>> _loggerMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly PaymentService _paymentService;

        public PaymentServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepo>();
            _aiSuggestionRepoMock = new Mock<IAISuggestionRequestRepo>();
            _userRepoMock = new Mock<IUserRepo>();
            _emailServiceMock = new Mock<IEmailService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<PaymentService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            // Setup configuration
            _configurationMock.Setup(c => c["VietQR:BankBin"]).Returns("970422");
            _configurationMock.Setup(c => c["VietQR:AccountNo"]).Returns("083001022003");
            _configurationMock.Setup(c => c["VietQR:AccountName"]).Returns("Dinh Ngoc Hung");
            _configurationMock.Setup(c => c["VietQR:Template"]).Returns("compact2");

            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            _paymentService = new PaymentService(
                _orderRepoMock.Object,
                _notificationServiceMock.Object,
                _aiSuggestionRepoMock.Object,
                _userRepoMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object,
                _loggerMock.Object,
                _httpClientFactoryMock.Object
            );
        }

        [Fact]
        public async Task CreateAIChatPaymentAsync_WithNewUser_ShouldCreateNewOrder()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                RoleId = 1
            };

            _orderRepoMock.Setup(r => r.GetByUserAndStatusAsync(userId, "Pending"))
                .ReturnsAsync((Order?)null);

            _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order order) =>
                {
                    order.OrderId = 1;
                    return order;
                });

            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _paymentService.CreateAIChatPaymentAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.TotalPrice.Should().Be(50000);
            result.QRCodeUrl.Should().Contain("vietqr.io");
            result.BankAccount.Should().Be("083001022003");

            _orderRepoMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
            _notificationServiceMock.Verify(
                n => n.NotifyAdminNewOrderAsync(It.IsAny<int>(), userId, 50000, "testuser"),
                Times.Once
            );
            _emailServiceMock.Verify(
                e => e.SendPaymentCreatedEmailAsync(
                    "test@example.com",
                    "Test User",
                    It.IsAny<int>(),
                    50000,
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAIChatPaymentAsync_WithExistingPendingOrder_ShouldReturnExistingOrder()
        {
            // Arrange
            var userId = 1;
            var existingOrder = new Order
            {
                OrderId = 100,
                UserId = userId,
                TotalPrice = 50000,
                Status = "Pending",
                OrderDate = DateTime.Now
            };

            _orderRepoMock.Setup(r => r.GetByUserAndStatusAsync(userId, "Pending"))
                .ReturnsAsync(existingOrder);

            // Act
            var result = await _paymentService.CreateAIChatPaymentAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(100);
            result.TotalPrice.Should().Be(50000);

            _orderRepoMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Never);
            _notificationServiceMock.Verify(
                n => n.NotifyAdminNewOrderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task ConfirmPaymentAsync_WithValidOrder_ShouldUpdateOrderStatusAndNotifyUser()
        {
            // Arrange
            var orderId = 100;
            var userId = 1;
            var order = new Order
            {
                OrderId = orderId,
                UserId = userId,
                TotalPrice = 50000,
                Status = "Pending",
                OrderDate = DateTime.Now
            };

            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                RoleId = 1
            };

            _orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _aiSuggestionRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync((AISuggestionRequest?)null);

            // Act
            var result = await _paymentService.ConfirmPaymentAsync(orderId);

            // Assert
            result.Should().BeTrue();

            _orderRepoMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Status == "Payed")), Times.Once);
            _notificationServiceMock.Verify(
                n => n.NotifyUserPaymentSuccessAsync(userId, orderId, 50000),
                Times.Once
            );
            _emailServiceMock.Verify(
                e => e.SendPaymentConfirmationEmailAsync("test@example.com", "testuser", orderId, 50000),
                Times.Once
            );
        }

        [Fact]
        public async Task ConfirmPaymentAsync_WithNonExistentOrder_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var orderId = 999;

            _orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _paymentService.ConfirmPaymentAsync(orderId)
            );
        }

        [Fact]
        public async Task ConfirmPaymentAsync_WithAlreadyPayedOrder_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var orderId = 100;
            var order = new Order
            {
                OrderId = orderId,
                UserId = 1,
                TotalPrice = 50000,
                Status = "Payed",
                OrderDate = DateTime.Now
            };

            _orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _paymentService.ConfirmPaymentAsync(orderId)
            );
        }

        [Fact]
        public async Task GetUserPaymentHistoryAsync_ShouldReturnAllUserOrders()
        {
            // Arrange
            var userId = 1;
            var orders = new List<Order>
            {
                new Order { OrderId = 1, UserId = userId, TotalPrice = 50000, Status = "Payed", OrderDate = DateTime.Now },
                new Order { OrderId = 2, UserId = userId, TotalPrice = 50000, Status = "Pending", OrderDate = DateTime.Now }
            };

            _orderRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(orders);

            // Act
            var result = await _paymentService.GetUserPaymentHistoryAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result[0].OrderId.Should().Be(1);
            result[1].OrderId.Should().Be(2);
        }
    }
}

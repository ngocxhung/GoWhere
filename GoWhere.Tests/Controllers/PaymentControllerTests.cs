using GoWhere_Api.Controllers;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using System.Security.Claims;

namespace GoWhere.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ILogger<PaymentController>> _loggerMock;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<PaymentController>>();

            _controller = new PaymentController(
                _paymentServiceMock.Object,
                _loggerMock.Object,
                _emailServiceMock.Object
            );

            // Setup controller context with authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [Fact]
        public async Task CreateAIChatPayment_WithValidUser_ShouldReturnOkResult()
        {
            // Arrange
            var expectedResponse = new PaymentResponseDto
            {
                OrderId = 1,
                TotalPrice = 50000,
                Status = "Pending",
                OrderDate = DateTime.Now,
                QRCodeUrl = "https://example.com/qr.png",
                PaymentContent = "PAYMENT1",
                BankAccount = "123456789",
                BankName = "Test Bank"
            };

            _paymentServiceMock.Setup(s => s.CreateAIChatPaymentAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateAIChatPayment();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            
            _paymentServiceMock.Verify(s => s.CreateAIChatPaymentAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateAIChatPayment_WithInvalidUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.CreateAIChatPayment();

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task CreateAIChatPayment_WhenServiceThrowsException_ShouldReturn500()
        {
            // Arrange
            _paymentServiceMock.Setup(s => s.CreateAIChatPaymentAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test error"));

            // Act
            var result = await _controller.CreateAIChatPayment();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var statusResult = result as ObjectResult;
            statusResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task ConfirmPayment_WithValidOrderId_ShouldReturnOkResult()
        {
            // Arrange
            var orderId = 100;
            _paymentServiceMock.Setup(s => s.ConfirmPaymentAsync(orderId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ConfirmPayment(orderId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _paymentServiceMock.Verify(s => s.ConfirmPaymentAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task ConfirmPayment_WithNonExistentOrder_ShouldReturnNotFound()
        {
            // Arrange
            var orderId = 999;
            _paymentServiceMock.Setup(s => s.ConfirmPaymentAsync(orderId))
                .ThrowsAsync(new KeyNotFoundException("Order not found"));

            // Act
            var result = await _controller.ConfirmPayment(orderId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ConfirmPayment_WithAlreadyPaidOrder_ShouldReturnBadRequest()
        {
            // Arrange
            var orderId = 100;
            _paymentServiceMock.Setup(s => s.ConfirmPaymentAsync(orderId))
                .ThrowsAsync(new InvalidOperationException("Already paid"));

            // Act
            var result = await _controller.ConfirmPayment(orderId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetPaymentHistory_WithValidUser_ShouldReturnOkResult()
        {
            // Arrange
            var expectedHistory = new List<PaymentResponseDto>
            {
                new PaymentResponseDto { OrderId = 1, TotalPrice = 50000, Status = "Payed" },
                new PaymentResponseDto { OrderId = 2, TotalPrice = 50000, Status = "Pending" }
            };

            _paymentServiceMock.Setup(s => s.GetUserPaymentHistoryAsync(1))
                .ReturnsAsync(expectedHistory);

            // Act
            var result = await _controller.GetPaymentHistory();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            
            _paymentServiceMock.Verify(s => s.GetUserPaymentHistoryAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetPaymentHistory_WithInvalidUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetPaymentHistory();

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task TestEmail_WithValidEmail_ShouldReturnOkResult()
        {
            // Arrange
            var toEmail = "test@example.com";

            // Act
            var result = await _controller.TestEmail(toEmail);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            
            _emailServiceMock.Verify(
                e => e.SendPaymentCreatedEmailAsync(
                    toEmail,
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<double>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task TestEmail_WhenEmailFails_ShouldReturn500()
        {
            // Arrange
            var toEmail = "test@example.com";
            _emailServiceMock.Setup(e => e.SendPaymentCreatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<double>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .ThrowsAsync(new Exception("SMTP error"));

            // Act
            var result = await _controller.TestEmail(toEmail);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var statusResult = result as ObjectResult;
            statusResult!.StatusCode.Should().Be(500);
        }
    }
}

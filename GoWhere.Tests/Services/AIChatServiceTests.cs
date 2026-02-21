using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Repo.IRepo;
using GoWhere_Api.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using FluentAssertions;
using Xunit;
using System.Net;

namespace GoWhere.Tests.Services
{
    public class AIChatServiceTests
    {
        private readonly Mock<IAISuggestionRequestRepo> _aiSuggestionRepoMock;
        private readonly Mock<IScenicSpotRepo> _scenicSpotRepoMock;
        private readonly Mock<IProvinceRepo> _provinceRepoMock;
        private readonly Mock<IAIScheduleHistoryRepo> _scheduleHistoryRepoMock;
        private readonly Mock<IOrderRepo> _orderRepoMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AIChatService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly AIChatService _aiChatService;

        public AIChatServiceTests()
        {
            _aiSuggestionRepoMock = new Mock<IAISuggestionRequestRepo>();
            _scenicSpotRepoMock = new Mock<IScenicSpotRepo>();
            _provinceRepoMock = new Mock<IProvinceRepo>();
            _scheduleHistoryRepoMock = new Mock<IAIScheduleHistoryRepo>();
            _orderRepoMock = new Mock<IOrderRepo>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AIChatService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // Setup configuration
            _configurationMock.Setup(c => c["GeminiAPI:ApiKey"]).Returns("test-api-key");

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);

            _aiChatService = new AIChatService(
                _aiSuggestionRepoMock.Object,
                _scenicSpotRepoMock.Object,
                _provinceRepoMock.Object,
                _scheduleHistoryRepoMock.Object,
                _loggerMock.Object,
                _orderRepoMock.Object,
                _configurationMock.Object,
                httpClientFactoryMock.Object
            );
        }

        [Fact]
        public async Task CheckUsageLimitAsync_WithNoExistingRequest_ShouldReturnZeroCount()
        {
            // Arrange
            var userId = 1;
            _aiSuggestionRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync((AISuggestionRequest?)null);

            // Act
            var result = await _aiChatService.CheckUsageLimitAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.TotalRequests.Should().Be(0);
            result.FreeLimit.Should().Be(2);
            result.RemainingFreeRequests.Should().Be(2);
            result.RequiresPayment.Should().BeFalse();
        }

        [Fact]
        public async Task CheckUsageLimitAsync_WithExistingUnpaidRequest_ShouldReturnCorrectCount()
        {
            // Arrange
            var userId = 1;
            var existingRequest = new AISuggestionRequest
            {
                UserId = userId,
                Count = 1,
                OrderId = null,
                Price = 0
            };

            _aiSuggestionRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(existingRequest);

            // Act
            var result = await _aiChatService.CheckUsageLimitAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.TotalRequests.Should().Be(1);
            result.FreeLimit.Should().Be(2);
            result.RemainingFreeRequests.Should().Be(1);
            result.RequiresPayment.Should().BeFalse();
        }

        [Fact]
        public async Task CheckUsageLimitAsync_WithPaidUser_ShouldReturnUnlimitedAccess()
        {
            // Arrange
            var userId = 1;
            var paidRequest = new AISuggestionRequest
            {
                UserId = userId,
                Count = 5,
                OrderId = 100,
                Price = 50000,
                Order = new Order { OrderId = 100, Status = "Payed" }
            };

            _aiSuggestionRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(paidRequest);

            // Act
            var result = await _aiChatService.CheckUsageLimitAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.TotalRequests.Should().Be(5);
            result.RequiresPayment.Should().BeFalse();
        }

        [Fact]
        public async Task CheckUsageLimitAsync_WithExceededFreeLimit_ShouldReturnRequiresPayment()
        {
            // Arrange
            var userId = 1;
            var exceededRequest = new AISuggestionRequest
            {
                UserId = userId,
                Count = 2,
                OrderId = null,
                Price = 0
            };

            _aiSuggestionRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(exceededRequest);

            // Act
            var result = await _aiChatService.CheckUsageLimitAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.TotalRequests.Should().Be(2);
            result.RequiresPayment.Should().BeTrue();
            result.PaymentAmount.Should().Be(50000);
        }
    }
}

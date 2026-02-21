using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Repo.IRepo;
using GoWhere_Api.Service.IService;
using System.Text;
using System.Text.Json;

namespace GoWhere_Api.Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IAISuggestionRequestRepo _aiSuggestionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PaymentService> _logger;
        private readonly HttpClient _httpClient;
        private const decimal AI_CHAT_PRICE = 50000; // 50,000 VND

        public PaymentService(
            IOrderRepo orderRepo,
            INotificationService notificationService,
            IAISuggestionRequestRepo aiSuggestionRepo,
            IUserRepo userRepo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<PaymentService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _orderRepo = orderRepo;
            _aiSuggestionRepo = aiSuggestionRepo;
            _userRepo = userRepo;
            _emailService = emailService;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<PaymentResponseDto> CreateAIChatPaymentAsync(int userId)
        {
            // Kiểm tra xem đã có đơn hàng Pending chưa
            var existingOrder = await _orderRepo.GetByUserAndStatusAsync(userId, OrderStatus.Pending.ToString());
            if (existingOrder != null)
            {
                // Trả về đơn hàng đang pending
                var existingQR = await GenerateVietQRAsync(existingOrder);
                return existingQR;
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                UserId = userId,
                TotalPrice = (double)AI_CHAT_PRICE,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending.ToString()
            };

            var createdOrder = await _orderRepo.CreateAsync(order);

            // Tạo QR code thanh toán
            var paymentResponse = await GenerateVietQRAsync(createdOrder);
            // Thông báo đến admin về đơn hàng mới
            try
            {
                var orderId = createdOrder.OrderId;
                var customerId = createdOrder.UserId;
                var amount = createdOrder.TotalPrice;
                var customerUserName = (await _userRepo.GetUserByIdAsync(customerId))?.Username ?? "Unknown";
                await _notificationService.NotifyAdminNewOrderAsync(orderId, customerId, amount, customerUserName);
                _logger.LogInformation("Admin notified about new order. OrderId: {OrderId}, CustomerId: {CustomerId}, Amount: {Amount}", orderId, customerId, amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while notifying admin about new order. OrderId: {OrderId}, CustomerId: {CustomerId}, Amount: {Amount}", createdOrder.OrderId, createdOrder.UserId, createdOrder.TotalPrice);
                // Log error nhưng không throw để không ảnh hưởng đến flow chính
            }

            // Gửi email hướng dẫn thanh toán
            try
            {
                var user = await _userRepo.GetUserByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendPaymentCreatedEmailAsync(
                        user.Email,
                        user.FullName ?? user.Username,
                        createdOrder.OrderId,
                        createdOrder.TotalPrice,
                        paymentResponse.QRCodeUrl,
                        paymentResponse.PaymentContent
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending payment created email.");
                // Log error nhưng không throw để không ảnh hưởng đến flow chính
            }

            return paymentResponse;
        }

        public async Task<bool> ConfirmPaymentAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Đơn hàng không tồn tại.");
            }

            if (order.Status == OrderStatus.Payed.ToString())
            {
                throw new InvalidOperationException("Đơn hàng đã được thanh toán.");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = OrderStatus.Payed.ToString();
            await _orderRepo.UpdateAsync(order);
            // Thông báo đến client về thanh toán thành công
            try
            {
                var user = await _userRepo.GetUserByIdAsync(order.UserId);
                if (user != null)
                {
                    await _notificationService.NotifyUserPaymentSuccessAsync(user.UserId, order.OrderId, order.TotalPrice);
                    _logger.LogInformation("User notified about payment success. UserId: {UserId}, OrderId: {OrderId}, Amount: {Amount}", user.UserId, order.OrderId, order.TotalPrice);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while notifying user about payment success. UserId: {UserId}, OrderId: {OrderId}, Amount: {Amount}", order.UserId, order.OrderId, order.TotalPrice);
                // Log error nhưng không throw để không ảnh hưởng đến flow chính
            }
            // Cập nhật AISuggestionRequest
            var aiRequest = await _aiSuggestionRepo.GetByUserIdAsync(order.UserId);
            if (aiRequest != null)
            {
                aiRequest.OrderId = orderId;
                aiRequest.Price = order.TotalPrice;
                await _aiSuggestionRepo.UpdateAsync(aiRequest);
            }
            else
            {
                // Tạo mới nếu chưa có
                aiRequest = new AISuggestionRequest
                {
                    UserId = order.UserId,
                    Count = 0,
                    Price = order.TotalPrice,
                    OrderId = orderId
                };
                await _aiSuggestionRepo.CreateAsync(aiRequest);
            }

            // Gửi email xác nhận thanh toán
            try
            {
                var user = await _userRepo.GetUserByIdAsync(order.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendPaymentConfirmationEmailAsync(
                        user.Email,
                        user.FullName ?? user.Username,
                        orderId,
                        order.TotalPrice
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending payment confirmation email.");
                // Log error nhưng không throw để không ảnh hưởng đến flow chính
            }

            return true;
        }

        public async Task<List<PaymentResponseDto>> GetUserPaymentHistoryAsync(int userId)
        {
            var orders = await _orderRepo.GetByUserIdAsync(userId);
            var paymentHistory = new List<PaymentResponseDto>();

            foreach (var order in orders)
            {
                paymentHistory.Add(new PaymentResponseDto
                {
                    OrderId = order.OrderId,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    QRCodeUrl = string.Empty,
                    PaymentContent = $"Thanh toan AI Chat - Order #{order.OrderId}",
                    BankAccount = string.Empty,
                    BankName = string.Empty
                });
            }

            return paymentHistory;
        }

        private async Task<PaymentResponseDto> GenerateVietQRAsync(Order order)
        {
            var bankBin = _configuration["VietQR:BankBin"] ?? "970422";
            var accountNo = _configuration["VietQR:AccountNo"] ?? "";
            var accountName = _configuration["VietQR:AccountName"] ?? "";
            var template = _configuration["VietQR:Template"] ?? "compact2";

            if (string.IsNullOrEmpty(accountNo))
            {
                throw new InvalidOperationException("VietQR configuration is missing. Please configure VietQR:AccountNo in appsettings.json");
            }

            var amount = (int)order.TotalPrice;
            var description = $"AICHAT{order.OrderId}";

            // VietQR API URL
            var apiUrl = $"https://img.vietqr.io/image/{bankBin}-{accountNo}-{template}.png?amount={amount}&addInfo={description}&accountName={accountName}";

            return new PaymentResponseDto
            {
                OrderId = order.OrderId,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderDate = order.OrderDate,
                QRCodeUrl = apiUrl,
                PaymentContent = description,
                BankAccount = accountNo,
                BankName = GetBankName(bankBin)
            };
        }

        private string GetBankName(string bankBin)
        {
            return bankBin switch
            {
                "970422" => "MB Bank (Ngân hàng TMCP Quân đội)",
                "970415" => "Vietinbank",
                "970436" => "Vietcombank",
                "970418" => "BIDV",
                "970405" => "Agribank",
                "970432" => "VPBank",
                "970407" => "Techcombank",
                "970416" => "ACB",
                "970423" => "TPBank",
                "970403" => "Sacombank",
                _ => "Ngân hàng"
            };
        }
    }
}

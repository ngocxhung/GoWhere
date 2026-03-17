using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace GoWhere_Api.Service.Service
{
    public class AIChatService : IService.IAIChatService
    {
        private readonly IAISuggestionRequestRepo _aiSuggestionRepo;
        private readonly IScenicSpotRepo _scenicSpotRepo;
        private readonly IProvinceRepo _provinceRepo;
        private readonly IAIScheduleHistoryRepo _scheduleHistoryRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIChatService> _logger;
        private readonly HttpClient _httpClient;
        private const int FREE_REQUEST_LIMIT = 2;
        private const decimal PAYMENT_PRICE = 50000; // 50,000 VND
        private const int PAID_TURNS_PER_PACKAGE = 25;

        public AIChatService(
            IAISuggestionRequestRepo aiSuggestionRepo,
            IScenicSpotRepo scenicSpotRepo,
            IProvinceRepo provinceRepo,
            IAIScheduleHistoryRepo scheduleHistoryRepo,
            ILogger<AIChatService> logger,
            IOrderRepo orderRepo,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _aiSuggestionRepo = aiSuggestionRepo;
            _scenicSpotRepo = scenicSpotRepo;
            _provinceRepo = provinceRepo;
            _scheduleHistoryRepo = scheduleHistoryRepo;
            _orderRepo = orderRepo;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<AIUsageLimitResponseDto> CheckUsageLimitAsync(int userId)
        {
            var aiRequest = await _aiSuggestionRepo.GetByUserIdAsync(userId);
            var currentUsed = aiRequest?.Count ?? 0;
            // Ensure allowedTotal is at least FREE_REQUEST_LIMIT even if record exists with 0
            var allowedTotal = Math.Max(aiRequest?.AllowedCount ?? 0, FREE_REQUEST_LIMIT);
            
            var remainingRequests = Math.Max(0, allowedTotal - currentUsed);
            var requiresPayment = currentUsed >= allowedTotal;

            return new AIUsageLimitResponseDto
            {
                TotalRequests = currentUsed,
                FreeLimit = allowedTotal, // Now dynamic
                RemainingFreeRequests = remainingRequests,
                RequiresPayment = requiresPayment,
                PaymentAmount = requiresPayment ? PAYMENT_PRICE : 0,
                Message = requiresPayment
                    ? $"Bạn đã hết lượt tạo lịch trình. Vui lòng thanh toán {PAYMENT_PRICE:N0} VND để có thêm {PAID_TURNS_PER_PACKAGE} lượt. (Đã dùng: {currentUsed}/{allowedTotal})"
                    : $"Bạn còn {remainingRequests} lượt tạo lịch trình. (Hạn mức: {allowedTotal}, Đã dùng: {currentUsed})"
            };
        }

        public async Task<AIScheduleResponseDto> GenerateScheduleAsync(int userId, AIScheduleRequestDto request)
        {
            // 1. Check usage limit
            var usageLimit = await CheckUsageLimitAsync(userId);
            if (usageLimit.RequiresPayment)
            {
                throw new InvalidOperationException($"Bạn đã hết lượt miễn phí. Vui lòng thanh toán {PAYMENT_PRICE:N0} VND để tiếp tục.");
            }

            // 2. Get province - either from ID or search by name
            Province? province = null;
            if (request.ProvinceId > 0)
            {
                province = await _provinceRepo.GetProvinceByIdAsync(request.ProvinceId);
            }
            else if (!string.IsNullOrEmpty(request.Destination))
            {
                // Try to find province by destination name (lowercase comparison)
                var allProvinces = await _provinceRepo.GetAllProvincesAsync();
                var destinationLower = request.Destination.ToLower().Trim();
                
                // First try exact match
                province = allProvinces.FirstOrDefault(p => 
                    p.ProvinceName.ToLower().Trim() == destinationLower
                );
                
                // If no exact match, try contains
                if (province == null)
                {
                    province = allProvinces.FirstOrDefault(p => 
                        destinationLower.Contains(p.ProvinceName.ToLower().Trim()) ||
                        p.ProvinceName.ToLower().Trim().Contains(destinationLower)
                    );
                }
            }

            if (province == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tỉnh/thành phố '{request.Destination}'. Vui lòng thử lại với tên khác.");
            }

            // 3. Get scenic spots data
            var scenicSpots = await _scenicSpotRepo.GetScenicSpotsByProvinceIdAsync(province.ProvinceId);

            // 4. Filter by budget
            var filteredSpots = scenicSpots
                .Where(s => s.EstimatedVisitPrice <= (double)(request.Budget / request.NumberOfDays))
                .ToList();

            // 5. Generate schedule using Gemini AI
            var aiResponse = await CallGeminiAPIAsync(request, filteredSpots, province);

            // 6. Only increment request count after successful AI response
            await IncrementRequestCountAsync(userId);

            // 7. Parse and return response, then enrich with real images
            var scheduleResponse = await ParseAndEnrichResponseAsync(aiResponse, filteredSpots, request);
            scheduleResponse.RemainingFreeRequests = usageLimit.RemainingFreeRequests;
            scheduleResponse.RequiresPayment = usageLimit.RequiresPayment;
            scheduleResponse.ProvinceId = province.ProvinceId;

            // 8. Save to history
            await SaveScheduleHistoryAsync(userId, request, province, scheduleResponse);

            return scheduleResponse;
        }

        private async Task SaveScheduleHistoryAsync(int userId, AIScheduleRequestDto request, Province province, AIScheduleResponseDto response)
        {
            try
            {
                var history = new AIScheduleHistory
                {
                    UserId = userId,
                    Destination = request.Destination,
                    ProvinceId = province.ProvinceId,
                    Budget = request.Budget,
                    NumberOfDays = request.NumberOfDays,
                    TravelStyle = request.TravelStyle,
                    CompanionType = request.CompanionType,
                    Pax = request.Pax,
                    ScheduleJson = JsonSerializer.Serialize(response, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    }),
                    Summary = response.Summary,
                    EstimatedTotalCost = response.EstimatedTotalCost,
                    CreatedAt = DateTime.UtcNow
                };

                await _scheduleHistoryRepo.CreateAsync(history);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                Console.WriteLine($"Failed to save schedule history: {ex.Message}");
            }
        }

        private async Task IncrementRequestCountAsync(int userId)
        {
            var aiRequest = await _aiSuggestionRepo.GetByUserIdAsync(userId);
            if (aiRequest == null)
            {
                var newReq = new AISuggestionRequest
                {
                    UserId = userId,
                    Count = 1,
                    AllowedCount = 2,
                    Price = 0
                };
                await _aiSuggestionRepo.CreateAsync(newReq);
            }
            else
            {
                // Double-check hạn mức tại thời điểm increment để tránh tăng Count khi đã hết lượt
                var allowedTotal = Math.Max(aiRequest.AllowedCount, FREE_REQUEST_LIMIT);
                if (aiRequest.Count >= allowedTotal)
                {
                    // Đã hết lượt, không tăng thêm Count
                    return;
                }

                await _aiSuggestionRepo.IncrementCountAsync(userId);
            }
        }

        private async Task<string> CallGeminiAPIAsync(AIScheduleRequestDto request, List<ScenicSpot> scenicSpots, Province province)
        {
            var apiKey = _configuration["GeminiAPI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key not configured. Add 'GeminiAPI:ApiKey' to appsettings.json");
            }

            // Build prompt with scenic spots data
            var prompt = BuildPrompt(request, scenicSpots, province);

            var modelName = "gemini-flash-latest";

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Gemini API error: {response.StatusCode} - {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            return geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? "Không thể tạo lịch trình. Vui lòng thử lại.";
        }

        private string BuildPrompt(AIScheduleRequestDto request, List<ScenicSpot> scenicSpots, Province province)
        {
            var spotsInfo = new StringBuilder();
            foreach (var spot in scenicSpots.Take(10))  // Limit to 10 spots to avoid token limit
            {
                spotsInfo.AppendLine($"- {spot.ScenicSpotName}: {spot.Description}, Giá: {spot.EstimatedVisitPrice:N0} VND");
            }

            var locationInfo = !string.IsNullOrEmpty(request.UserLocation) && request.UserLocation != "Hà Nội, Việt Nam"
                ? $"- Vị trí xuất phát: {request.UserLocation} (tọa độ GPS)"
                : "- Vị trí xuất phát: Chưa xác định (khách hàng chưa cung cấp hoặc từ chối quyền truy cập)";

            var prompt = $$"""
Bạn là chuyên gia tư vấn du lịch Việt Nam. Hãy tạo một lịch trình du lịch chi tiết cho khách hàng với thông tin sau:

**Thông tin chuyến đi:**
{{locationInfo}}
- Điểm đến: {{province.ProvinceName}}
- Ngân sách: {{request.Budget:N0}} VND
- Số ngày: {{request.NumberOfDays}} ngày
- Phong cách: {{request.TravelStyle}}
- Đi cùng: {{request.CompanionType}}
{{(string.IsNullOrEmpty(request.AdditionalRequirements) ? "" : $"- Yêu cầu thêm: {request.AdditionalRequirements}")}}

**Các địa điểm du lịch có sẵn trong hệ thống:**
{{spotsInfo}}

**YÊU CẦU:**
1. Tạo lịch trình {{request.NumberOfDays}} ngày, mỗi ngày có 3-5 hoạt động
2. Ưu tiên sử dụng các địa điểm trong danh sách trên
3. Tổng chi phí phải trong ngân sách {{request.Budget:N0}} VND
4. Phù hợp với phong cách {{request.TravelStyle}} và nhóm {{request.CompanionType}}
5. Với mỗi địa điểm (scenicSpot), BẮT BUỘC cung cấp đầy đủ:
   a) openingHours / closingHours: giờ mở/đóng cửa định dạng "HH:mm"
   b) highlights: mảng 2-3 điểm nổi bật ngắn gọn
   c) googleMapsLink: URL Google Maps tìm kiếm tên địa điểm, ví dụ: "https://www.google.com/maps/search/Hồ+Hoàn+Kiếm+Hà+Nội"
   d) cuisine: mảng 1-3 món ăn/nhà hàng đặc trưng gần địa điểm theo cấu trúc:
      - googleMapsUrl: URL Google Maps tìm kiếm tên quán, ví dụ: "https://www.google.com/maps/search/Phở+Thìn+Hà+Nội"
   KHÔNG CẦN cung cấp suggestedImageUrl hay imageUrl cho ảnh - hệ thống sẽ tự tìm ảnh.
6. Trả về JSON format như sau:

```json
{
  "summary": "Tóm tắt lịch trình",
  "estimatedTotalCost": 0,
  "dailyItinerary": [
    {
      "day": 1,
      "date": "Ngày 1",
      "dayBudget": 0,
      "activities": [
        {
          "time": "08:00",
          "scenicSpot": {
            "name": "Tên địa điểm",
            "description": "Mô tả chi tiết về địa điểm, lịch sử, đặc điểm nổi bật",
            "estimatedVisitPrice": 0,
            "tips": "Lưu ý thực tế hữu ích khi tham quan",
            "openingHours": "07:00",
            "closingHours": "21:00",
            "highlights": ["Điểm nổi bật 1", "Điểm nổi bật 2", "Điểm nổi bật 3"],
            "googleMapsLink": "https://www.google.com/maps/search/Tên+địa+điểm+Tỉnh",
            "cuisine": [
              {
                "name": "Tên món ăn đặc trưng",
                "restaurant": "Tên quán nổi tiếng",
                "priceRange": "30.000 - 60.000 VND",
                "mealType": "LUNCH",
                "googleMapsUrl": "https://www.google.com/maps/search/Tên+quán+Tỉnh"
              }
            ]
          },
          "activity": "Tên hoạt động",
          "description": "Mô tả chi tiết hoạt động",
          "estimatedCost": 0
        }
      ]
    }
  ]
}
```

CHỈ TRẢ VỀ JSON, KHÔNG THÊM TEXT NÀO KHÁC.
""";
            return prompt;
        }

        private async Task<AIScheduleResponseDto> ParseAndEnrichResponseAsync(string aiResponse, List<ScenicSpot> scenicSpots, AIScheduleRequestDto request)
        {
            try
            {
                aiResponse = aiResponse.Trim();
                if (aiResponse.StartsWith("```json"))
                    aiResponse = aiResponse.Substring(7);
                if (aiResponse.StartsWith("```"))
                    aiResponse = aiResponse.Substring(3);
                if (aiResponse.EndsWith("```"))
                    aiResponse = aiResponse.Substring(0, aiResponse.Length - 3);
                aiResponse = aiResponse.Trim();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var parsedResponse = JsonSerializer.Deserialize<AIScheduleResponseDto>(aiResponse, options);

                if (parsedResponse == null)
                    throw new Exception("Failed to parse AI response");

                var enrichmentTasks = new List<Task>();

                foreach (var day in parsedResponse.DailyItinerary ?? new List<DayItineraryDto>())
                {
                    foreach (var activity in day.Activities ?? new List<ScheduleItemDto>())
                    {
                        if (activity.ScenicSpot == null) continue;

                        var matchedSpot = scenicSpots.FirstOrDefault(s =>
                            s.ScenicSpotName.Contains(activity.ScenicSpot.Name, StringComparison.OrdinalIgnoreCase) ||
                            activity.ScenicSpot.Name.Contains(s.ScenicSpotName, StringComparison.OrdinalIgnoreCase)
                        );

                        if (matchedSpot != null)
                        {
                            activity.ScenicSpot.ScenicSpotId = matchedSpot.ScenicSpotId;
                            activity.ScenicSpot.SuggestedImageUrl = matchedSpot.ImageUrl;
                            activity.ScenicSpot.MediaUrls = matchedSpot.Medias?.Select(m => m.MediaUrl).ToList() ?? new List<string>();
                        }
                        else if (string.IsNullOrEmpty(activity.ScenicSpot.SuggestedImageUrl))
                        {
                            var spot = activity.ScenicSpot;
                            enrichmentTasks.Add(Task.Run(async () =>
                            {
                                var url = await SearchImageAsync(spot.Name);
                                if (!string.IsNullOrEmpty(url))
                                    spot.SuggestedImageUrl = url;
                            }));
                        }

                        if (activity.ScenicSpot.Cuisine != null)
                        {
                            foreach (var c in activity.ScenicSpot.Cuisine)
                            {
                                if (string.IsNullOrEmpty(c.ImageUrl))
                                {
                                    var cuisine = c;
                                    enrichmentTasks.Add(Task.Run(async () =>
                                    {
                                        var url = await SearchImageAsync($"{cuisine.Name} {cuisine.Restaurant} món ăn Việt Nam");
                                        if (!string.IsNullOrEmpty(url))
                                            cuisine.ImageUrl = url;
                                    }));
                                }
                            }
                        }
                    }
                }

                if (enrichmentTasks.Count > 0)
                {
                    try
                    {
                        await Task.WhenAll(enrichmentTasks);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Some image searches failed: {Message}", ex.Message);
                    }
                }

                return parsedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse AI response");
                return new AIScheduleResponseDto
                {
                    Summary = $"Lịch trình {request.NumberOfDays} ngày tại tỉnh {request.ProvinceId}",
                    EstimatedTotalCost = request.Budget,
                    DailyItinerary = new List<DayItineraryDto>(),
                    RemainingFreeRequests = 0,
                    RequiresPayment = false
                };
            }
        }

        private async Task<string?> SearchImageAsync(string query)
        {
            try
            {
                // Dùng SerpAPI cho Google Images search
                var apiKey = _configuration["GoogleSearch:ApiKey"];

                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_SERPAPI_KEY")
                    return null;

                var encodedQuery = Uri.EscapeDataString(query);
                var url = $"https://serpapi.com/search.json?engine=google_images&q={encodedQuery}&api_key={apiKey}&num=1";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("images_results", out var images) && images.GetArrayLength() > 0)
                {
                    var first = images[0];
                    if (first.TryGetProperty("original", out var originalProp))
                    {
                        return originalProp.GetString();
                    }
                    if (first.TryGetProperty("thumbnail", out var thumbProp))
                    {
                        return thumbProp.GetString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Image search failed for '{Query}': {Message}", query, ex.Message);
                return null;
            }
        }

        public async Task<AIChatResponseDto> ChatWithAIAsync(AIChatRequestDto request)
        {
            var apiKey = _configuration["GeminiAPI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key not configured. Add 'GeminiAPI:ApiKey' to appsettings.json");
            }

            var modelName = "gemini-flash-latest";
            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";

            // Build chat prompt with context about GoWhere
            var systemContext = @"Bạn là trợ lý du lịch thông minh của GoWhere - nền tảng du lịch Việt Nam. 
Nhiệm vụ của bạn là:
- Tư vấn về các địa điểm du lịch tại Việt Nam
- Giúp khách hàng lên kế hoạch chuyến đi
- Gợi ý các hoạt động, ẩm thực, văn hóa địa phương
- Trả lời các câu hỏi về du lịch một cách thân thiện và chuyên nghiệp

Hãy trả lời ngắn gọn, súc tích và hữu ích.";

            var fullPrompt = $"{systemContext}\n\nKhách hàng hỏi: {request.Message}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = fullPrompt }
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini API error: {response.StatusCode} - {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            var aiResponse = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? "Xin lỗi, tôi không thể trả lời câu hỏi này. Vui lòng thử lại.";

            return new AIChatResponseDto
            {
                Response = aiResponse,
                ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
        }

        // Gemini API Response Model
        private class GeminiResponse
        {
            public Candidate[]? candidates { get; set; }
        }

        private class Candidate
        {
            public Content? content { get; set; }
        }

        private class Content
        {
            public Part[]? parts { get; set; }
        }

        private class Part
        {
            public string? text { get; set; }
        }

        // ============ HISTORY MANAGEMENT ============
        public async Task<List<AIScheduleHistory>> GetHistoryByUserIdAsync(int userId)
        {
            return await _scheduleHistoryRepo.GetByUserIdAsync(userId);
        }

        public async Task<AIScheduleHistory?> GetHistoryByIdAsync(int userId, int historyId)
        {
            var history = await _scheduleHistoryRepo.GetByIdAsync(historyId);
            
            if (history == null || history.UserId != userId)
            {
                return null;
            }

            return history;
        }

        public async Task<bool> DeleteHistoryAsync(int userId, int historyId)
        {
            var history = await _scheduleHistoryRepo.GetByIdAsync(historyId);
            
            if (history == null || history.UserId != userId)
            {
                return false;
            }

            return await _scheduleHistoryRepo.DeleteAsync(historyId);
        }

        // ============ PAYMENT ORDER MANAGEMENT ============
        public async Task<AIPaymentOrderResponseDto> CreateAIPaymentOrderAsync(int userId)
        {
            var aiRequest = await _aiSuggestionRepo.GetByUserIdAsync(userId);

            // Allow repayment if turns are exhausted
            //Temporarily remove this check to allow users to create payment orders even if they still have free turns, as they might want to top up in advance or for future use.
            /* if (aiRequest != null && aiRequest.Count < aiRequest.AllowedCount)
             {
                 throw new InvalidOperationException($"Bạn vẫn còn {aiRequest.AllowedCount - aiRequest.Count} lượt. Hãy sử dụng hết trước khi nạp thêm!");
             }*/

            // Check if already has pending payment order
            if (aiRequest?.Order != null && aiRequest.Order.Status == "WaitPayment")
            {
                return new AIPaymentOrderResponseDto
                {
                    Message = "Đơn hàng đã tồn tại",
                    OrderId = aiRequest.Order.OrderId,
                    Amount = 50000
                };
            }

            // Create new order for AI payment
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "WaitPayment",
                TotalPrice = 50000,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ItemName = "AI Package - 5 lượt tạo lịch trình",
                        Price = 50000,
                        Quantity = 1,
                        ServiceType = "AI"
                    }
                }
            };

            await _orderRepo.CreateAsync(order);

            // Link order to AI request
            if (aiRequest == null)
            {
                aiRequest = new AISuggestionRequest
                {
                    UserId = userId,
                    Count = 0,
                    Price = 50000,
                    OrderId = order.OrderId
                };
                await _aiSuggestionRepo.CreateAsync(aiRequest);
            }
            else
            {
                aiRequest.OrderId = order.OrderId;
                aiRequest.Price = 50000;
                await _aiSuggestionRepo.UpdateAsync(aiRequest);
            }

            return new AIPaymentOrderResponseDto
            {
                Message = "Tạo đơn hàng thành công",
                OrderId = order.OrderId,
                Amount = 50000
            };
        }

    }
}

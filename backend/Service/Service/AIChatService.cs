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
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private const int FREE_REQUEST_LIMIT = 2;
        private const decimal PAYMENT_PRICE = 50000; // 50,000 VND

        public AIChatService(
            IAISuggestionRequestRepo aiSuggestionRepo,
            IScenicSpotRepo scenicSpotRepo,
            IProvinceRepo provinceRepo,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _aiSuggestionRepo = aiSuggestionRepo;
            _scenicSpotRepo = scenicSpotRepo;
            _provinceRepo = provinceRepo;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<AIUsageLimitResponseDto> CheckUsageLimitAsync(int userId)
        {
            var aiRequest = await _aiSuggestionRepo.GetByUserIdAsync(userId);
            var currentCount = aiRequest?.Count ?? 0;
            var remainingRequests = Math.Max(0, FREE_REQUEST_LIMIT - currentCount);
            var requiresPayment = currentCount >= FREE_REQUEST_LIMIT && (aiRequest?.Order == null || aiRequest.Order.Status != "Payed");

            return new AIUsageLimitResponseDto
            {
                TotalRequests = currentCount,
                FreeLimit = FREE_REQUEST_LIMIT,
                RemainingFreeRequests = remainingRequests,
                RequiresPayment = requiresPayment,
                PaymentAmount = requiresPayment ? PAYMENT_PRICE : 0,
                Message = requiresPayment
                    ? $"Bạn đã hết lượt tạo lịch trình miễn phí. Vui lòng thanh toán {PAYMENT_PRICE:N0} VND để tiếp tục."
                    : $"Bạn còn {remainingRequests} lượt miễn phí."
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

            // 2. Get scenic spots data
            var scenicSpots = await _scenicSpotRepo.GetScenicSpotsByProvinceIdAsync(request.ProvinceId);
            var province = await _provinceRepo.GetProvinceByIdAsync(request.ProvinceId);

            if (province == null)
            {
                throw new KeyNotFoundException($"Tỉnh với ID {request.ProvinceId} không tồn tại.");
            }

            // 3. Filter by budget
            var filteredSpots = scenicSpots
                .Where(s => s.EstimatedVisitPrice <= (double)(request.Budget / request.NumberOfDays))
                .ToList();

            // 4. Generate schedule using Gemini AI
            var aiResponse = await CallGeminiAPIAsync(request, filteredSpots, province);

            // 5. Only increment request count after successful AI response
            await IncrementRequestCountAsync(userId);

            // 6. Parse and return response
            var scheduleResponse = ParseAIResponse(aiResponse, filteredSpots, request);
            scheduleResponse.RemainingFreeRequests = FREE_REQUEST_LIMIT - (await _aiSuggestionRepo.GetRequestCountAsync(userId));
            scheduleResponse.RequiresPayment = scheduleResponse.RemainingFreeRequests <= 0;

            return scheduleResponse;
        }

        private async Task IncrementRequestCountAsync(int userId)
        {
            var aiRequest = await _aiSuggestionRepo.GetByUserIdAsync(userId);

            if (aiRequest == null)
            {
                // Create new record
                aiRequest = new AISuggestionRequest
                {
                    UserId = userId,
                    Count = 1,
                    Price = 0,
                    OrderId = null
                };
                await _aiSuggestionRepo.CreateAsync(aiRequest);
            }
            else
            {
                // Increment count
                aiRequest.Count++;
                await _aiSuggestionRepo.UpdateAsync(aiRequest);
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
                throw new HttpRequestException($"Gemini API error: {response.StatusCode} - {error}");
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

            var prompt = $@"
Bạn là chuyên gia tư vấn du lịch Việt Nam. Hãy tạo một lịch trình du lịch chi tiết cho khách hàng với thông tin sau:

**Thông tin chuyến đi:**
- Điểm đến: {province.ProvinceName}
- Ngân sách: {request.Budget:N0} VND
- Số ngày: {request.NumberOfDays} ngày
- Phong cách: {request.TravelStyle}
- Đi cùng: {request.CompanionType}
{(string.IsNullOrEmpty(request.AdditionalRequirements) ? "" : $"- Yêu cầu thêm: {request.AdditionalRequirements}")}

**Các địa điểm du lịch có sẵn trong hệ thống:**
{spotsInfo}

**YÊU CẦU:**
1. Tạo lịch trình {request.NumberOfDays} ngày, mỗi ngày có 3-5 hoạt động
2. Ưu tiên sử dụng các địa điểm trong danh sách trên
3. Thêm thông tin về ẩm thực, di chuyển, nghỉ ngơi
4. Tổng chi phí phải trong ngân sách {request.Budget:N0} VND
5. Phù hợp với phong cách {request.TravelStyle} và nhóm {request.CompanionType}
6. Trả về JSON format như sau:

{{
  ""summary"": ""Tóm tắt lịch trình"",
  ""estimatedTotalCost"": 0,
  ""dailyItinerary"": [
    {{
      ""day"": 1,
      ""date"": ""Ngày 1"",
      ""dayBudget"": 0,
      ""activities"": [
        {{
          ""time"": ""08:00"",
          ""scenicSpot"": {{
            ""name"": ""Tên địa điểm"",
            ""description"": ""Mô tả"",
            ""estimatedVisitPrice"": 0,
            ""tips"": ""Lưu ý khi tham quan""
          }},
          ""activity"": ""Hoạt động"",
          ""description"": ""Mô tả chi tiết"",
          ""estimatedCost"": 0
        }}
      ]
    }}
  ]
}}

CHỈ TRẢ VỀ JSON, KHÔNG THÊM TEXT NÀO KHÁC.
";
            return prompt;
        }

        private AIScheduleResponseDto ParseAIResponse(string aiResponse, List<ScenicSpot> scenicSpots, AIScheduleRequestDto request)
        {
            try
            {
                // Remove markdown code blocks if present
                aiResponse = aiResponse.Trim();
                if (aiResponse.StartsWith("```json"))
                {
                    aiResponse = aiResponse.Substring(7);
                }
                if (aiResponse.StartsWith("```"))
                {
                    aiResponse = aiResponse.Substring(3);
                }
                if (aiResponse.EndsWith("```"))
                {
                    aiResponse = aiResponse.Substring(0, aiResponse.Length - 3);
                }
                aiResponse = aiResponse.Trim();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var parsedResponse = JsonSerializer.Deserialize<AIScheduleResponseDto>(aiResponse, options);

                if (parsedResponse == null)
                {
                    throw new Exception("Failed to parse AI response");
                }

                // Match scenic spots with database
                foreach (var day in parsedResponse.DailyItinerary ?? new List<DayItineraryDto>())
                {
                    foreach (var activity in day.Activities ?? new List<ScheduleItemDto>())
                    {
                        if (activity.ScenicSpot != null)
                        {
                            var matchedSpot = scenicSpots.FirstOrDefault(s =>
                                s.ScenicSpotName.Contains(activity.ScenicSpot.Name, StringComparison.OrdinalIgnoreCase) ||
                                activity.ScenicSpot.Name.Contains(s.ScenicSpotName, StringComparison.OrdinalIgnoreCase)
                            );

                            if (matchedSpot != null)
                            {
                                activity.ScenicSpot.ScenicSpotId = matchedSpot.ScenicSpotId;
                                activity.ScenicSpot.MediaUrls = matchedSpot.Medias?.Select(m => m.MediaUrl).ToList() ?? new List<string>();
                            }
                        }
                    }
                }

                return parsedResponse;
            }
            catch (Exception ex)
            {
                // Fallback: Return a simple response
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
    }
}

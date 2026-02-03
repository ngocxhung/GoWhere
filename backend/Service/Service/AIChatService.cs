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
        private readonly ApplicationDbContext _context;
        private const int FREE_REQUEST_LIMIT = 5; // Increased for better testing
        private const decimal PAYMENT_PRICE = 50000; // 50,000 VND

        public AIChatService(
            IAISuggestionRequestRepo aiSuggestionRepo,
            IScenicSpotRepo scenicSpotRepo,
            IProvinceRepo provinceRepo,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context)
        {
            _aiSuggestionRepo = aiSuggestionRepo;
            _scenicSpotRepo = scenicSpotRepo;
            _provinceRepo = provinceRepo;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
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
            try {
                // 1. Check usage limit
                var usageLimit = await CheckUsageLimitAsync(userId);
                if (usageLimit.RequiresPayment)
                {
                    throw new InvalidOperationException($"Bạn đã hết lượt miễn phí. Vui lòng thanh toán {PAYMENT_PRICE:N0} VND để tiếp tục.");
                }

                // 2. Try to find the province based on destination
                var provinces = await _context.Provinces.ToListAsync();
                var targetProvince = provinces.FirstOrDefault(p => !string.IsNullOrEmpty(p.ProvinceName) && !string.IsNullOrEmpty(request.Destination) && 
                    (request.Destination.Contains(p.ProvinceName, StringComparison.OrdinalIgnoreCase) || p.ProvinceName.Contains(request.Destination, StringComparison.OrdinalIgnoreCase)));

                if (targetProvince == null)
                {
                    // Try finding via ScenicSpot name
                    var spotFound = await _context.ScenicSpots.Include(s => s.Province)
                        .FirstOrDefaultAsync(s => s.ScenicSpotName.Contains(request.Destination));
                    targetProvince = spotFound?.Province;
                }

                // Fallback to first province if nothing found
                targetProvince ??= provinces.FirstOrDefault() ?? new Province { ProvinceId = 1, ProvinceName = "Việt Nam" };

                // 3. Get database data
                var scenicSpots = await _context.ScenicSpots
                    .Where(s => s.ProvinceId == targetProvince.ProvinceId)
                    .Include(s => s.Medias)
                    .ToListAsync();

                var suitableCar = await _context.CarRentals
                    .Where(c => c.Seats >= request.Pax)
                    .OrderBy(c => c.PricePerDay)
                    .FirstOrDefaultAsync();

                var suitableHotel = await _context.Hotels
                    .Where(h => h.ProvinceId == targetProvince.ProvinceId)
                    .OrderByDescending(h => h.Rating)
                    .FirstOrDefaultAsync();

                // 4. Manual Logic to Generate Schedule (Replacing Gemini call for "trial")
                var scheduleResponse = GenerateManualSchedule(request, targetProvince, scenicSpots, suitableCar, suitableHotel);

                // 5. Increment request count
                await IncrementRequestCountAsync(userId);

                // 6. Finalize response
                scheduleResponse.RemainingFreeRequests = FREE_REQUEST_LIMIT - (await _aiSuggestionRepo.GetRequestCountAsync(userId));
                scheduleResponse.RequiresPayment = scheduleResponse.RemainingFreeRequests <= 0;

                return scheduleResponse;
            } catch (Exception ex) {
                Console.WriteLine("ERROR IN GenerateScheduleAsync: " + ex.ToString());
                throw;
            }
        }

        private AIScheduleResponseDto GenerateManualSchedule(AIScheduleRequestDto request, Province province, List<ScenicSpot> spots, CarRental? car, Hotel? hotel)
        {
            var result = new AIScheduleResponseDto
            {
                Summary = $"Dựa trên yêu cầu của bạn, GoWhere đã tối ưu lịch trình {request.NumberOfDays} ngày tại {province.ProvinceName}. " +
                          (car != null ? $"Hệ thống đã chọn xe {car.Model} phù hợp cho {request.Pax} người. " : "Không tìm thấy xe phù hợp trong DB. ") +
                          (hotel != null ? $"Khách sạn được chọn: {hotel.Name}." : "Vui lòng tự chọn khách sạn."),
                EstimatedTotalCost = 0,
                DailyItinerary = new List<DayItineraryDto>()
            };

            decimal totalCost = 0;
            // Add Car cost
            if (car != null) totalCost += (decimal)(car.PricePerDay * request.NumberOfDays);
            // Add Hotel cost
            if (hotel != null) totalCost += (decimal)(hotel.PricePerNight * (request.NumberOfDays - 1));

            int spotIndex = 0;
            for (int i = 1; i <= request.NumberOfDays; i++)
            {
                var day = new DayItineraryDto
                {
                    Day = i,
                    Date = $"Ngày {i}",
                    Activities = new List<ScheduleItemDto>(),
                    DayBudget = 0
                };

                // Activity 1: Morning
                if (i == 1)
                {
                    day.Activities.Add(new ScheduleItemDto { 
                        Time = "08:00", 
                        Activity = "Di chuyển", 
                        Description = $"Bắt đầu hành trình từ {request.UserLocation} tới {province.ProvinceName} bằng {car?.Model ?? "phương tiện tự chọn"}.", 
                        EstimatedCost = 0 
                    });
                }
                else
                {
                    day.Activities.Add(new ScheduleItemDto { 
                        Time = "08:00", 
                        Activity = "Ăn sáng", 
                        Description = $"Dùng bữa sáng tại {hotel?.Name ?? "khách sạn"}.", 
                        EstimatedCost = 50000 
                    });
                    day.DayBudget += 50000;
                }

                // Activity 2: Scenic Spot
                if (spotIndex < spots.Count)
                {
                    var spot = spots[spotIndex++];
                    day.Activities.Add(new ScheduleItemDto
                    {
                        Time = "10:00",
                        Activity = "Tham quan",
                        Description = $"Khám phá {spot.ScenicSpotName}. {spot.Description}",
                        EstimatedCost = (decimal)spot.EstimatedVisitPrice,
                        ScenicSpot = new ScenicSpotDetailDto
                        {
                            ScenicSpotId = spot.ScenicSpotId,
                            Name = spot.ScenicSpotName,
                            Description = spot.Description,
                            EstimatedVisitPrice = spot.EstimatedVisitPrice,
                            MediaUrls = spot.Medias?.Select(m => m.MediaUrl).ToList() ?? new List<string>()
                        }
                    });
                    day.DayBudget += (decimal)spot.EstimatedVisitPrice;
                }

                // Activity 3: Afternoon
                if (spotIndex < spots.Count)
                {
                    var spot = spots[spotIndex++];
                    day.Activities.Add(new ScheduleItemDto
                    {
                        Time = "14:00",
                        Activity = "Tham quan",
                        Description = $"Ghé thăm {spot.ScenicSpotName}.",
                        EstimatedCost = (decimal)spot.EstimatedVisitPrice,
                        ScenicSpot = new ScenicSpotDetailDto
                        {
                            ScenicSpotId = spot.ScenicSpotId,
                            Name = spot.ScenicSpotName,
                            Description = spot.Description,
                            EstimatedVisitPrice = spot.EstimatedVisitPrice,
                            MediaUrls = spot.Medias?.Select(m => m.MediaUrl).ToList() ?? new List<string>()
                        }
                    });
                    day.DayBudget += (decimal)spot.EstimatedVisitPrice;
                }

                totalCost += day.DayBudget;
                result.DailyItinerary.Add(day);
            }

            result.EstimatedTotalCost = totalCost;
            return result;
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

        private async Task<string> CallGeminiAPIAsync(AIScheduleRequestDto request, List<ScenicSpot> scenicSpots, List<CarRental> cars, List<Hotel> hotels)
        {
            var apiKey = _configuration["GeminiAPI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Gemini API Key not configured. Add 'GeminiAPI:ApiKey' to appsettings.json");
            }

            // Build prompt
            var prompt = BuildPrompt(request, scenicSpots, cars, hotels);

            // Call Gemini API
            var modelName = "gemini-flash-latest";

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";            // ... rest of logic remains same ...
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
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

        private string BuildPrompt(AIScheduleRequestDto request, List<ScenicSpot> scenicSpots, List<CarRental> cars, List<Hotel> hotels)
        {
            var spotsInfo = new StringBuilder();
            foreach (var spot in scenicSpots.Take(20))
            {
                spotsInfo.AppendLine($"- [ID:{spot.ScenicSpotId}] {spot.ScenicSpotName} ({spot.Province?.ProvinceName}): {spot.Description}, Giá: {spot.EstimatedVisitPrice:N0} VND");
            }

            var carsInfo = new StringBuilder();
            if (!cars.Any()) carsInfo.AppendLine("KHÔNG CÓ XE PHÙ HỢP");
            else {
                foreach (var car in cars) {
                    carsInfo.AppendLine($"- [ID:{car.CarRentalId}] {car.Model} ({car.Seats} chỗ), Giá: {car.PricePerDay:N0} VND/ngày");
                }
            }

            var hotelsInfo = new StringBuilder();
            if (!hotels.Any()) hotelsInfo.AppendLine("KHÔNG CÓ KHÁCH SẠN PHÙ HỢP");
            else {
                foreach (var hotel in hotels.Take(10)) {
                    hotelsInfo.AppendLine($"- [ID:{hotel.HotelId}] {hotel.Name} ({hotel.Location}), Giá: {hotel.PricePerNight:N0} VND/đêm");
                }
            }

            var prompt = $@"
Bạn là chuyên gia tư vấn du lịch Việt Nam. Hãy tạo một lịch trình du lịch chi tiết cho khách hàng với thông tin sau:

**Thông tin chuyến đi:**
- Xuất phát từ: {request.UserLocation}
- Điểm tới (Destination): {request.Destination} 
- Số người tham gia: {request.Pax} ({request.CompanionType})
- Ngân sách: {request.Budget:N0} VND
- Số ngày: {request.NumberOfDays} ngày
- Phong cách: {request.TravelStyle}

**Dữ liệu hệ thống hỗ trợ (Dựa vào đây để chọn):**
1. DANH SÁCH ĐỊA ĐIỂM (SCENIC SPOTS):
{spotsInfo}

2. DANH SÁCH XE (CARS):
{carsInfo}

3. DANH SÁCH KHÁCH SẠN (HOTELS):
{hotelsInfo}

**YÊU CẦU QUAN TRỌNG:**
1. Tìm trong DANH SÁCH ĐỊA ĐIỂM trên một địa điểm GẦN NHẤT với '{request.Destination}' thông qua kiến thức về bản đồ.
2. Tạo lịch trình di chuyển từ '{request.UserLocation}' tới địa điểm đó.
3. PHẢI chọn 1 xe từ danh sách 'DANH SÁCH XE' phù hợp với {request.Pax} người. Nếu danh sách báo 'KHÔNG CÓ XE PHÙ HỢP', hãy ghi chú trong phần summary.
4. PHẢI chọn 1 khách sạn từ 'DANH SÁCH KHÁCH SẠN' gần khu vực điểm đến. Nếu không có khách sạn nào phù hợp, hãy ghi chú trong summary.
5. Thiết kế lịch trình {request.NumberOfDays} ngày.
6. Trả về JSON format:
{{
  ""summary"": ""Tóm tắt lịch trình, bao gồm việc chọn xe và khách sạn nào, hoặc báo lỗi nếu không tìm thấy cái phù hợp."",
  ""estimatedTotalCost"": 0,
  ""dailyItinerary"": [
    {{
      ""day"": 1, ""date"": ""Ngày 1"", ""dayBudget"": 0,
      ""activities"": [
        {{
          ""time"": ""08:00"",
          ""scenicSpot"": {{ ""name"": ""Tên địa điểm từ danh sách"", ""description"": """", ""estimatedVisitPrice"": 0 }},
          ""activity"": ""Di chuyển/Tham quan"", ""description"": """", ""estimatedCost"": 0
        }}
      ]
    }}
  ]
}}
CHỈ TRẢ VỀ JSON.
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
                    Summary = $"Lịch trình {request.NumberOfDays} ngày đi {request.Destination}",
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

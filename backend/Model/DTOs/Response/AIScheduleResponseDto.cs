namespace GoWhere_Api.Model.DTOs.Response
{
    public class AIScheduleResponseDto
    {
        public string Summary { get; set; }  // Tóm tắt lịch trình
        public decimal EstimatedTotalCost { get; set; }
        public List<DayItineraryDto> DailyItinerary { get; set; }
        public int RemainingFreeRequests { get; set; }  // Số lần free còn lại
        public bool RequiresPayment { get; set; }  // Có cần thanh toán không
    }

    public class DayItineraryDto
    {
        public int Day { get; set; }
        public string Date { get; set; }
        public List<ScheduleItemDto> Activities { get; set; }
        public decimal DayBudget { get; set; }
    }

    public class ScheduleItemDto
    {
        public string Time { get; set; }
        public ScenicSpotDetailDto? ScenicSpot { get; set; }
        public string Activity { get; set; }
        public string Description { get; set; }
        public decimal EstimatedCost { get; set; }
    }

    public class ScenicSpotDetailDto
    {
        public int? ScenicSpotId { get; set; }  // Nullable vì có thể là hoạt động không phải scenic spot
        public string Name { get; set; }
        public string Description { get; set; }
        public double EstimatedVisitPrice { get; set; }
        public List<string> MediaUrls { get; set; }
        public string? GoogleMapsLink { get; set; }
        public string? Tips { get; set; }
    }
}

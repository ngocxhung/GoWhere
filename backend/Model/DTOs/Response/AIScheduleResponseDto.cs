namespace GoWhere_Api.Model.DTOs.Response
{
    public class AIScheduleResponseDto
    {
        public string Summary { get; set; }
        public decimal EstimatedTotalCost { get; set; }
        public List<DayItineraryDto> DailyItinerary { get; set; }
        public int RemainingFreeRequests { get; set; }
        public bool RequiresPayment { get; set; }
        public int? ProvinceId { get; set; }
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
        public int? ScenicSpotId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double EstimatedVisitPrice { get; set; }
        public List<string> MediaUrls { get; set; }
        public string? GoogleMapsLink { get; set; }
        public string? Tips { get; set; }
        public string? OpeningHours { get; set; }
        public string? ClosingHours { get; set; }
        public List<string>? Highlights { get; set; }
        public string? SuggestedImageUrl { get; set; }
        public List<CuisineItemDto>? Cuisine { get; set; }
    }

    public class CuisineItemDto
    {
        public string Name { get; set; }
        public string? Restaurant { get; set; }
        public string? PriceRange { get; set; }
        public string? MealType { get; set; }
        public string? ImageUrl { get; set; }
        public string? GoogleMapsUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class AIScheduleRequestDto
    {
        public Coordinate? Location { get; set; }
        public int ProvinceId { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string UserLocation { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Number of people is required.")]
        [Range(1, 45, ErrorMessage = "Number of people must be between 1 and 45.")]
        public int Pax { get; set; }

        [Required(ErrorMessage = "Budget is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive number.")]
        public decimal Budget { get; set; }

        [Required(ErrorMessage = "Number of days is required.")]
        [Range(1, 30, ErrorMessage = "Number of days must be between 1 and 30.")]
        public int NumberOfDays { get; set; }

        [Required(ErrorMessage = "Travel style is required.")]
        public string TravelStyle { get; set; }  // "Relax", "Adventure", "Culture", "FoodTour", "Shopping"

        [Required(ErrorMessage = "Companion type is required.")]
        public string CompanionType { get; set; }  // "Solo", "Couple", "Family", "Friends", "Group"

        public string? AdditionalRequirements { get; set; }  // Yêu cầu thêm từ user
    }
    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

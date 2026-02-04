using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class AIScheduleHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string Destination { get; set; }

        public int? ProvinceId { get; set; }

        [ForeignKey("ProvinceId")]
        public Province? Province { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Required]
        public int NumberOfDays { get; set; }

        [Required]
        public string TravelStyle { get; set; }

        [Required]
        public string CompanionType { get; set; }

        public int Pax { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ScheduleJson { get; set; }  // Store full AI response as JSON

        public string? Summary { get; set; }

        public decimal EstimatedTotalCost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

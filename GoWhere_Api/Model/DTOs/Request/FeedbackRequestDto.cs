using System.ComponentModel.DataAnnotations;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class FeedbackRequestDto
    {
        [Required(ErrorMessage = "ScheduleId is required.")]
        public int ScheduleId { get; set; }
        [Required(ErrorMessage = "Rating is required.")]    
        public int Rating { get; set; }

        public string? Comments { get; set; }

    }
}
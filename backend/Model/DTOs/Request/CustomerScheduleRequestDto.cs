using System.ComponentModel.DataAnnotations;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class CustomerScheduleRequestDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } 
        public ICollection<int> ScenicSpots { get; set; }




    }
}

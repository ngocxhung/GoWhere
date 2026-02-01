using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class CustomerSchedule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        [ForeignKey("FeedbackId")]
        public int? FeedbackId { get; set; }
        public Feedback? Feedback { get; set; }
        public User User { get; set; }
        public ICollection<ScenicSpot> ScenicSpots { get; set; }
        
    }
    public enum ScheduleStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }


}
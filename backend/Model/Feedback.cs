using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model
{
    public class Feedback
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        [StringLength(1000)]
        public string? Comments { get; set; }
        [Range(1, 5)]
        public double Rating { get; set; }
        [ForeignKey("ScheduleId")]
        public int? ScheduleId { get; set; }
        public CustomerSchedule? CustomerSchedule { get; set; }

    }
}
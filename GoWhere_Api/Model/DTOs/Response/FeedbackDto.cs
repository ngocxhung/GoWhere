namespace GoWhere_Api.Model.DTOs.Response
{
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public string Comments { get; set; }
        public double Rating { get; set; }
    }
}
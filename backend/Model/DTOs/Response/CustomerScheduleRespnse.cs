namespace GoWhere_Api.Model.DTOs.Response
{
    public class CustomerScheduleRespnse
    {
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public List<ScenicSpotDto> ScenicSpots { get; set; }
        public FeedbackDto? Feedback { get; set; }
    }
}
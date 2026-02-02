namespace GoWhere_Api.Model.DTOs.Response
{
    public class AIUsageLimitResponseDto
    {
        public int TotalRequests { get; set; }
        public int FreeLimit { get; set; }
        public int RemainingFreeRequests { get; set; }
        public bool RequiresPayment { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Message { get; set; }
    }
}

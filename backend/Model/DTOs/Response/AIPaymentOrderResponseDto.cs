namespace GoWhere_Api.Model.DTOs.Response
{
    public class AIPaymentOrderResponseDto
    {
        public string Message { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}

namespace GoWhere_Api.Model.DTOs.Response
{
    public class PaymentResponseDto
    {
        public int OrderId { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string QRCodeUrl { get; set; }
        public string PaymentContent { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
    }
}

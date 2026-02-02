namespace GoWhere_Api.Model.DTOs.Response
{
    public class VietQRResponseDto
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public VietQRData Data { get; set; }
    }

    public class VietQRData
    {
        public int AcqId { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string Amount { get; set; }
        public string Template { get; set; }
        public string QrDataURL { get; set; }
    }
}

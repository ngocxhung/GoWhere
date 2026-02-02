namespace GoWhere_Api.Service.IService
{
    public interface IEmailService
    {
        Task SendPaymentConfirmationEmailAsync(string toEmail, string userName, int orderId, double amount);
        Task SendPaymentCreatedEmailAsync(string toEmail, string userName, int orderId, double amount, string qrCodeUrl, string paymentContent);
    }
}

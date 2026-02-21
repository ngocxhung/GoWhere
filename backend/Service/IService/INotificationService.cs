namespace GoWhere_Api.Service.IService
{
    public interface INotificationService
    {
        /// <summary>
        /// Thông báo đến tất cả Admin khi có order mới
        /// </summary>
        Task NotifyAdminNewOrderAsync(int orderId, int userId, double amount, string userName);

        /// <summary>
        /// Thông báo đến client cụ thể khi payment thành công
        /// </summary>
        Task NotifyUserPaymentSuccessAsync(int userId, int orderId, double amount);

        /// <summary>
        /// Thông báo đến client cụ thể (generic)
        /// </summary>
        Task NotifyUserAsync(int userId, string message, object? data = null);

        /// <summary>
        /// Thông báo đến tất cả Admin (generic)
        /// </summary>
        Task NotifyAllAdminsAsync(string message, object? data = null);
    }
}
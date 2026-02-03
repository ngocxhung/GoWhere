using GoWhere_Api.Model;

namespace GoWhere_Api.Repo.IRepo
{
    public interface IOrderRepo
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task<Order?> GetByUserAndStatusAsync(int userId, string status);
        Task<bool> UpdateAsync(Order order);
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<List<Order>> GetAllAsync();
        Task<bool> DeleteAsync(int orderId);
    }
}

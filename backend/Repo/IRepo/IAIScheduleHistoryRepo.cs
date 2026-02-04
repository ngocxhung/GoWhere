using GoWhere_Api.Model;

namespace GoWhere_Api.Repo.IRepo
{
    public interface IAIScheduleHistoryRepo
    {
        Task<AIScheduleHistory> CreateAsync(AIScheduleHistory history);
        Task<List<AIScheduleHistory>> GetByUserIdAsync(int userId);
        Task<AIScheduleHistory?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}

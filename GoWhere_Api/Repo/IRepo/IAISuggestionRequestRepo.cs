using GoWhere_Api.Model;

namespace GoWhere_Api.Repo.IRepo
{
    public interface IAISuggestionRequestRepo
    {
        Task<AISuggestionRequest?> GetByUserIdAsync(int userId);
        Task<AISuggestionRequest> CreateAsync(AISuggestionRequest request);
        Task UpdateAsync(AISuggestionRequest request);
        Task<int> GetRequestCountAsync(int userId);
        Task<bool> CanMakeFreeRequestAsync(int userId);
    }
}

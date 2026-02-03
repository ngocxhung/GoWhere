using GoWhere_Api.Model;

namespace GoWhere_Api.Repo.IRepo
{
    public interface IUserRepo
    {
        Task<bool> IsUserNameExistAsync(string username);
        Task CreateUser(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUser(User user);
        Task<User> GetUserByUsernameAsync(string username);
    }
}

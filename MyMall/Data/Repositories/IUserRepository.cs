using MyMall.Entities;

namespace MyMall.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginAsync(string login);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}
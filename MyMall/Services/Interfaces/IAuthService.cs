using MyMall.Entities;

namespace MyMall.Services.Interfaces;

public interface IAuthService
{
    User? CurrentUser { get; }
    Task<User?> LoginAsync(string login, string password);
    void Logout();
    bool IsAdmin();
    bool IsCashier();
}
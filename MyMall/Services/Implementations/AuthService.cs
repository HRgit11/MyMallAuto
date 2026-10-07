using MyMall.Common;
using MyMall.Data.Repositories;
using MyMall.Entities;
using MyMall.Services.Interfaces;

namespace MyMall.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? CurrentUser { get; private set; }

    public async Task<User?> LoginAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userRepository.GetByLoginAsync(login);
        if (user == null) return null;

        if (!PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        CurrentUser = user;
        return user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool IsAdmin() => CurrentUser?.Role == Enums.UserRole.Admin;
    public bool IsCashier() => CurrentUser?.Role == Enums.UserRole.Cashier;
}
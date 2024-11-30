using AuthorizationService.Application.Interfaces;
using AuthorizationService.Domain.Entities;
using AuthorizationService.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorizationService.Application.Services;

public class UserManager : IUserManager
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthManager _authManager;

    public UserManager(
        IUserRepository userRepository,
        IAuthManager authManager)
    {
        _userRepository = userRepository;
        _authManager = authManager;
    }

    public async Task<List<User>> GetShortUsers()
    {
        var users = await _userRepository.GetShortUsersAsync();

        return users;
    }

    public async Task SetUserIsActive(int userId, bool isActive)
    {
        await _userRepository.SetUserIsActive(userId, isActive);

        if (!isActive)
        {
            await _authManager.RevokeTokenAsync(userId);
        }
    }
}
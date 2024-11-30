using AuthorizationService.Domain.Entities;

namespace AuthorizationService.Application.Interfaces;

public interface IAuthManager
{
    Task<string> RegisterAsync(User user);

    Task<UserToken> LoginAsync(User user);

    Task<bool> ValidateToken(int userId, string token);

    Task AddToken(UserToken userToken);

    Task RevokeTokenAsync(int userId);
}
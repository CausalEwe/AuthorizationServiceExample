using AuthorizationService.Domain.Entities;

namespace AuthorizationService.Domain.Interfaces;

public interface IAuthRepository
{
    Task RevokeTokenAsync(int userId);

    Task<string> GetTokenByUserIdAsync(int userId);

    Task AddToken(UserToken userToken);

    Task<bool> ValidateToken(int userId, string token);
}
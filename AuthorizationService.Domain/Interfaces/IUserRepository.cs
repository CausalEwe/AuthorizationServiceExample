using AuthorizationService.Domain.Entities;

namespace AuthorizationService.Domain.Interfaces;

public interface IUserRepository
{
    Task RegisterAsync(User user);

    Task<List<User>> GetShortUsersAsync();

    Task<User> GetUserByLoginAsync(string login);

    Task<List<string>> GetUserRoles(int userId);

    Task SetUserIsActive(int userId, bool isActive);
}
using AuthorizationService.Domain.Entities;

namespace AuthorizationService.Application.Interfaces;

public interface IUserManager
{
    Task<List<User>> GetShortUsers();

    Task SetUserIsActive(int userId, bool isActive);
}
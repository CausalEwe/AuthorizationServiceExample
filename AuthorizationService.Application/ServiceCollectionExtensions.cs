using AuthorizationService.Application.Interfaces;
using AuthorizationService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationService.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IUserManager, UserManager>();
    }
}
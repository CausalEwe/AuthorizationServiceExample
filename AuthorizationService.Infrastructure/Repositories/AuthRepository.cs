using AuthorizationService.Domain.Entities;
using AuthorizationService.Domain.Interfaces;
using Dapper;

namespace AuthorizationService.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IUnitOfWork _uow;

    public AuthRepository(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task RevokeTokenAsync(int userId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("userId", userId);

        _uow.Begin();

        await _uow.Connection.ExecuteAsync("RevokeToken", parameters, _uow.Transaction);
        await _uow.CompleteAsync();
    }

    public async Task<string> GetTokenByUserIdAsync(int userId)
    {
        var query = @"select token from dbo.tokens where userId = @userId and isRevoked = false";

        var parameters = new DynamicParameters();
        parameters.Add("userId", userId);

        _uow.Begin();

        var token = await _uow.Connection.QueryFirstAsync<string>(query, parameters, _uow.Transaction);
        await _uow.CompleteAsync();

        return token;
    }

    public async Task AddToken(UserToken userToken)
    {
        // в принципе наверно можно сделать чтобы филд createdAt тригером заполнялся, но я не хочу выносить какую-либо логику из приложения
        var parameters = new DynamicParameters();
        parameters.Add("userId", userToken.UserId);
        parameters.Add("token", userToken.Token);
        parameters.Add("createdAt", userToken.CreatedAt);
        parameters.Add("expiresAt", userToken.ExpiresAt);
        parameters.Add("isRevoked", userToken.IsRevoked);

        _uow.Begin();

        await _uow.Connection.ExecuteAsync("AddToken", parameters, _uow.Transaction);
        await _uow.CompleteAsync();
    }

    public async Task<bool> ValidateToken(int userId, string token)
    {
        var query = @"select isRevoked, expiresAt from dbo.tokens where userId = @userId and token = @token";

        var parameters = new DynamicParameters();
        parameters.Add("userId", userId);
        parameters.Add("token", token);

        _uow.Begin();

        var userToken = await _uow.Connection.QueryFirstAsync<UserToken>(query, parameters, _uow.Transaction);
        await _uow.CompleteAsync();

        return !userToken.IsRevoked && userToken.ExpiresAt >= DateTime.Now;
    }
}
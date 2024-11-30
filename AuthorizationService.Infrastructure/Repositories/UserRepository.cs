using AuthorizationService.Domain.Entities;
using AuthorizationService.Domain.Interfaces;
using Dapper;

namespace AuthorizationService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _uow;

    public UserRepository(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task RegisterAsync(User user)
    {
        var query = @"insert into users (login, password, isActive) values (@login, @password, @isActive); SELECT SCOPE_IDENTITY();";
        var parameters = new DynamicParameters();

        parameters.Add("login", user.Login);
        parameters.Add("password", user.Password);
        parameters.Add("isActive", user.IsActive);

        _uow.Begin();

        var newUserId = await _uow.Connection.ExecuteScalarAsync<int>(query, parameters, _uow.Transaction);

        query = @"select id from dbo.roles where name = 'User'";
        var roleId = await _uow.Connection.ExecuteScalarAsync<int>(query, transaction: _uow.Transaction);

        query = @"insert into userRoles values (@userId, @roleId)";

        var roleParameters = new DynamicParameters();
        roleParameters.Add("userId", newUserId);
        roleParameters.Add("roleId", roleId);

        await _uow.Connection.ExecuteAsync(query, roleParameters, _uow.Transaction);

        await _uow.CompleteAsync();
    }

    public async Task<List<User>> GetShortUsersAsync()
    {
        var query = @"select id, login, isActive from users";

        _uow.Begin();

        var result = await _uow.Connection.QueryAsync<User>(query, transaction: _uow.Transaction);
        await _uow.CompleteAsync();

        return result.ToList(); // вообще автомаппер вроде сам материализует коллекцию когда мапит по дефолту в List<T>, думаю решение нужно принимать исходя из стайл кода
    }

    // можно сделать универсальный метод с параметрами для получения юзера по предикату/не нуловым параметрам, но мне больше нравится так
    public async Task<User> GetUserByLoginAsync(string login)
    {
        var query = @"select id, login, password, isActive from dbo.users where login = @login";

        var parameters = new DynamicParameters();
        parameters.Add("login", login);

        _uow.Begin();

        var user = await _uow.Connection.QuerySingleOrDefaultAsync<User>(query, parameters, _uow.Transaction);

        await _uow.CompleteAsync();

        return user;
    }

    public async Task<List<string>> GetUserRoles(int userId)
    {
        var query = @"select r.name from roles r
                        join userRoles ur on ur.roleId = r.id
                        where ur.userId = @userId";

        var parameters = new DynamicParameters();
        parameters.Add("userId", userId);

        _uow.Begin();

        var userRoles = await _uow.Connection.QueryAsync<string>(query, parameters, _uow.Transaction);

        await _uow.CompleteAsync();

        return userRoles.ToList();
    }

    public async Task SetUserIsActive(int userId, bool isActive)
    {
        var query = @"update dbo.users set isActive = @isActive where id = @userId";

        var parameters = new DynamicParameters();
        parameters.Add("isActive", isActive);
        parameters.Add("userId", userId);

        _uow.Begin();

        await _uow.Connection.ExecuteAsync(query, parameters, _uow.Transaction);
        await _uow.CompleteAsync();
    }
}
using System.Data;
using AuthorizationService.Domain.Interfaces;

namespace AuthorizationService.Infrastructure.UoW;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private IDbConnection _connection;
    private IDbTransaction _transaction;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    public IDbConnection Connection => _connection;

    public IDbTransaction Transaction => _transaction;

    public async Task CompleteAsync()
    {
        if (_transaction != null)
        {
            await Task.Run(() => _transaction.Commit());
        }

        _connection.Close();
    }

    public void Begin()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        _transaction = _connection.BeginTransaction();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Close();
        _connection?.Dispose();
    }
}
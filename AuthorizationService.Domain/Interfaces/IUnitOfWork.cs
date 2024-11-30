using System.Data;

namespace AuthorizationService.Domain.Interfaces;

public interface IUnitOfWork
{
    Task CompleteAsync();

    void Begin();

    void Rollback();

    IDbConnection Connection { get; }

    IDbTransaction Transaction { get; }
}
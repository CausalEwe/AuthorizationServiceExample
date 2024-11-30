using System.Data;
using Microsoft.Data.SqlClient;

namespace AuthorizationService.Infrastructure.Contexts;

public class DbContext
{
    private readonly string _connectionString;

    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
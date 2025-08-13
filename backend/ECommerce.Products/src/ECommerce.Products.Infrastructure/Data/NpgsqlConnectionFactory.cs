using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ECommerce.Products.Infrastructure.Data;

public interface IConnectionFactory
{
    IDbConnection Create();
}

public sealed class NpgsqlConnectionFactory : IConnectionFactory
{
    private readonly IOptions<DbOptions> _options;
    public NpgsqlConnectionFactory(IOptions<DbOptions> options) => _options = options;
    public IDbConnection Create() => new NpgsqlConnection(_options.Value.ConnectionString);
}
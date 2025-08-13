using Dapper;
using ECommerce.Products.Application.Repositories;
using ECommerce.Products.Domain.ValueObjects;
using ECommerce.Products.Infrastructure.Data;

namespace ECommerce.Products.Infrastructure.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly IConnectionFactory _factory;
    public DepartmentRepository(IConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Department>> ListAsync(CancellationToken ct)
    {
        const string sql = "SELECT code, description FROM departments ORDER BY code";
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<(string Code, string Description)>(new CommandDefinition(sql, cancellationToken: ct));
        return rows.Select(r => new Department(r.Code, r.Description)).ToList();
    }
}
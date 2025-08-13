using Dapper;
using ECommerce.Products.Application.Repositories;
using ECommerce.Products.Domain.Entities;
using ECommerce.Products.Infrastructure.Data;

namespace ECommerce.Products.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IConnectionFactory _factory;
    public ProductRepository(IConnectionFactory factory) => _factory = factory;

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM products WHERE code = @code AND deleted_at IS NULL LIMIT 1";
        using var conn = _factory.Create();
        var exists = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { code }, cancellationToken: ct));
        return exists.HasValue;
    }

    public async Task<Guid> InsertAsync(Product p, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO products (id, code, description, department_code, price, is_active, deleted_at, created_at, updated_at)
            VALUES (@Id, @Code, @Description, @DepartmentCode, @Price, @IsActive, NULL, @CreatedAt, @UpdatedAt);
        ";
        using var conn = _factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: ct));
        return p.Id;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
            SELECT id, code, description, department_code AS DepartmentCode, price, is_active AS IsActive,
                   deleted_at AS DeletedAt, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM products WHERE id = @id AND deleted_at IS NULL";
        using var conn = _factory.Create();
        return await conn.QueryFirstOrDefaultAsync<Product>(new CommandDefinition(sql, new { id }, cancellationToken: ct));
    }

    public async Task<Product?> GetByCodeAsync(string code, CancellationToken ct)
    {
        const string sql = @"
            SELECT id, code, description, department_code AS DepartmentCode, price, is_active AS IsActive,
                   deleted_at AS DeletedAt, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM products WHERE code = @code AND deleted_at IS NULL";
        using var conn = _factory.Create();
        return await conn.QueryFirstOrDefaultAsync<Product>(new CommandDefinition(sql, new { code }, cancellationToken: ct));
    }

    public async Task UpdateAsync(Product p, CancellationToken ct)
    {
        const string sql = @"
            UPDATE products SET description = @Description, department_code = @DepartmentCode,
                price = @Price, is_active = @IsActive, updated_at = NOW()
            WHERE id = @Id AND deleted_at IS NULL";
        using var conn = _factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, p, cancellationToken: ct));
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct)
    {
        const string sql = "UPDATE products SET deleted_at = NOW(), is_active = FALSE, updated_at = NOW() WHERE id = @id AND deleted_at IS NULL";
        using var conn = _factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
    }

    public async Task<(IReadOnlyList<Product>, int)> ListAsync(string? search, string? departmentCode, bool? isActive, int page, int pageSize, CancellationToken ct)
    {
        var where = new List<string> { "deleted_at IS NULL" };
        if (!string.IsNullOrWhiteSpace(search)) where.Add("(code ILIKE '%' || @search || '%' OR description ILIKE '%' || @search || '%')");
        if (!string.IsNullOrWhiteSpace(departmentCode)) where.Add("department_code = @departmentCode");
        if (isActive.HasValue) where.Add("is_active = @isActive");
        var whereSql = string.Join(" AND ", where);

        var sql = $@"
            WITH filtered AS (
              SELECT * FROM products WHERE {whereSql}
            )
            SELECT * FROM filtered ORDER BY created_at DESC
            OFFSET @offset LIMIT @limit;
        ";

        var countSql = $"SELECT COUNT(1) FROM products WHERE {whereSql}";

        using var conn = _factory.Create();
        var p = new { search, departmentCode, isActive, offset = (page - 1) * pageSize, limit = pageSize };
        var items = (await conn.QueryAsync<Product>(new CommandDefinition(sql, p, cancellationToken: ct))).ToList();
        var total = await conn.ExecuteScalarAsync<int>(new CommandDefinition(countSql, p, cancellationToken: ct));
        return (items, total);
    }
}
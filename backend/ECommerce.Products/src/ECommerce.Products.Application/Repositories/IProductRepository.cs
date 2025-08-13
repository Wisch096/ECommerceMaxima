namespace ECommerce.Products.Application.Repositories;

public interface IProductRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<Guid> InsertAsync(Domain.Entities.Product product, CancellationToken ct);
    Task<Domain.Entities.Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Domain.Entities.Product?> GetByCodeAsync(string code, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.Product product, CancellationToken ct);
    Task SoftDeleteAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<Domain.Entities.Product> Items, int Total)> ListAsync(string? search, string? departmentCode, bool? isActive, int page, int pageSize, CancellationToken ct);
}
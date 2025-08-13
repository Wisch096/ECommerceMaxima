using ECommerce.Products.Application.DTOs;
using ECommerce.Products.Application.Repositories;
using ECommerce.Products.Domain.Entities;

namespace ECommerce.Products.Application.Services;

public sealed class ProductService(IProductRepository repo)
{
    public async Task<Guid> CreateAsync(ProductRequest req, CancellationToken ct)
    {
        if (await repo.CodeExistsAsync(req.Code, ct))
            throw new InvalidOperationException($"Código '{req.Code}' já existe");

        var entity = Product.Create(Guid.NewGuid(), req.Code, req.Description, req.DepartmentCode, req.Price, req.IsActive);
        return await repo.InsertAsync(entity, ct);
    }

    public async Task<ProductResponse?> GetAsync(Guid id, CancellationToken ct)
    {
        var e = await repo.GetByIdAsync(id, ct);
        return e is null ? null : Map(e);
    }

    public async Task<PagedResult<ProductResponse>> ListAsync(string? search, string? departmentCode, bool? isActive, int page, int pageSize, CancellationToken ct)
    {
        var (items, total) = await repo.ListAsync(search, departmentCode, isActive, page, pageSize, ct);
        return new PagedResult<ProductResponse>(items.Select(Map).ToList(), total, page, pageSize);
    }

    public async Task UpdateAsync(Guid id, ProductUpdateRequest req, CancellationToken ct)
    {
        var e = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Produto não encontrado");
        e.Update(req.Description, req.DepartmentCode, req.Price, req.IsActive);
        await repo.UpdateAsync(e, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct) => repo.SoftDeleteAsync(id, ct);

    private static ProductResponse Map(Product e) => new(e.Id, e.Code, e.Description, e.DepartmentCode, e.Price, e.IsActive);
}
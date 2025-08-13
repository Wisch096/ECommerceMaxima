namespace ECommerce.Products.Application.Repositories;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Domain.ValueObjects.Department>> ListAsync(CancellationToken ct);
}
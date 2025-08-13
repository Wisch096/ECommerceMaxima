namespace ECommerce.Products.Application.DTOs;

public sealed record ProductResponse(
    Guid Id,
    string Code,
    string Description,
    string DepartmentCode,
    decimal Price,
    bool IsActive
);
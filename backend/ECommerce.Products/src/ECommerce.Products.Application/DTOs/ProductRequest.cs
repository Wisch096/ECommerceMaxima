namespace ECommerce.Products.Application.DTOs;

public sealed record ProductRequest(
    string Code,
    string Description,
    string DepartmentCode,
    decimal Price,
    bool IsActive
);
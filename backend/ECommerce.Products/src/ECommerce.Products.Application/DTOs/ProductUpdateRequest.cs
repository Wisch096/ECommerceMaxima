namespace ECommerce.Products.Application.DTOs;

public sealed record ProductUpdateRequest(
    string Description,
    string DepartmentCode,
    decimal Price,
    bool IsActive
);
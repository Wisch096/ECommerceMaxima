namespace ECommerce.Products.Application.DTOs;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
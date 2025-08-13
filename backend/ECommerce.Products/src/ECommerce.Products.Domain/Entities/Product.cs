namespace ECommerce.Products.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Code { get; private set; }
    public string Description { get; private set; }
    public string DepartmentCode { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Product() { }

    public static Product Create(Guid id, string code, string description, string departmentCode, decimal price, bool isActive)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id inválido");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Código obrigatório");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Descrição obrigatória");
        if (string.IsNullOrWhiteSpace(departmentCode)) throw new ArgumentException("Departamento obrigatório");
        if (price < 0) throw new ArgumentException("Preço inválido");

        return new Product
        {
            Id = id,
            Code = code.Trim(),
            Description = description.Trim(),
            DepartmentCode = departmentCode.Trim(),
            Price = price,
            IsActive = isActive,
            DeletedAt = null,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(string description, string departmentCode, decimal price, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Descrição obrigatória");
        if (string.IsNullOrWhiteSpace(departmentCode)) throw new ArgumentException("Departamento obrigatório");
        if (price < 0) throw new ArgumentException("Preço inválido");

        Description = description.Trim();
        DepartmentCode = departmentCode.Trim();
        Price = price;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
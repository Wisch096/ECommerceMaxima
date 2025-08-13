using ECommerce.Products.Application.DTOs;
using FluentValidation;

namespace ECommerce.Products.Application.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(255);
        RuleFor(x => x.DepartmentCode).NotEmpty().Length(3);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
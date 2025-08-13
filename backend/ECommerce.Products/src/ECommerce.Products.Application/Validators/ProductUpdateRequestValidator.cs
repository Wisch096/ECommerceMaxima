using ECommerce.Products.Application.DTOs;
using FluentValidation;

namespace ECommerce.Products.Application.Validators;

public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(255);
        RuleFor(x => x.DepartmentCode).NotEmpty().Length(3);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}